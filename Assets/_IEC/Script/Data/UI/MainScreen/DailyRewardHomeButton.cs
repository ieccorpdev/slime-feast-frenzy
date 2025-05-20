using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class DailyRewardHomeButton : _BaseBehaviour
    {
        public GameObject titleGroup;

        public GameObject claimGroup;

        public void UpdateUI()
        {
            titleGroup.SetActive(!Locator.Main.PopupDailyReward.CanClaimReward());
            claimGroup.SetActive(Locator.Main.PopupDailyReward.CanClaimReward());
        }
    }
}
