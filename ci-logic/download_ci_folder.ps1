echo "download-ci-folder"
cd $UNITY_DIR
Invoke-Webrequest -URI https://window-build--custom-cloud-build.netlify.app/ci.zip -OutFile ci.zip
Expand-Archive -Path ci.zip -DestinationPath $UNITY_DIR
Copy-Item -Path BuildLogic/ci -Destination ci -Recurse
mkdir Assets/CustomBuild
mkdir Assets/CustomBuild/ci
move BuildLogic/ci/Scripts Assets/CustomBuild/ci