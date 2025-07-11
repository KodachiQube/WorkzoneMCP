#!/bin/bash

# This script creates a minimal MCP server implementation for Workzone
# Run from ~/DevProjects/WorkzoneMCP/WorkzoneMCP

echo "Creating minimal MCP server for Workzone..."

# Create the minimal MCP folder structure
mkdir -p src/MinimalMCP

# Create a minimal Program.cs
cat > src/MinimalMCP/Program.cs << 'EOF'
using System;
using System.Text.Json;

namespace MinimalMCP;

public class Program
{
    public static void Main(string[] args)
    {
        // Read from stdin
        string? line;
        while ((line = Console.ReadLine()) != null)
        {
            try
            {
                var request = JsonDocument.Parse(line);
                var method = request.RootElement.GetProperty("method").GetString();
                var id = request.RootElement.GetProperty("id");
                
                object result = method switch
                {
                    "initialize" => new { capabilities = new { }, serverInfo = new { name = "minimal-mcp", version = "0.1.0" } },
                    "tools/list" => new { tools = new[] { new { name = "test_tool", description = "Test tool" } } },
                    _ => null
                };

                var response = new
                {
                    jsonrpc = "2.0",
                    id = id,
                    result = result
                };

                Console.WriteLine(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
EOF

# Create project file
cat > src/MinimalMCP/MinimalMCP.csproj << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
EOF

echo "Minimal MCP server created! To run:"
echo "cd src/MinimalMCP && dotnet run"
