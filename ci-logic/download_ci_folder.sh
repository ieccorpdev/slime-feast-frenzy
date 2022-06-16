echo "download-ci-folder"
cd $UNITY_DIR
wget "https://beta--custom-cloud-build.netlify.app/ci.zip"
unzip ci.zip
cp -r BuildLogic/ci ci
mv BuildLogic/ci/Scripts Assets