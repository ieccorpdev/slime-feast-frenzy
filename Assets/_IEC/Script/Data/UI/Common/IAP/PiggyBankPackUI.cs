using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class PiggyBankPackUI : PackUI
    {
        protected override void Awake()
        {
            base.Awake();
            OnPurchaseSuccess += () =>
            {
                PlayerData.CollectPiggyBankGold();
            };
        }
    }
}
