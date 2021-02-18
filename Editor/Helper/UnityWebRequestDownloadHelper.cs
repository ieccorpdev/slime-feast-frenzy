using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace _IECModules
{
    public static class UnityWebRequestDownloadHelper
    {
        public static byte[] DownloadDataFromUrl(string url)
        {
            var uwr = DownloadFromUrl(url);
            var data = uwr.downloadHandler.data;
            uwr.Dispose();
            return data;
        }
        public static string DownloadTextFromUrl(string url)
        {
            var uwr = DownloadFromUrl(url);
            var text = uwr.downloadHandler.text;
            uwr.Dispose();
            return text;
        }
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
                LogEditorHelper.AddLogError($"Downloading: {url}");
            }
            return uwr;
        }
        public static List<List<string>> DownloadCsvFromAuthUrl(string url)
        {
            var jsoner = new IEC.JsonDotNetModule();
            var hostUrl = "passive.discordbot.game.ieccorp.vn:3002/sheet";
            var dictContent = new Dictionary<string, string>
            {
                ["token"] = "WinterWolf2020",
                ["sheetUrl"] = url,
                ["format"] = "jsonarray"
            };

            var uwr = new UnityWebRequest(hostUrl, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsoner.ToJson(dictContent));
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            uwr.SendWebRequest();
            while (!uwr.isDone)
            {
                //Debug.Log(uwr.downloadProgress);
            }

            if (uwr.isNetworkError)
            {
                Debug.LogError("Download Error");
                LogEditorHelper.AddLogError($"Downloading: {url}");
            }

            return jsoner.FromJson<List<List<string>>>(uwr.downloadHandler.text);
        }
    }
}