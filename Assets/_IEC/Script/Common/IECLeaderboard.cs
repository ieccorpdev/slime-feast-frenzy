using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace IEC
{
    public class APIUrl
    {
        private const string BASE = "https://leaderboard.haleinteractive.vn/api";
        public const string LOGIN_CUSTOM = BASE + "/auth/login-custom";
        public const string LB_GLOBAL = BASE + "/leaderboards/global";
        public const string LB_COUNTRY = BASE + "/leaderboards/country";
        public const string USER_STATS = BASE + "/leaderboards/user-stats";
        public const string SUBMIT_SCORE = BASE + "/leaderboards/submit";
        public const string LB_GLOBAL_AROUND = BASE + "/leaderboards/global/around-me";
        public const string LB_COUNTRY_AROUND = BASE + "/leaderboards/country/around-me";
        public const string PROFILE = BASE + "/auth/profile";
        public const string LOCATION_INFO = BASE + "/location-info";
        
    }
    public class IECLeaderboard
    {

        private static string s_AccessToken = "";

        private static bool s_IsLoggedIn = false;
        public static string CountryCode { get; private set; }
        // Start is called before the first frame update

        public static void Login (string gameCode, string platformId = null, Action<string> onDone = null, Action onError = null)
        {
            if (s_IsLoggedIn) return;
            if (string.IsNullOrWhiteSpace(platformId))
            {
                platformId = SystemInfo.deviceUniqueIdentifier;
            }
            // Debug.Log(Time.realtimeSinceStartup);
            Post(APIUrl.LOGIN_CUSTOM, new
            {
                playerId = platformId,
                gameCode = gameCode,
                platform = Application.platform == RuntimePlatform.Android ? "android" : "ios",
            }, result =>
            {
                // Debug.Log(Time.realtimeSinceStartup);
                // Debug.Log(result);
                var data = SimpleJSON.JSON.Parse(result);
                s_IsLoggedIn = true;
                s_AccessToken = data["accessToken"];
                Debug.Log(s_AccessToken);
                CountryCode = data["userInfo"]["country"];
                // Debug.Log(CountryCode);
                onDone?.Invoke(data);
            }, onError, false);
        }

        public static void GetProfile(Action<string> onDone = null, Action onError = null)
        {
            Get(APIUrl.PROFILE, null, onDone, onError);
        }

        public static void UpdateProfile(Action<string> onDone = null, Action onError = null)
        {
            Put(APIUrl.PROFILE, null, onDone, onError);
        }

        public static void SubmitScore(string key, int val)
        {
            Post(APIUrl.SUBMIT_SCORE, new
            {
                keyName = key,
                value = val,
                country = CountryCode
            });
        }

        public static void GetGlobalLB(string key, int page, int pageSize = 10, Action<string> onDone = null, Action onError = null)
        {
            Get(APIUrl.LB_GLOBAL, new
            {
                keyName = key,
                country = CountryCode,
                page = page,
                pageSize = pageSize
            }, onDone, onError);
        }

        public static void GetCountryLB(string key, int page, int pageSize = 10, Action<string> onDone = null,
            Action onError = null)
        {
            Get(APIUrl.LB_COUNTRY, new
            {
                keyName = key,
                country = CountryCode,
                page = page,
                pageSize = pageSize
            }, onDone, onError);
        }
        
        public static void GetGlobalAroundMe(string key, int pageSize = 10, Action<string> onDone = null, Action onError = null)
        {
            Get(APIUrl.LB_GLOBAL_AROUND, new
            {
                keyName = key,
                country = CountryCode,
                size = pageSize
            }, onDone, onError);
        }
        
        public static void GetCountryAroundMe(string key, int pageSize = 10, Action<string> onDone = null, Action onError = null)
        {
            Get(APIUrl.LB_COUNTRY_AROUND, new
            {
                keyName = key,
                country = CountryCode,
                size = pageSize
            }, onDone, onError);
        }
        
        static async void Post(string url, object body, Action<string> onDone = null, Action onError = null, bool isNeedLogin = true)
        {
            if(isNeedLogin && !IsLogin()) return;
            try
            {
                var bodyStr = JsonConvert.SerializeObject(body);
                Debug.Log("Post Request:\nUrl:" + url + "\nBody:" + bodyStr);
                var request = UnityWebRequest.Post(url, bodyStr);
                var bodyRaw = Encoding.UTF8.GetBytes(bodyStr);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("x-access-token", s_AccessToken);
                Debug.Log(request.GetRequestHeader("x-access-token"));
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

        static async void Get(string url, object body, Action<string> onDone = null, Action onError = null, bool isNeedLogin = true)
        {
            if (isNeedLogin && !IsLogin()) return;
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

                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("x-access-token", s_AccessToken);
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

        static async void Put(string url, object body, Action<string> onDone = null, Action onError = null, bool isNeedLogin = true)
        {
            if (isNeedLogin && !IsLogin()) return;
            try
            {
                var bodyStr = body != null ? JsonConvert.SerializeObject(body): string.Empty;
                Debug.Log("Put Request:\nUrl:" + url);
                var request = UnityWebRequest.Put(url, bodyStr);

                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("x-access-token", s_AccessToken);
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
                Debug.Log("<color=#ff0000ff>Post Failed</color>:" + res.webRequest.error);
                onError?.Invoke();
            }
        }

        static bool IsLogin()
        {
            if (s_IsLoggedIn) return true;
            Debug.Log("<color=#ff0000ff>Post Failed</color>: Not login");
            return false;

        }
    }
}