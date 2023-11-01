using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _IECModules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace IEC.ModuleManager
{
    public class GitModule
    {
        const string GITLAB_API = "https://gitlab.com/api/v4";
        const string GITLAB_GROUP_NAME = "iecgames%2Funity-module-sdk";
        public string id = "com.iec.plugin";
        public string jsonID = "sdk";
        public bool isAddAsset;
        public bool isAddPackage;
        public int versionSelected;
        
        private string m_AccessToken = "glpat-caUzdnAjgtdU4qzQ1AEx";

        public string CurrentVersion { get; private set; }
        public string CurrentCommit { get; private set; }
        public string CurrentCommitTitle { get; private set; }
        public string CurrentCommitDate { get; private set; }
        
        public string[] ListVersion { get; private set; }

        private Dictionary<string, JToken> m_DictData = null;
        private bool m_IsLoadingVersion = false;

        private string BaseUrlSdk => 
            $"{GITLAB_API}/projects/{GITLAB_GROUP_NAME}%2F{id}/repository/";

        private string GetUrl(string version)
        {
            var subFolder = isAddAsset ? "Assets" : "";
            subFolder = isAddPackage ? "Packages" : subFolder;
            return BaseUrlSdk + $"archive.zip?sha={version}&path={subFolder}/";
        }
        
        private Dictionary<string, PluginDefineSymbol> dictAssetDefineSymbol =
            new Dictionary<string, PluginDefineSymbol>()
            {
                { "FacebookSDK", PluginDefineSymbol.PluginFacebook },
                { "AudienceNetwork", PluginDefineSymbol.PluginFacebookAudienceNetwork },
                { "Adjust", PluginDefineSymbol.PluginAdjust },
                { "MaxSdk", PluginDefineSymbol.PluginApplovin },
                { "Amazon", PluginDefineSymbol.PluginMaxAmazonNetwork },
                { @"MaxSdk\MEDIATION\StartIO", PluginDefineSymbol.PuginStartIONetwork },
            };
        
        private Dictionary<string, PluginDefineSymbol> dictPackageDefineSymbol =
            new Dictionary<string, PluginDefineSymbol>()
            {
                { SdkHelper.FIREBASE_APP_CORE, PluginDefineSymbol.PluginFirebase },
                { SdkHelper.FIREBASE_ANALYTIC_PKG, PluginDefineSymbol.PluginFirebaseAnalytic },
                { SdkHelper.FIREBASE_REMOTE_CONFIG_PKG, PluginDefineSymbol.PluginFirebaseRemote },
                { SdkHelper.IN_APP_RATING_PKG, PluginDefineSymbol.PluginInAppRating },
                { SdkHelper.NOTIFICATION_PKG, PluginDefineSymbol.PluginNotification },
                { SdkHelper.I2_LOCALIZATION_PKG, PluginDefineSymbol.PluginLocalization },
            };

        private List<string> listPackageSDK =
            new List<string>()
            {
                SdkHelper.EXTERNAL_DEPENDENCY_PKG,
                SdkHelper.FIREBASE_APP_CORE,
                SdkHelper.FIREBASE_REMOTE_CONFIG_PKG,
                SdkHelper.FIREBASE_ANALYTIC_PKG,
                SdkHelper.IN_APP_RATING_PKG,
                SdkHelper.ANDROID_APPBUNDLE,
                SdkHelper.GOOGLE_PLAY_COMMON,
                SdkHelper.GOOGLE_PLAY_CORE,
                SdkHelper.NOTIFICATION_PKG,
                SdkHelper.I2_LOCALIZATION_PKG,
            };

        public void LoadSdkVersions()
        {
            if (m_IsLoadingVersion) 
                return;
            m_IsLoadingVersion = true;
            var url = BaseUrlSdk + "branches";
            
            var requestUrl100 = url + "?per_page=100";
            var listExclude = new List<string> {"deleted", "deprecated"};
            var request = new UnityWebRequest(new Uri(requestUrl100));
            var downloadHandler = new DownloadHandlerBuffer();
            request.downloadHandler = downloadHandler;
            request.SetRequestHeader("PRIVATE-TOKEN", m_AccessToken);
            request.SendWebRequest();
            while (!request.isDone)
            {
            }

            // Debug.Log(request.downloadHandler.text);
            var numPage = int.Parse(request.GetResponseHeader("x-total-pages"));
            var listPageUrl = new List<string>();
            var data = JsonConvert.DeserializeObject<JArray>(request.downloadHandler.text);
            
            for (int i = 2; i <= numPage; i++)
            {
                listPageUrl.Add($"{requestUrl100}&page={i}");
            }

            RequestListUrl(listPageUrl, 0, data, () =>
            {
                m_DictData = data.ToDictionary(x => x["name"].Value<string>(), x => x);
                m_DictData = m_DictData.Where(kvp =>
                    {
                        var version = kvp.Key;
                        if (version == "master" || version == "main") 
                            return false;
                        if (listExclude.Count > 0 && listExclude.Any(x => version.Contains(x))) 
                            return false;
                        return true;
                    })
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                ListVersion = m_DictData.Keys.OrderBy(x => x).ToArray();
                
                versionSelected = 0;
                SetCurrentVersionInfo();

                if (ListVersion.Contains(CurrentVersion))
                {
                    versionSelected = ListVersion.ToList().IndexOf(CurrentVersion);
                }
                
                m_IsLoadingVersion = false;
                //Debug.Log($"Load done {ListVersion.Length} versions");
            });
        }

        void RequestListUrl(List<string> listUrl, int index, JArray data, Action callback)
        {
            if (index < listUrl.Count)
            {
                var url = listUrl[index];
                //Debug.Log("page:" + url);
                var request = new UnityWebRequest(new Uri(url));
                var downloadHandler = new DownloadHandlerBuffer();
                request.downloadHandler = downloadHandler;
                request.SetRequestHeader("PRIVATE-TOKEN", m_AccessToken);
                request.SendWebRequest();
                while (!request.isDone)
                {
                }

                if (request.error != null)
                {
                    Debug.LogError(request.error);
                    RequestListUrl(listUrl, index + 1, data, callback);
                    return;
                }

                //Debug.Log(request.downloadHandler.text);
                var dataItem = JsonConvert.DeserializeObject<JArray>(request.downloadHandler.text);
                foreach (var t in dataItem)
                {
                    data.Add(t);
                }

                RequestListUrl(listUrl, index + 1, data, callback);
            }
            else
            {
                callback?.Invoke();
            }
        }

        public bool IsVersionsLoaded => m_DictData != null;

        public void DownloadModule(string version)
        {
            var webRequest = UnityWebRequest.Get(GetUrl(version));
            var downloadHandler = new DownloadHandlerBuffer();
            webRequest.downloadHandler = downloadHandler;
            webRequest.SetRequestHeader("PRIVATE-TOKEN", m_AccessToken);
            webRequest.SendWebRequest();
            EditorUtility.DisplayProgressBar("Download Module", "", 0f);
            EditorCoroutine.StartCoroutine(() => webRequest.isDone, () =>
            {
                EditorUtility.DisplayProgressBar("Download Module", "", 1f);
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var data = m_DictData[version];
                        UnzipAndAddModule(version, webRequest.downloadHandler.data, data);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        EditorUtility.ClearProgressBar();
                    }
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                }
            });
        }
        
        JToken GetJsonFromFile(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            JToken packageJson = new JObject();
            if (fi.Exists)
            {
                using StreamReader sr = fi.OpenText();
                string s = sr.ReadToEnd();
                packageJson = JToken.Parse(s);
                sr.Close();
            }
            return packageJson;
        }

        JToken GetPackageJson()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var pathAsset = currentDir + "/Assets";
            var packageFile = Path.Combine(pathAsset, "package.json");
            return GetJsonFromFile(packageFile);
        }

        void SetCurrentVersionInfo()
        {
            JToken packageJson = GetPackageJson();
            if (packageJson[jsonID] == null)
                return;
            var sdk = packageJson[jsonID];
            var commit = sdk["commit"];
            CurrentVersion = sdk["name"].Value<string>();
            CurrentCommit = commit["short_id"].Value<string>();
            CurrentCommitTitle = commit["title"].Value<string>();
            CurrentCommitDate = commit["committed_date"].Value<string>();
        }

        void UnzipAndAddModule(string version, byte[] webData, JToken versionData)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentDir, "module.zip");
            EditorUtility.DisplayProgressBar("Extract Module", "", 0f);
            var pathModule = currentDir + "/module";
            if (Directory.Exists(pathModule)) Directory.Delete(pathModule, true);
            ZipUtils.ExtractZipFile(webData, pathModule);
            EditorUtility.DisplayProgressBar("Copy Module", "", 0f);
            
            if (isAddAsset)
                AddFromAsset(version, versionData);
            
            if (isAddPackage)
                AddFromPackage(version, versionData);
            
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            Directory.Delete(pathModule, true);
            EditorApplication.ExecuteMenuItem("Assets/Refresh");
            Debug.Log($"Done UnzipAndAddModule {version}");
        }

        void AddFromAsset(string version, JToken versionData)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var pathModule = currentDir + "/module";
            var pathAssetsModule = Directory.GetDirectories(pathModule)[0] + "/Assets";
            var pathAsset = currentDir + "/Assets";
            var directoriesInfo = new DirectoryInfo(pathAssetsModule).GetDirectories();
            
            var oldPackageFile = new FileInfo(Path.Combine(pathAssetsModule, "package.json"));
            if (oldPackageFile.Exists)
            {
                oldPackageFile.Delete();
            }
            
            JToken packageJson = GetPackageJson();

            if (packageJson[jsonID] != null)
            {
                var listOldFolders = packageJson[jsonID]["folders"].ToObject<List<string>>();
                foreach (var dir in listOldFolders)
                {
                    var dirName = Path.Combine(pathAsset, dir);
                    if (Directory.Exists(dirName)) 
                        Directory.Delete(dirName, true);
                }
            }

            var listModuleDirs = directoriesInfo.Select(x => x.Name).ToList();

            var listSymbols = dictAssetDefineSymbol.Values.Distinct().ToList();
            foreach (var symbol in listSymbols)
            {
                ScriptingDefineSymbolEditor.RemoveDefineSymbol(symbol);
            }

            foreach (var kv in dictAssetDefineSymbol)
            {
                var pathCheck = Path.Combine(pathAssetsModule, kv.Key);
                if (Directory.Exists(pathCheck))
                {
                    ScriptingDefineSymbolEditor.AddDefineSymbol(kv.Value);
                }
            }

            var files = Directory.GetFiles(pathAssetsModule);
            foreach (var file in files)
            {
                var fileName = GetBetweenString(file, pathAssetsModule, string.Empty, false);
                if (File.Exists(pathAsset + fileName))
                {
                    File.Delete(pathAsset + fileName);
                }
            }

            CopyAll(pathAssetsModule, pathAsset);
            
            var packageFile = Path.Combine(pathAsset, "package.json");
            FileInfo fi = new FileInfo(packageFile);
            using StreamWriter sw = fi.CreateText();
            versionData["folders"] = JArray.FromObject(listModuleDirs);
            packageJson[jsonID] = versionData;

            var json = JsonPrettify(packageJson.JsonSerialize());
            sw.Write(json);
            sw.Close();
            
            SetCurrentVersionInfo();
        }

        void AddFromPackage(string version, JToken versionData)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var pathModule = currentDir + "/module";
            var pathModulePackage = Directory.GetDirectories(pathModule)[0] + "/Packages";
            var pathAsset = currentDir + "/Assets";
            var pathPackage = currentDir + "/Packages";

            JToken packageJson = GetPackageJson();
            JToken moduleManifest = GetJsonFromFile(Path.Combine(pathModulePackage, "manifest.json"));
            JToken manifest = GetJsonFromFile(Path.Combine(pathPackage, "manifest.json"));
            
            var moduleDependencies = moduleManifest["dependencies"];
            var dependencies = manifest["dependencies"];
            
            foreach (var packageName in listPackageSDK)
            {
                var curData = dependencies[packageName] as JProperty;
                var newData = moduleDependencies[packageName];
                if (curData != null)
                {
                    curData.Remove();
                    if (dictPackageDefineSymbol.TryGetValue(packageName, out var s))
                    {
                        ScriptingDefineSymbolEditor.RemoveDefineSymbol(s);
                    }
                }
                if (newData != null)
                {
                    dependencies[packageName] = newData;
                    if (dictPackageDefineSymbol.TryGetValue(packageName, out var s))
                    {
                        ScriptingDefineSymbolEditor.AddDefineSymbol(s);
                    }
                }
            }

            packageJson[jsonID] = versionData;
            
            WriteJsonToFile(Path.Combine(pathPackage, "manifest.json"), manifest);
            WriteJsonToFile(Path.Combine(pathAsset, "package.json"), packageJson);
            
            SetCurrentVersionInfo();
        }

        void WriteJsonToFile(string filePath, JToken data)
        {
            FileInfo fi = new FileInfo(filePath);
            using StreamWriter sw = fi.CreateText();
            var json = JsonPrettify(data.JsonSerialize());
            sw.Write(json);
            sw.Close();
        }
        
        static string JsonPrettify(string json)
        {
            using var stringReader = new StringReader(json);
            using var stringWriter = new StringWriter();
            var jsonReader = new JsonTextReader(stringReader);
            var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
            jsonWriter.WriteToken(jsonReader);
            return stringWriter.ToString();
        }
        
        static string GetBetweenString(string original, string start, string end,
            bool includeStartEndString = true)
        {
            var pFrom = original.IndexOf(start, StringComparison.Ordinal);
            var pTo = original.IndexOf(end, pFrom + start.Length, StringComparison.Ordinal);
            if (end == string.Empty || end == "") pTo = original.Length;

            if (pFrom == -1 || pTo == -1) return "";
            return includeStartEndString
                ? original.Substring(pFrom, pTo - pFrom + end.Length)
                : original.Substring(pFrom + start.Length, pTo - pFrom - start.Length);
        }

        private void CopyAll(string source, string target)
        {
            var allDirs = Directory.GetDirectories(source, "*", SearchOption.AllDirectories);
            foreach (var dir in allDirs)
            {
                var dirCreate = dir.Replace(source, target);
                Directory.CreateDirectory(dirCreate);
            }

            var allFiles = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                var fileCpy = file.Replace(source, target);
                File.Copy(file, fileCpy, true);
            }
        }
    }
}