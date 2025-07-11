using Polly;
using Polly.Extensions.Http;

namespace WorkzoneMCP.Functions.Configuration;

/// <summary>
/// HTTP resilience policies using Polly
/// </summary>
public static class HttpPolicies
{
    /// <summary>
    /// Get retry policy with exponential backoff
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => !msg.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var logger = context.Values.ContainsKey("logger") ? context.Values["logger"] as ILogger : null;
                    logger?.LogWarning("Retry {RetryCount} after {Delay}ms", retryCount, timespan.TotalMilliseconds);
                });
    }

    /// <summary>
    /// Get circuit breaker policy
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                5,
                TimeSpan.FromSeconds(30),
                onBreak: (result, timespan) =>
                {
                    // Log circuit breaker open
                },
                onReset: () =>
                {
                    // Log circuit breaker reset
                });
    }
}