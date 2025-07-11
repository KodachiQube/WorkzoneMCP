using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using WorkzoneMCP.Core.Interfaces;
using WorkzoneMCP.Core.Models;
using StreamJsonRpc;

namespace WorkzoneMCP.Functions.MCP;

public class McpServer
{
    private readonly ILogger<McpServer> _logger;
    private readonly IWorkzoneService _workzoneService;
    private JsonRpc? _rpc;

    private readonly List<Tool> _tools = new()
    {
        new Tool
        {
            Name = "workzone_get_case",
            Description = "Get a case by ID from Workzone",
            InputSchema = new
            {
                type = "object",
                properties = new
                {
                    caseId = new { type = "string", description = "The ID of the case to retrieve" }
                },
                required = new[] { "caseId" }
            }
        },
        new Tool
        {
            Name = "workzone_create_case",
            Description = "Create a new case in Workzone",
            InputSchema = new
            {
                type = "object",
                properties = new
                {
                    title = new { type = "string", description = "Title of the case" },
                    description = new { type = "string", description = "Description of the case" },
                    caseType = new { type = "string", description = "Type of the case" }
                },
                required = new[] { "title" }
            }
        },
        new Tool
        {
            Name = "workzone_update_case",
            Description = "Update an existing case in Workzone",
            InputSchema = new
            {
                type = "object",
                properties = new
                {
                    caseId = new { type = "string", description = "The ID of the case to update" },
                    title = new { type = "string", description = "New title" },
                    description = new { type = "string", description = "New description" },
                    status = new { type = "string", description = "New status" }
                },
                required = new[] { "caseId" }
            }
        },
        new Tool
        {
            Name = "workzone_search_cases",
            Description = "Search for cases in Workzone",
            InputSchema = new
            {
                type = "object",
                properties = new
                {
                    query = new { type = "string", description = "Search query" }
                },
                required = new[] { "query" }
            }
        }
    };

    public McpServer(ILogger<McpServer> logger, IWorkzoneService workzoneService)
    {
        _logger = logger;
        _workzoneService = workzoneService;
    }

    public void SetJsonRpc(JsonRpc rpc)
    {
        _rpc = rpc;
    }

    [JsonRpcMethod("initialize")]
    public async Task<object> InitializeAsync(InitializeParams initParams)
    {
        _logger.LogInformation("MCP Initialize request received");
        
        return new
        {
            protocolVersion = "0.1.0",
            capabilities = new ServerCapabilities
            {
                Tools = new { },
                Resources = null,
                Prompts = null
            },
            serverInfo = new
            {
                name = "workzone-mcp-server",
                version = "1.0.0"
            }
        };
    }

    [JsonRpcMethod("tools/list")]
    public async Task<object> ListToolsAsync()
    {
        _logger.LogInformation("MCP Tools list request received");
        
        return new
        {
            tools = _tools
        };
    }

    [JsonRpcMethod("tools/call")]
    public async Task<object> CallToolAsync(ToolCallParams toolParams)
    {
        _logger.LogInformation("MCP Tool call request: {ToolName}", toolParams.Name);
        
        try
        {
            var result = toolParams.Name switch
            {
                "workzone_get_case" => await HandleGetCaseAsync(toolParams.Arguments),
                "workzone_create_case" => await HandleCreateCaseAsync(toolParams.Arguments),
                "workzone_update_case" => await HandleUpdateCaseAsync(toolParams.Arguments),
                "workzone_search_cases" => await HandleSearchCasesAsync(toolParams.Arguments),
                _ => throw new Exception($"Unknown tool: {toolParams.Name}")
            };

            return new
            {
                content = new[]
                {
                    new
                    {
                        type = "text",
                        text = JsonSerializer.Serialize(result)
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling tool {ToolName}", toolParams.Name);
            return new
            {
                content = new[]
                {
                    new
                    {
                        type = "text",
                        text = $"Error: {ex.Message}"
                    }
                },
                isError = true
            };
        }
    }

    private async Task<object> HandleGetCaseAsync(Dictionary<string, object>? arguments)
    {
        if (arguments == null || !arguments.TryGetValue("caseId", out var caseIdObj))
        {
            throw new ArgumentException("Missing required parameter: caseId");
        }

        var caseId = caseIdObj.ToString() ?? throw new ArgumentException("Invalid caseId parameter");
        var caseData = await _workzoneService.GetCaseAsync(caseId);
        
        return new
        {
            success = true,
            data = caseData
        };
    }

    private async Task<object> HandleCreateCaseAsync(Dictionary<string, object>? arguments)
    {
        if (arguments == null || !arguments.TryGetValue("title", out var titleObj))
        {
            throw new ArgumentException("Missing required parameter: title");
        }

        var createData = new
        {
            title = titleObj.ToString(),
            description = arguments.TryGetValue("description", out var descObj) ? descObj.ToString() : null,
            caseType = arguments.TryGetValue("caseType", out var typeObj) ? typeObj.ToString() : null
        };

        var caseId = await _workzoneService.CreateCaseAsync(createData);
        
        return new
        {
            success = true,
            caseId = caseId,
            message = "Case created successfully"
        };
    }

    private async Task<object> HandleUpdateCaseAsync(Dictionary<string, object>? arguments)
    {
        if (arguments == null || !arguments.TryGetValue("caseId", out var caseIdObj))
        {
            throw new ArgumentException("Missing required parameter: caseId");
        }

        var caseId = caseIdObj.ToString() ?? throw new ArgumentException("Invalid caseId parameter");
        
        var updateData = new
        {
            title = arguments.TryGetValue("title", out var titleObj) ? titleObj.ToString() : null,
            description = arguments.TryGetValue("description", out var descObj) ? descObj.ToString() : null,
            status = arguments.TryGetValue("status", out var statusObj) ? statusObj.ToString() : null
        };

        var success = await _workzoneService.UpdateCaseAsync(caseId, updateData);
        
        return new
        {
            success = success,
            message = success ? "Case updated successfully" : "Update failed"
        };
    }

    private async Task<object> HandleSearchCasesAsync(Dictionary<string, object>? arguments)
    {
        if (arguments == null || !arguments.TryGetValue("query", out var queryObj))
        {
            throw new ArgumentException("Missing required parameter: query");
        }

        var query = queryObj.ToString() ?? throw new ArgumentException("Invalid query parameter");
        var cases = await _workzoneService.SearchCasesAsync(query);
        
        return new
        {
            success = true,
            cases = cases,
            count = cases.Count()
        };
    }

    // Optional: Implement resources if needed
    [JsonRpcMethod("resources/list")]
    public async Task<object> ListResourcesAsync()
    {
        _logger.LogInformation("MCP Resources list request received");
        
        return new
        {
            resources = Array.Empty<object>()
        };
    }

    // Optional: Implement prompts if needed
    [JsonRpcMethod("prompts/list")]
    public async Task<object> ListPromptsAsync()
    {
        _logger.LogInformation("MCP Prompts list request received");
        
        return new
        {
            prompts = Array.Empty<object>()
        };
    }
}