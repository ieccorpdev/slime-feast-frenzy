Write-Host "download-ci-folder"
Set-Location $env:UNITY_DIR

# Download the zip file using Invoke-WebRequest
Invoke-WebRequest -Uri "https://window-build--custom-cloud-build.netlify.app/ci.zip" -OutFile "ci.zip"

# Unzip the file using Expand-Archive
Expand-Archive -Path "ci.zip" -DestinationPath "."

# Copy the directory using Copy-Item
Copy-Item -Recurse -Path "BuildLogic/ci" -Destination "ci"

# Create directories using New-Item
New-Item -ItemType Directory -Path "Assets/CustomBuild"
New-Item -ItemType Directory -Path "Assets/CustomBuild/ci"

# Move files using Move-Item
Move-Item -Path "BuildLogic/ci/Scripts" -Destination "Assets/CustomBuild/ci"