namespace WorkzoneMCP.Core.Interfaces;

/// <summary>
/// Workzone service interface
/// </summary>
public interface IWorkzoneService
{
    /// <summary>
    /// Get case by ID
    /// </summary>
    Task<string> GetCaseAsync(string caseId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create new case
    /// </summary>
    Task<string> CreateCaseAsync(object caseData, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update existing case
    /// </summary>
    Task<bool> UpdateCaseAsync(string caseId, object caseData, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Search cases
    /// </summary>
    Task<IEnumerable<object>> SearchCasesAsync(string query, CancellationToken cancellationToken = default);
}

/// <summary>
/// Workzone API client interface
/// </summary>
public interface IWorkzoneApiClient
{
    /// <summary>
    /// Make GET request
    /// </summary>
    Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Make POST request
    /// </summary>
    Task<T?> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Make PUT request
    /// </summary>
    Task<T?> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Make DELETE request
    /// </summary>
    Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
}