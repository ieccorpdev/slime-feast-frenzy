using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace _IECModules
{
    public static class UnityWebRequestDownloadRuntimeHelper
    {
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