using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public string id;
        public int versionSelected;
        
        private string m_AccessToken = "glpat-dMxZF-pqZGsF3fns2uNu";

        string[] listDefaultVerion = new string[] {"1.0"};
        string[] listVersionLoad;

        public string[] listVersion =>
            listVersionLoad == null ||
            listVersionLoad.Length < listDefaultVerion.Length
                ? listDefaultVerion
                : listVersionLoad;

        private string m_Cache => id + "_cache";
        private string m_CacheArray => id + "_cacheArray";
        private JArray m_DataArray;
        
        private string BaseUrlSdk => 
            $"{GITLAB_API}/projects/{GITLAB_GROUP_NAME}%2F{id}/repository/";

        private string GetUrl(string version) =>
            BaseUrlSdk + $"archive.zip?sha={version}&path=Assets/";

        public void LoadSdkVersions()
        {
            PlayerPrefs.DeleteKey(m_Cache);
            var url = BaseUrlSdk + "branches";
            Debug.Log(url);
            var versions = new List<string>();
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
            m_DataArray = data;
            for (int i = 2; i <= numPage; i++)
            {
                listPageUrl.Add($"{requestUrl100}&page={i}");
            }

            RequestListUrl(listPageUrl, 0, data, () =>
            {
                for (int i = 0; i < data.Count; i++)
                {
                    var version = (data[i]["name"] ?? "master").Value<string>();
                    if (version != "master" && version != "main")
                    {
                        if (listExclude.Count > 0 && listExclude.Any(x => version.Contains(x))) continue;
                        versions.Add(version);
                    }
                }

                versions = versions.OrderByDescending(x => x).ToList();
                // Debug.Log(id);
                // foreach (var version in versions)
                // {
                //     Debug.Log(version);
                // }
                
                // Debug.LogError(JsonUtility.ToJson(versions));
                PlayerPrefs.SetString(m_Cache, JsonConvert.SerializeObject(versions));
                PlayerPrefs.SetString(m_CacheArray, JsonConvert.SerializeObject(data));
                versionSelected = 0;
                LoadPKGVersionFromCache();
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

        public bool LoadPKGVersionFromCache()
        {
            if (listVersionLoad != null) return true;
            var data = PlayerPrefs.GetString(m_Cache);
            var dataArray = PlayerPrefs.GetString(m_CacheArray);
            if (string.IsNullOrEmpty(data)) return false;

            listVersionLoad = JsonConvert.DeserializeObject<string[]>(data);
            m_DataArray = JsonConvert.DeserializeObject<JArray>(dataArray);
            return true;
        }

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
                        UnzipAndAddModule(version, webRequest.downloadHandler.data);
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

        void UnzipAndAddModule(string version, byte[] webData)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentDir, "module.zip");
            EditorUtility.DisplayProgressBar("Extract Module", "", 0f);
            var pathModule = currentDir + "/module";
            if (Directory.Exists(pathModule)) Directory.Delete(pathModule, true);
            ZipUtils.ExtractZipFile(webData, pathModule);
            EditorUtility.DisplayProgressBar("Copy Module", "", 0f);
            var pathAssetsModule = Directory.GetDirectories(pathModule)[0] + "/Assets";
            var pathAsset = currentDir + "/Assets";
            var directoriesInfo = new DirectoryInfo(pathAssetsModule).GetDirectories();
            
            var oldPackageFile = new FileInfo(Path.Combine(pathAssetsModule, "package.json"));
            if (oldPackageFile.Exists)
            {
                oldPackageFile.Delete();
            }
            
            var packageFile = Path.Combine(pathAsset, "package.json");
            FileInfo fi = new FileInfo(packageFile);

            JToken packageJson = new JObject();
            if (fi.Exists)
            {
                using StreamReader sr = fi.OpenText();
                string s = sr.ReadToEnd();
                packageJson = JToken.Parse(s);
            }
            
            ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginFacebookAudienceNetwork);
            ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginFacebook);
            ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginApplovin);
            ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginAdjust);
            ScriptingDefineSymbolEditor.RemoveDefineSymbol(PluginDefineSymbol.PluginMaxAmazonNetwork);

            if (packageJson["sdk"] != null)
            {
                var listOldFolders = packageJson["sdk"]["folders"].ToObject<List<string>>();
                foreach (var dir in listOldFolders)
                {
                    var dirName = Path.Combine(pathAsset, dir);
                    if (Directory.Exists(dirName)) 
                        Directory.Delete(dirName, true);
                }
            }

            var listModuleDirs = directoriesInfo.Select(x => x.Name).ToList();
            if (listModuleDirs.Exists(x => x.ToLower().Contains("amazon")))
            {
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginMaxAmazonNetwork);
            }

            var files = Directory.GetFiles(pathAssetsModule);
            foreach (var file in files)
            {
                var fileName = GetBetweenString(file, pathAssetsModule, string.Empty, false);
                if (File.Exists(pathAsset + fileName)) File.Delete(pathAsset + fileName);
            }

            CopyAll(pathAssetsModule, pathAsset);
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            Directory.Delete(pathModule, true);

            Debug.Log("version: " + version);;
            using StreamWriter sw = fi.CreateText();
            var versionData = m_DataArray.FirstOrDefault(x => x["name"].Value<string>() == version);

            versionData["folders"] = JArray.FromObject(listModuleDirs);
            packageJson["sdk"] = versionData;

            var json = JsonPrettify(packageJson.JsonSerialize());
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