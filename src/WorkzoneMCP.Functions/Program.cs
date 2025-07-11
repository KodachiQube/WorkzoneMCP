using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkzoneMCP.Functions.MCP;
using WorkzoneMCP.Core.Interfaces;
using WorkzoneMCP.Services.Services;
using WorkzoneMCP.Services.Clients;
using WorkzoneMCP.Functions.Configuration;
using StreamJsonRpc;

namespace WorkzoneMCP.Functions;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Check if running as MCP server
        if (args.Length > 0 && args[0] == "--mcp")
        {
            await RunAsMcpServer();
            return;
        }

        // Otherwise run as Azure Function
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(context, services);
            })
            .Build();

        await host.RunAsync();
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // Configure logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Register configuration
        services.Configure<AppConfiguration>(context.Configuration.GetSection("App"));

        // Register HTTP client factory with Polly policies
        services.AddHttpClient<IWorkzoneApiClient, WorkzoneApiClient>()
            .AddPolicyHandler(HttpPolicies.GetRetryPolicy())
            .AddPolicyHandler(HttpPolicies.GetCircuitBreakerPolicy());

        // Register services
        services.AddScoped<IWorkzoneService, WorkzoneService>();
        
        // Register MCP server
        services.AddScoped<McpServer>();

        // Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    }

    private static async Task RunAsMcpServer()
    {
        // Create a host for dependency injection
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                });

                // Register configuration from environment variables
                services.Configure<AppConfiguration>(options =>
                {
                    options.WorkzoneApiUrl = Environment.GetEnvironmentVariable("App__WorkzoneApiUrl") ?? "https://api.workzone.com";
                    options.WorkzoneApiKey = Environment.GetEnvironmentVariable("App__WorkzoneApiKey") ?? "";
                    options.TimeoutSeconds = int.Parse(Environment.GetEnvironmentVariable("App__TimeoutSeconds") ?? "30");
                    options.MaxRetryAttempts = int.Parse(Environment.GetEnvironmentVariable("App__MaxRetryAttempts") ?? "3");
                    options.RetryDelayMilliseconds = int.Parse(Environment.GetEnvironmentVariable("App__RetryDelayMilliseconds") ?? "1000");
                });

                // Register HTTP client factory
                services.AddHttpClient<IWorkzoneApiClient, WorkzoneApiClient>();

                // Register services
                services.AddScoped<IWorkzoneService, WorkzoneService>();
                services.AddScoped<McpServer>();
            })
            .Build();

        using (host)
        {
            await host.StartAsync();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting MCP Server...");

            try
            {
                // Get the MCP server instance
                using var scope = host.Services.CreateScope();
                var mcpServer = scope.ServiceProvider.GetRequiredService<McpServer>();

                // Set up JSON-RPC over stdio
                using var input = Console.OpenStandardInput();
                using var output = Console.OpenStandardOutput();

                var messageHandler = new HeaderDelimitedMessageHandler(output, input);
                var jsonRpc = new JsonRpc(messageHandler, mcpServer);

                mcpServer.SetJsonRpc(jsonRpc);

                // Attach to JSON-RPC
                jsonRpc.StartListening();

                logger.LogInformation("MCP Server is running. Listening for requests...");

                // Wait until the JSON-RPC connection is closed
                await jsonRpc.Completion;

                logger.LogInformation("MCP Server stopped.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error running MCP server");
                throw;
            }
            finally
            {
                await host.StopAsync();
            }
        }
    }
}