#!/bin/bash

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${GREEN}Starting Workzone MCP Server...${NC}"

# Navigate to the Functions project
cd "$(dirname "$0")/../src/WorkzoneMCP.Functions" || exit

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}Error: .NET SDK is not installed${NC}"
    echo "Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

# Set environment variables
export App__WorkzoneApiUrl="${WORKZONE_API_URL:-https://api.workzone.com}"
export App__WorkzoneApiKey="${WORKZONE_API_KEY:-your-api-key}"
export App__TimeoutSeconds="${WORKZONE_TIMEOUT:-30}"

# Build the project
echo -e "${YELLOW}Building project...${NC}"
dotnet build --configuration Release

# Run as MCP server
echo -e "${GREEN}Starting MCP Server...${NC}"
echo -e "${YELLOW}Server is listening for JSON-RPC requests on stdio${NC}"
dotnet run --configuration Release -- --mcp