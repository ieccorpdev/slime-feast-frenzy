using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

        public static async UniTask<string> UseGiftCode(string giftCode)
        {
            Debug.LogWarning("Body: " + JsonConvert.SerializeObject(Body) + ",m_body" + JsonConvert.SerializeObject(m_Body) );
            var url = string.Format(APIUrl.USE_GIFT_CODE, giftCode.Trim());
            var res =  await NetworkBase.Put(url, new
            {
                userId = Pool.Get<ServiceData>().firebasePseudoId,
                deviceId = SystemInfo.deviceUniqueIdentifier,
                gameId = Application.identifier
            });
            return string.IsNullOrWhiteSpace(res) ? res : SimpleJSON.JSON.Parse(res)["rewards"].ToString();
        }
    }
}