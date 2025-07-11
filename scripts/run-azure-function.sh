#!/bin/bash

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${GREEN}Starting Workzone Azure Function...${NC}"

# Navigate to the Functions project
cd "$(dirname "$0")/../src/WorkzoneMCP.Functions" || exit

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}Error: .NET SDK is not installed${NC}"
    echo "Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

# Restore dependencies
echo -e "${YELLOW}Restoring dependencies...${NC}"
dotnet restore

# Build the project
echo -e "${YELLOW}Building project...${NC}"
dotnet build --configuration Debug

# Check if Azure Functions Core Tools is installed
if ! command -v func &> /dev/null; then
    echo -e "${RED}Error: Azure Functions Core Tools is not installed${NC}"
    echo "Install with: npm install -g azure-functions-core-tools@4 --unsafe-perm true"
    exit 1
fi

# Start the Azure Function
echo -e "${GREEN}Starting Azure Function on http://localhost:7071${NC}"
echo -e "${YELLOW}Health check endpoint: http://localhost:7071/api/health${NC}"
func start --verbose