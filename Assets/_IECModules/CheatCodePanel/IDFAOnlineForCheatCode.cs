using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IEC;
using UnityEngine;
using UnityEngine.Networking;

namespace _IECModules
{
    public static class IdfaOnlineForCheatCode
    {
        public static bool isFetchOnlineSheet;
        public static List<string> cachedListIdfa;
        public static readonly ServicesController Services = Controllers.Get<ServicesController>();

        public static void CheckContainInOnlineIDFA(string idfa,Action<bool> onComplete)
        {
            if (cachedListIdfa == null)
            {
                LoadCachedListIdfa();
            }
            if (cachedListIdfa != null && cachedListIdfa.Contains(idfa))
            {
                onComplete?.Invoke(true);
                return;
            }

            if (isFetchOnlineSheet && cachedListIdfa != null && cachedListIdfa.Contains(idfa) == false)
            {
                onComplete?.Invoke(false);
                return;
            }
            if (isFetchOnlineSheet == false)
            {
                Services.StartCoroutine(IEDownloadOnlineIDFA(text =>
                {
                    var listIdfa = GetAllIdfaFromCsv(text);
                    foreach (var onlineIdfa in listIdfa)
                    {
                        Debug.LogError(onlineIdfa);
                    }
                    isFetchOnlineSheet = true;
                    if (listIdfa.Count > 0)
                    {
                        cachedListIdfa = listIdfa;
                        SaveCachedListIdfa();
                        if (listIdfa.Contains(idfa))
                        {
                            onComplete?.Invoke(true);
                        }
                        else
                        {
                            onComplete?.Invoke(false);
                        }
                    }
                }));
            }
        }
        
        public static IEnumerator IEDownloadOnlineIDFA(Action<string> onComplete)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(GOOGLE_SHEET_IDFA);
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Fail to get Google Sheet IDFA: " + webRequest.error);
                onComplete?.Invoke("");
            }
            else
            {
                onComplete?.Invoke(webRequest.downloadHandler.text);
            }
        }
        private static List<string> GetAllIdfaFromCsv(string textSource)
        {
            Regex regex = new Regex(@"\w+\-\w+\-\w+\-\w+\-\w+");
            var res = new List<string>();
            MatchCollection matchCollection = regex.Matches(textSource);
            if (matchCollection.Count > 0)
            {
                foreach (Match match in matchCollection)
                {
                    res.Add(match.Value);
                }
            }
            return res;
        }

        private static void LoadCachedListIdfa()
        {
            var sourceText = PlayerPrefs.GetString(LOCAL_IDFA_KEY,"");
            var jsoner = new JsonDotNetModule();
            cachedListIdfa = jsoner.FromJson<List<string>>(sourceText);
        }

        private static void SaveCachedListIdfa()
        {
            if (cachedListIdfa != null)
            {
                var jsoner = new JsonDotNetModule();
                var text = jsoner.ToJson(cachedListIdfa);
                PlayerPrefs.SetString(LOCAL_IDFA_KEY,text);
            }
        }

        private const string LOCAL_IDFA_KEY = "cheatcode_idfa_list";
        private const string GOOGLE_SHEET_IDFA = @"https://docs.google.com/spreadsheets/d/e/2PACX-1vT7lRe3WZRtd-0lfL_Z7uJSpGfI5hpVIKR0U8y4itYCJUVoH_ZefViy5T9w9pndO74XD9_7JeXQzKVf/pub?gid=1696717826&single=true&output=csv";
    }
}
