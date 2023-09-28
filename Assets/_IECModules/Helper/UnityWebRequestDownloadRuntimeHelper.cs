using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace _IECModules
{
    public static class UnityWebRequestDownloadRuntimeHelper
    {
        public static UnityWebRequest DownloadFromUrl(string url)
        {
            var uwr = UnityWebRequest.Get(url);
            
            // uwr.SetRequestHeader("accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" ); 
            // // uwr.SetRequestHeader("accept-encoding", "gzip, deflate, br");
            // uwr.SetRequestHeader("accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");

            uwr.SendWebRequest();
            while (!uwr.isDone)
            {
                // Debug.Log(uwr.downloadProgress);
            }

            if (uwr.isNetworkError)
            {
                Debug.LogError("Download Error");
            }
            return uwr;
        }

        public static List<List<string>> DownloadCsvFromAuthUrl(string url)
        {
            var jsoner = new IEC.JsonDotNetModule();
            var uwr = UnityEditorHelper.GetWebRequestCsvFromAuthUrl(url);
            uwr.SendWebRequest();
            while (!uwr.isDone)
            {
                //Debug.Log(uwr.downloadProgress);
            }

            if (uwr.isNetworkError)
            {
                Debug.LogError("Download Error");
            }

            Debug.LogError(uwr.downloadHandler.text);
            return jsoner.FromJson<List<List<string>>>(uwr.downloadHandler.text);
        }
    }
}