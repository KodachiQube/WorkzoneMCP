#!/bin/bash

# Create the basic file structure for WorkzoneMCP
# Run this from ~/DevProjects/WorkzoneMCP/WorkzoneMCP

echo "Creating WorkzoneMCP project structure..."

# Create directories
mkdir -p src/WorkzoneMCP.Core/{Constants,Interfaces,Models/WorkzoneModels}
mkdir -p src/WorkzoneMCP.Functions/{Functions,Properties,ToolHandlers}
mkdir -p src/WorkzoneMCP.Services/{Clients,Services,Properties,ToolHandlers}
mkdir -p src/WorkzoneMCP.Tests/{Unit/Services,Integration}

# Create the solution file
cat > WorkzoneMCP.sln << 'EOF'

Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "src", "src", "{E4B3C2D1-8F3A-4B2C-9E1D-7A6B5C4D3E2F}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "WorkzoneMCP.Core", "src\WorkzoneMCP.Core\WorkzoneMCP.Core.csproj", "{A1B2C3D4-E5F6-7890-1234-567890ABCDEF}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "WorkzoneMCP.Functions", "src\WorkzoneMCP.Functions\WorkzoneMCP.Functions.csproj", "{B2C3D4E5-F6A7-8901-2345-6789ABCDEF01}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "WorkzoneMCP.Services", "src\WorkzoneMCP.Services\WorkzoneMCP.Services.csproj", "{C3D4E5F6-A7B8-9012-3456-789ABCDEF012}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "WorkzoneMCP.Tests", "src\WorkzoneMCP.Tests\WorkzoneMCP.Tests.csproj", "{D4E5F6A7-B8C9-0123-4567-89ABCDEF0123}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(NestedProjects) = preSolution
		{A1B2C3D4-E5F6-7890-1234-567890ABCDEF} = {E4B3C2D1-8F3A-4B2C-9E1D-7A6B5C4D3E2F}
		{B2C3D4E5-F6A7-8901-2345-6789ABCDEF01} = {E4B3C2D1-8F3A-4B2C-9E1D-7A6B5C4D3E2F}
		{C3D4E5F6-A7B8-9012-3456-789ABCDEF012} = {E4B3C2D1-8F3A-4B2C-9E1D-7A6B5C4D3E2F}
		{D4E5F6A7-B8C9-0123-4567-89ABCDEF0123} = {E4B3C2D1-8F3A-4B2C-9E1D-7A6B5C4D3E2F}
	EndGlobalSection
EndGlobal
EOF

echo "Solution file created!"
echo "Next, run ./create-all-project-files.sh to create all the source files."
