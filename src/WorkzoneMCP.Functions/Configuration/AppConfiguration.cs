namespace WorkzoneMCP.Functions.Configuration;

/// <summary>
/// Application configuration settings
/// </summary>
public class AppConfiguration
{
    /// <summary>
    /// Workzone API URL
    /// </summary>
    public string WorkzoneApiUrl { get; set; } = string.Empty;

    /// <summary>
    /// Workzone API Key
    /// </summary>
    public string WorkzoneApiKey { get; set; } = string.Empty;

    /// <summary>
    /// HTTP request timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum retry attempts
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Base delay for exponential backoff in milliseconds
    /// </summary>
    public int RetryDelayMilliseconds { get; set; } = 1000;
}