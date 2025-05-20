using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using IEC;

namespace _IECModules
{ 
    public class NetworkBase
    {
        public const string BASE = "https://leaderboard.haleinteractive.vn/api";
        public const int TIMEOUT = 10;
        public static async UniTask<string> Post(string url, object body, Dictionary<string, string> headers = null)
        {
            try
            {
                var bodyStr = JsonConvert.SerializeObject(body);
                Debug.Log("Post Request:\nUrl:" + url + "\nBody:" + bodyStr);
                var request = UnityWebRequest.Post(url, bodyStr);
                request.timeout = TIMEOUT;
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

                await request.SendWebRequest().ToUniTask();
                return ProcessResResult(request);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return string.Empty;
            }
        }

        public static async UniTask<string> Get(string url, object body, Dictionary<string, string> headers = null)
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
                request.timeout = TIMEOUT;
                request.SetRequestHeader("Content-Type", "application/json");
                if (headers != null)
                {
                    foreach (var h in headers)
                    {
                        request.SetRequestHeader(h.Key, h.Value);
                    }
                }
                await request.SendWebRequest().ToUniTask();
                return ProcessResResult(request);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return string.Empty;
            }
        }

        public static async UniTask<string> Put(string url, object body, Dictionary<string, string> headers = null)
        {
            try
            {
                var bodyStr = body != null ? JsonConvert.SerializeObject(body): string.Empty;
                Debug.Log("Put Request:\nUrl:" + url);
                Debug.Log("Put Request Body:" + bodyStr);
                var request = UnityWebRequest.Put(url, bodyStr);
                request.timeout = TIMEOUT;
                request.SetRequestHeader("Content-Type", "application/json");
                if (headers != null)
                {
                    foreach (var h in headers)
                    {
                        request.SetRequestHeader(h.Key, h.Value);
                    }
                }
                await request.SendWebRequest().ToUniTask();
                return ProcessResResult(request);
            }
            catch (Exception e)
            {
                string rawMessage = e.Message;
                string parsedMessage = "Invalid Error Message"; // Mặc định nếu không trích được

                try
                {
                    Match match = Regex.Match(rawMessage, "\"message\"\\s*:\\s*\"([^\"]+)\"");
                    if (match.Success)
                    {
                        parsedMessage = match.Groups[1].Value;
                    }
                }
                catch
                {
                    //
                }
                EventController.RaiseEvent(EventList.Setting.ERROR_GIFTCODE, new EventParam { p0 = parsedMessage });
                Debug.Log(e.Message);
                return string.Empty;
            }
        }

        static string ProcessResResult(UnityWebRequest res)
        {
            if (res.result == UnityWebRequest.Result.Success)
            {
                var result = res.downloadHandler.text;
                var resultJson = SimpleJSON.JSON.Parse(result);
                if (resultJson["statusCode"] == 200)
                {
                    Debug.Log("<color=#00ff00ff>Success</color>:" + result);
                    // Debug.Log(resultJson["data"]);
                    // onDone?.Invoke(resultJson["data"].ToString());
                    return resultJson["data"].ToString();
                }
                else
                {
                    Debug.Log("<color=#ff0000ff>Failed</color>:" + result);
                    return string.Empty;
                }
            }
            else
            {
                Debug.Log("<color=#ff0000ff>Failed</color>:" + res.error);
                // onError?.Invoke();
                return string.Empty;
            }
        }
    }
}