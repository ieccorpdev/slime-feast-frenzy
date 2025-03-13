using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IECModules;
using SimpleJSON;
using UnityEngine;

namespace IEC
{
    public class GiftCodeController : MonoBehaviour
    {
        private void Start()
        {
            //Test
            UseGiftCode("WD8F9D1W");
            //
        }

        private void UseGiftCode(string giftCode)
        {
            GiftCodeModule.UseGiftCode(giftCode, rewards =>
            {
                var json = JSON.Parse(rewards);
                if (json.HasKey("iapBundles"))
                {
                    var iapBundles = json["iapBundles"];
                    var listBundle = iapBundles.IsArray 
                        ? iapBundles.AsStringArray.ToList() 
                        : new List<string>{iapBundles.Value};
                    foreach (var bundle in listBundle)
                    {
                        //TODO: Claim bundle
                    }
                }

                if (json.HasKey("isNoAds"))
                {
                    Pool.Get<ServiceData>().isNoAds = json["isNoAds"].AsBool;
                }
                
                //TODO: Code for other items, resources
            });
        }
    }
}