using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using _IECModules;
using Cysharp.Threading.Tasks;
using SimpleJSON;
using UnityEngine;

namespace IEC
{
    public class GiftCodeController : _BaseBehaviour, IEventListener, IEventAwake, IEventStart
    {
        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourStart()
        {
            UseGiftCode("WC64A90W").Forget();
        }

        public void BehaviourAwake()
        {
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
        }

        #region Customize
        
        private void ClaimIapBundle(string bundleId)
        {
            //TODO: Claim iap bundle id
        }

        private void ProcessOtherData(JSONNode rewards)
        {
            //TODO: Process other data
        }

        #endregion

        #region Base
        
        private async UniTaskVoid UseGiftCode(string giftCode)
        {
            giftCode = giftCode.Trim();
            var data = await GiftCodeModule.UseGiftCode(giftCode);
            // Debug.LogError(data);
            if (string.IsNullOrEmpty(data)) return;
            var json = JSON.Parse(data);
            // Debug.LogError(json);
            if (json.HasKey("iapBundles"))
            {
                ProcessIapBundle(json["iapBundles"]);
            }

            if (json.HasKey("isNoAds"))
            {
                Pool.Get<ServiceData>().isNoAds = json["isNoAds"].AsBool;
            }

            if (json.HasKey("playerData"))
            {
                ProcessPlayerData(json["playerData"]);
            }
                
            //TODO: Code for other items, resources, datas
            ProcessOtherData(json);
        }

        private void ProcessIapBundle(JSONNode iapBundles)
        {
            var listBundle = iapBundles.IsArray
                ? iapBundles.AsStringArray.ToList()
                : new List<string> { iapBundles.Value };
            foreach (var bundle in listBundle)
            {
                ClaimIapBundle(bundle);
            }
        }

        private void ProcessPlayerData(JSONNode playerData)
        {
            Debug.LogError(playerData.ToString());
            var keys = playerData.Keys;

            var playerDataType = typeof(PlayerSecret);
            foreach (var key in keys)
            {
                var field = playerDataType.GetField(key);
                if (field != null)
                {
                    Debug.LogError(field);
                    try
                    {
                        field.SetValue(PlayerData, Parse(field.FieldType, playerData[key].Value));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                var property = playerDataType.GetProperty(key);
                if (property != null)
                {
                    Debug.LogError("property:" + key);
                    try
                    {
                        property.SetValue(PlayerData, Parse(property.PropertyType,
                            playerData[key].Value));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        private object Parse(Type t, string s)
            => TypeDescriptor.GetConverter(t).ConvertFromInvariantString(s);

        #endregion
    }
}