using Microsoft.Extensions.Logging;
using WorkzoneMCP.Core.Interfaces;

namespace WorkzoneMCP.Services.Services;

/// <summary>
/// Workzone service implementation
/// </summary>
public class WorkzoneService : IWorkzoneService
{
    private readonly ILogger<WorkzoneService> _logger;
    private readonly IWorkzoneApiClient _apiClient;

    public WorkzoneService(ILogger<WorkzoneService> logger, IWorkzoneApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    public async Task<string> GetCaseAsync(string caseId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting case {CaseId}", caseId);
        
        try
        {
            var result = await _apiClient.GetAsync<object>($"/cases/{caseId}", cancellationToken);
            return System.Text.Json.JsonSerializer.Serialize(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting case {CaseId}", caseId);
            throw;
        }
    }

    public async Task<string> CreateCaseAsync(object caseData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new case");
        
        try
        {
            var result = await _apiClient.PostAsync<dynamic>("/cases", caseData, cancellationToken);
            return result?.id?.ToString() ?? throw new InvalidOperationException("Failed to create case");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating case");
            throw;
        }
    }

    public async Task<bool> UpdateCaseAsync(string caseId, object caseData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating case {CaseId}", caseId);
        
        try
        {
            var result = await _apiClient.PutAsync<object>($"/cases/{caseId}", caseData, cancellationToken);
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating case {CaseId}", caseId);
            throw;
        }
    }

    public async Task<IEnumerable<object>> SearchCasesAsync(string query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching cases with query: {Query}", query);
        
        try
        {
            var result = await _apiClient.GetAsync<IEnumerable<object>>($"/cases/search?q={Uri.EscapeDataString(query)}", cancellationToken);
            return result ?? Enumerable.Empty<object>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching cases with query: {Query}", query);
            throw;
        }
    }
}