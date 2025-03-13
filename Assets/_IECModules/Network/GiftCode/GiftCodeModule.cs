using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace _IECModules
{
    public class GiftCodeModule
    {
        // Start is called before the first frame update
        private class APIUrl
        {
            public static string USE_GIFT_CODE = NetworkBase.BASE + "/gift-codes/{0}/use";
        }

        private static object m_Body;

        private static object Body
        {
            get
            {
                return m_Body ??= new
                {
                    userId = Pool.Get<ServiceData>().firebasePseudoId,
                    deviceId = SystemInfo.deviceUniqueIdentifier,
                    gameId = Application.identifier
                };
            }
        }

        public static void UseGiftCode(string giftCode, Action<string> onDone, Action onFailed = null)
        {
            var url = string.Format(APIUrl.USE_GIFT_CODE, giftCode);
            NetworkBase.Put(url, Body, s =>
            {
                var rewards = SimpleJSON.JSON.Parse(s)["rewards"];
                onDone?.Invoke(rewards.ToString());
            }, onFailed);
        }
    }
}