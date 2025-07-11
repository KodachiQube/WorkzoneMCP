#!/bin/bash

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${GREEN}Testing Workzone MCP Server...${NC}"

# Test initialize request
echo -e "\n${BLUE}Testing initialize request:${NC}"
echo '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"0.1.0","capabilities":{},"clientInfo":{"name":"test-client","version":"1.0.0"}}}' | dotnet run --project ../src/WorkzoneMCP.Functions -- --mcp

# Test tools/list request
echo -e "\n${BLUE}Testing tools/list request:${NC}"
echo '{"jsonrpc":"2.0","id":2,"method":"tools/list","params":{}}' | dotnet run --project ../src/WorkzoneMCP.Functions -- --mcp

# Test tool call
echo -e "\n${BLUE}Testing workzone_search_cases tool:${NC}"
echo '{"jsonrpc":"2.0","id":3,"method":"tools/call","params":{"name":"workzone_search_cases","arguments":{"query":"test"}}}' | dotnet run --project ../src/WorkzoneMCP.Functions -- --mcp

echo -e "\n${GREEN}Test completed!${NC}"