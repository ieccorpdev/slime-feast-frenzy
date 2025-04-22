using System;
using _IEC.Features.Booster;
using _IEC.Features.Gameplay;
using IEC;
using UnityEngine;

namespace IEC
{
    public static class EnumHelper
    {
        public static HelperId ConvertToHelperId(RewardTypes rewardType)
        {
            return (HelperId)Enum.Parse(typeof(HelperId), rewardType.ToString());
        }

        public static BoosterId ConvertToBoosterId(RewardTypes rewardTypes)
        {
            return (BoosterId)Enum.Parse(typeof(BoosterId), rewardTypes.ToString());
        }
    }
}