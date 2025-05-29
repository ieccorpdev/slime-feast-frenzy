using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtils;

namespace IEC
{
    public class BundlePackVoyagePass : _BaseBehaviour
    {
        public RectTransform root;
        public BundlePackUI bundlePackUI;

        private void Awake()
        {
            bundlePackUI.OnPurchaseSuccess += OnPurchaseSuccess;
            if (PlayerData.journeyData.isBuyIAP || PlayerData.level <= Config.specialObjectConfig.kingdomSpecialObjectSpawnData.levelStartSpawn)
            {
                this.gameObject.SetActive(false);
                LayoutRebuilder.ForceRebuildLayoutImmediate(root);
            }
        }

        private void OnPurchaseSuccess()
        {
            if (root)
            {
                PlayerData.journeyData.isBuyIAP = true;
                this.gameObject.SetActive(false);
                LayoutRebuilder.ForceRebuildLayoutImmediate(root);
            }
        }
    }
}
