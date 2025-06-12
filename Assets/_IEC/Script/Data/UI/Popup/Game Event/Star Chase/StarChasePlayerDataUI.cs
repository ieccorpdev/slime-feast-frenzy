using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class StarChasePlayerDataUI : GameEventPlayerDataUI
    {
        public RewardInfoGroup rewardInfoUp;
        public RewardInfoGroup rewardInfoDown;

        protected override void SetRewardInfoDown()
        {
            base.SetRewardInfoDown();
            rewardInfo = rewardInfoDown;
        }

        protected override void SetRewardInfoUp()
        {
            base.SetRewardInfoUp();
            rewardInfo = rewardInfoUp;
        }
    }
}
