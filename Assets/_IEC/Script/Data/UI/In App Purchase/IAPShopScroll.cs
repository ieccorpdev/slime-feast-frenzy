using System;
using System.Collections.Generic;
using NaughtyAttributes;

namespace IEC
{
    public class IAPShopScroll : _BaseBehaviour
    {
        public List<PackUI> listIAPPackage;
        public string idButton;

        public void OnEnable()
        {
            CheckListShowing();
        }

        public void CheckListShowing()
        {
            foreach (var iapPack in listIAPPackage)
            {
                iapPack.gameObject.SetActive(true);
            }

            if (PlayerData.isBoughtStarterPack)
            {
                var starterPack =
                    listIAPPackage.Find(x => x.GetConfig.bundleId == "com.shark.frenzy.force.starter.pack");
                if (starterPack != null)
                {
                    starterPack.gameObject.SetActive(false);
                }
            }
            if (PlayerData.isNoAds)
            {
                var starterPack = listIAPPackage.FindAll(x => x.GetConfig.isHaveNoAds);
                if (starterPack != null)
                {
                    foreach (var packsNoAds in starterPack)
                    {
                        packsNoAds.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}