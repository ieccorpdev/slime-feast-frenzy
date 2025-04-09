using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace _IECModules
{ 
    public class NetworkBase
    {
        public const string BASE = "https://leaderboard.haleinteractive.vn/api";
        public static async void Post(string url, object body, Action<string> onDone = null, Action onError = null, Dictionary<string, string> headers = null)
        {
            try
            {
                var bodyStr = JsonConvert.SerializeObject(body);
                Debug.Log("Post Request:\nUrl:" + url + "\nBody:" + bodyStr);
                var request = UnityWebRequest.Post(url, bodyStr);
                request.timeout = 10;
                var bodyRaw = Encoding.UTF8.GetBytes(bodyStr);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Content-Type", "application/json");
                if (headers != null)
                {
                    foreach (var h in headers)
                    {
                        request.SetRequestHeader(h.Key, h.Value);
                    }
                }

                var res = request.SendWebRequest();
                while (!res.isDone)
                {
                    await Task.Yield();
                }

                ProcessResResult(res, onDone, onError);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public static async void Get(string url, object body, Action<string> onDone = null, Action onError = null, Dictionary<string, string> headers = null)
        {
            try
            {
                Debug.Log("Get Request:\nUrl:" + url);
                if (body != null)
                {
                    var bodyStr = JsonConvert.SerializeObject(body);
                    var bodyJson = SimpleJSON.JSON.Parse(bodyStr);
                    url += "?";
                    foreach (var key in bodyJson.Keys)
                    {
                        url += key + "=" + bodyJson[key] + "&";
                    }
                    url = url.Remove(url.Length - 1, 1);
                }
                var request = UnityWebRequest.Get(url);
                request.timeout = 10;
                request.SetRequestHeader("Content-Type", "application/json");
                if (headers != null)
                {
                    foreach (var h in headers)
                    {
                        request.SetRequestHeader(h.Key, h.Value);
                    }
                }
                var res = request.SendWebRequest();
                while (!res.isDone)
                {
                    await Task.Yield();
                }

                ProcessResResult(res, onDone, onError);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public static async void Put(string url, object body, Action<string> onDone = null, Action onError = null, Dictionary<string, string> headers = null)
        {
            try
            {
                var bodyStr = body != null ? JsonConvert.SerializeObject(body): string.Empty;
                Debug.Log("Put Request:\nUrl:" + url);
                var request = UnityWebRequest.Put(url, bodyStr);
                request.timeout = 10;
                request.SetRequestHeader("Content-Type", "application/json");
                if (headers != null)
                {
                    foreach (var h in headers)
                    {
                        request.SetRequestHeader(h.Key, h.Value);
                    }
                }
                var res = request.SendWebRequest();
                while (!res.isDone)
                {
                    await Task.Yield();
                }

                ProcessResResult(res, onDone, onError);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        static void ProcessResResult(UnityWebRequestAsyncOperation res, Action<string> onDone = null, Action onError = null)
        {
            if (res.webRequest.result == UnityWebRequest.Result.Success)
            {
                var result = res.webRequest.downloadHandler.text;
                var resultJson = SimpleJSON.JSON.Parse(result);
                if (resultJson["statusCode"] == 200)
                {
                    Debug.Log("<color=#00ff00ff>Success</color>:" + result);
                    // Debug.Log(resultJson["data"]);
                    onDone?.Invoke(resultJson["data"].ToString());
                }
                else
                {
                    Debug.Log("<color=#ff0000ff>Failed</color>:" + result);
                }
            }
            else
            {
                Debug.Log("<color=#ff0000ff>Failed</color>:" + res.webRequest.error);
                onError?.Invoke();
            }
        }
    }
}