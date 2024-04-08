echo "download-ci-folder"
cd $UNITY_DIR
wget "https://custom-gradle-7-java-11--custom-cloud-build.netlify.app/ci.zip"
unzip ci.zip
cp -r BuildLogic/ci ci
mkdir Assets/CustomBuild
mkdir Assets/CustomBuild/ci
mv BuildLogic/ci/Scripts Assets/CustomBuild/ci