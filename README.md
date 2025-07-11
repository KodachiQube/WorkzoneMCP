# Workzone MCP Server

A Model Context Protocol (MCP) server that provides AI assistants with access to Workzone case management functionality. This server can run both as an Azure Function and as a standalone MCP server.

## Features

- **Case Management**: Get, create, update, and search Workzone cases
- **Dual Mode**: Run as Azure Function or standalone MCP server
- **Resilient**: Built-in retry policies and circuit breakers
- **Observable**: Application Insights integration
- **Type-Safe**: Fully typed with C# 8.0

## Prerequisites

- .NET 8.0 SDK
- Azure Functions Core Tools v4 (for Azure Function mode)
- Workzone API credentials

## Project Structure

```
WorkzoneMCP/
├── src/
│   ├── WorkzoneMCP.Core/          # Core models and interfaces
│   ├── WorkzoneMCP.Functions/     # Azure Functions and MCP server
│   ├── WorkzoneMCP.Services/      # Business logic and API clients
│   └── WorkzoneMCP.Tests/         # Unit and integration tests
├── scripts/                       # Utility scripts
└── docs/                         # Documentation
```

## Configuration

### Environment Variables

- `App__WorkzoneApiUrl`: Workzone API base URL
- `App__WorkzoneApiKey`: Workzone API key
- `App__TimeoutSeconds`: HTTP timeout (default: 30)
- `App__MaxRetryAttempts`: Max retry attempts (default: 3)
- `App__RetryDelayMilliseconds`: Base retry delay (default: 1000)

### Local Settings

Update `src/WorkzoneMCP.Functions/local.settings.json`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "App__WorkzoneApiUrl": "https://api.workzone.com",
    "App__WorkzoneApiKey": "your-api-key-here"
  }
}
```

## Running the Server

### As MCP Server

```bash
# Using the script
./scripts/run-mcp-server.sh

# Or directly
cd src/WorkzoneMCP.Functions
dotnet run -- --mcp
```

### As Azure Function

```bash
# Using the script
./scripts/run-azure-function.sh

# Or directly
cd src/WorkzoneMCP.Functions
func start
```

## Available Tools

### workzone_get_case
Get a case by ID from Workzone.

**Parameters:**
- `caseId` (string, required): The ID of the case to retrieve

### workzone_create_case
Create a new case in Workzone.

**Parameters:**
- `title` (string, required): Title of the case
- `description` (string, optional): Description of the case
- `caseType` (string, optional): Type of the case

### workzone_update_case
Update an existing case in Workzone.

**Parameters:**
- `caseId` (string, required): The ID of the case to update
- `title` (string, optional): New title
- `description` (string, optional): New description
- `status` (string, optional): New status

### workzone_search_cases
Search for cases in Workzone.

**Parameters:**
- `query` (string, required): Search query

## Testing

### Run Unit Tests
```bash
cd src/WorkzoneMCP.Tests
dotnet test
```

### Test MCP Server
```bash
./scripts/test-mcp-server.sh
```

## Integration with Claude Desktop

Add to your Claude Desktop configuration:

```json
{
  "mcpServers": {
    "workzone": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/WorkzoneMCP/src/WorkzoneMCP.Functions", "--", "--mcp"],
      "env": {
        "App__WorkzoneApiUrl": "https://api.workzone.com",
        "App__WorkzoneApiKey": "your-api-key"
      }
    }
  }
}
```

## Development

### Adding New Tools

1. Add tool definition in `McpServer.cs`
2. Implement handler method
3. Add corresponding service method in `IWorkzoneService`
4. Implement service logic in `WorkzoneService`
5. Add tests

### Building

```bash
dotnet build
```

### Publishing

```bash
dotnet publish -c Release
```

## Troubleshooting

### Common Issues

1. **Connection refused**: Ensure Workzone API is accessible
2. **Authentication failed**: Check API key configuration
3. **Timeout errors**: Increase `TimeoutSeconds` setting

### Logging

- Azure Function mode: Check Application Insights
- MCP mode: Console output with log levels

## License

[Your License Here]

## Contributing

[Your Contributing Guidelines Here]