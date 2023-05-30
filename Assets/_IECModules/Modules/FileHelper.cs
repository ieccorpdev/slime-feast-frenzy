#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace _IECModules
{
    public class FileHelper : MonoBehaviour
    {
        public static FileHelper Instance => FindObjectOfType<FileHelper>();
        public static string KeystoreSamplePath => ProjectPath + "/{0}.keystore";
        public static string IconSamplePath => "Assets/_IEC/Image/icon.png";
        public static string IconSampleFullPath => ProjectAssetPath + "/_IEC/Image/icon.png";

        public static string ProjectPath => System.IO.Directory.GetCurrentDirectory();
        public static string ProjectAssetPath => Application.dataPath;

        public static string KeystorePath(string fileName)
        {
            return string.Format(KeystoreSamplePath, fileName);
        }

        public static bool FileExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }
        public static void DeleteAllKeystore(string exception)
        {
            DirectoryInfo dic = new DirectoryInfo(ProjectPath);
            FileInfo[] files = dic.GetFiles("*.keystore");
            foreach (var file in files)
            {
                 if (!file.Name.Contains(exception+".keystore"))
                file.Delete();
            }
        }

        public static bool FileKeystoreExist(string keyStoreName)
        {
            return FileExists(string.Format(KeystoreSamplePath, keyStoreName));
        }

        public void DownloadFile(string url, string savePath, System.Action<bool, string> OnDownloadDone)
        {
            StartCoroutine(DoDownloadFile(url, savePath, OnDownloadDone));
        }

        IEnumerator DoDownloadFile(string url, string savePath, System.Action<bool, string> OnDownloadDone)
        {
            yield return null;
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += (sender, args) =>
                {
                    if (args.Error != null)
                    {
                        Debug.LogError(args.Error.ToString());
                        OnDownloadDone?.Invoke(false, args.Error.ToString());
                    }
                    else
                    {
                        OnDownloadDone?.Invoke(true, savePath);
                    }
                };
                webClient.DownloadFileAsync(new System.Uri(url), savePath);
            }

            //{
            //    yield return www.Send();
            //    if (www.isNetworkError || www.isHttpError)
            //    {
            //        Debug.Log(www.error);
            //        OnDownloadDone?.Invoke(false, www.error);
            //    }
            //    else
            //    {
            //        OnDownloadDone?.Invoke(true, savePath);
            //    }

            //}
            //    //  Debug.Log("Start download f");
            //    var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            //// string savePath = Path.Combine(Application.persistentDataPath, "unity3d.html");
            //uwr.downloadHandler = new DownloadHandlerFile(savePath);
            //yield return uwr.SendWebRequest();
            //if (uwr.result != UnityWebRequest.Result.Success)
            //{
            //    Debug.LogError(uwr.error);
            //    OnDownloadDone?.Invoke(false, uwr.error);
            //}
            //else
            //{
            //    Debug.Log("File successfully downloaded and saved to " + savePath);
            //    OnDownloadDone?.Invoke(true, savePath);
            //}
        }

    }
}
#endif