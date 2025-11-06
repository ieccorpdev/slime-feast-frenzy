using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using _IECModules;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
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
            //UseGiftCode("WC64A90W").Forget();
        }

        public void BehaviourAwake()
        {
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.Setting.REQUEST_GIFTCODE:
                    string giftCode = (string)param.p0;
                    UseGiftCode(giftCode).Forget();
                    break;
            }
        }

        #region Customize
        
        private void ClaimIapBundle(string bundleId)
        {
            //TODO: Claim iap bundle id
            var pack = Pool.Get<GameConfig>().listAllIapPackConfig.Find(pack => pack.bundleId == bundleId);
            if (pack != null)
            {
                List<Reward> listPackReward = new List<Reward>();
                foreach (var rsc in pack.resources)
                {
                    listPackReward.Add(new Reward(rsc.value,rsc.type) );
                }
                //maybe change to iap thread ? 
                if (pack.isHaveNoAds)
                {
                    Services.ActiveNoAds(true);
                    Services.SetActiveBanner(AdsEnum.BANNER_BOTTOM, false); //Disable banner Ads
                    Controllers.Get<AdsController>().UpdateWhiteBannerSize();
                }

                //For Voyage Pass
                if (pack.bundleId.Contains("voyage"))
                {
                    PlayerData.journeyData.isBuyIAP = true;
                }
                
                Locator.Main.PopupGiftCode.ReceiveGiftCode(listPackReward);
            }
        }

        private void ProcessOtherData(JSONNode jsonString)
        {
            List<Reward> rewardWrapper = new List<Reward>();
            foreach (var reward in jsonString)
            {
                if (Enum.TryParse<ResourceType>(reward.Key, out var result))
                {
                    rewardWrapper.Add(new Reward(reward.Value,result));
                }
            }
            if ( rewardWrapper.Count > 0)
            {
                //foreach (Reward reward in rewardWrapper.rewards)
                //{
                //    Debug.Log($"Reward: {reward.typeReward}, Amount: {reward.rewardAmount}");
                //}
                Locator.Main.PopupGiftCode.ReceiveGiftCode(rewardWrapper);
            }
        }

        #endregion
        
        #region Base
        
        private async UniTaskVoid UseGiftCode(string giftCode)
        {
            Debug.LogError("UseGiftCode" + giftCode);
            giftCode = giftCode.Trim();
            var data = await GiftCodeModule.UseGiftCode(giftCode);
            Locator.Service.PopupLoadingPurchaseIap.Hide();
            Debug.LogWarning("UseGiftCode data: "+data);
            if (string.IsNullOrEmpty(data)) return;
            var json = JSON.Parse(data);
            Debug.LogWarning("UseGiftCode data: "+data);
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