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
        public Button buyIAP;

        private void Awake()
        {
            Locator.Main.PopupVoyagePassIAPInfor.configBundle.OnPurchaseSuccess += OnPurchaseSuccess;
            buyIAP.onClick.AddListener(Locator.Main.PopupVoyagePassIAPInfor.Show);
            if (PlayerData.journeyData.isBuyIAP || PlayerData.level <= Config.specialObjectConfig.kingdomSpecialObjectSpawnData.levelStartSpawn)
            {
                this.gameObject.SetActive(false);
                LayoutRebuilder.ForceRebuildLayoutImmediate(root);
            }
        }

        private void OnDestroy()
        {
            Locator.Main.PopupVoyagePassIAPInfor.configBundle.OnPurchaseSuccess -= OnPurchaseSuccess;
        }

        private void OnPurchaseSuccess()
        {
            if (root)
            {
                this.gameObject.SetActive(false);
                LayoutRebuilder.ForceRebuildLayoutImmediate(root);
            }
        }
    }
}
