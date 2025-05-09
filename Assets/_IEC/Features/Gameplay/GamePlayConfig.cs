using System;
using System.Collections.Generic;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    [CreateAssetMenu(menuName = "Gameplay/GameplayConfig")]
    public class GamePlayConfig : ScriptableObject
    {

        private PlayerSecret PlayerData => Pool.Get<PlayerSecret>();
        public List<HelperConfig> helperConfigs;
        public float scaleUpHelperAdditionalSize;
        public int maxNumberDirectorUI;
        public int baseNumberCoinForRevive;
        public int reviveTimeForTimer;

        public float dangerousTime;
        public float warningTime;

        public Color dangerousTimeColor;
        public Color warningTimeColor;
        public Color safeTimeColor;

        public float ItemDrag = 2;
        public float ItemAngularDrag = 4;
        public bool IsHelperUnlocked(HelperId helperId)
        {
            return PlayerData.level >= GetUnlockLevelHelper(helperId);
        }

        private int GetUnlockLevelHelper(HelperId helperId)
        {
           return GetHelperConfig(helperId).unlockLevel;
        }
        public float GetHelperActiveTime(HelperId helperId)
        {
            return GetHelperConfig(helperId).activeTime;
        }

        public int GetStartNumber(HelperId helperId)
        {
            return GetHelperConfig(helperId).startNumber;
        }
        public HelperConfig GetHelperConfig(HelperId helperId)
        {
            return helperConfigs.Find(x => x.id == helperId);
        }

        public int GetHelperPrice(HelperId helperId)
        {
            return GetHelperConfig(helperId).price;
        }

        public int GetNumberBuyingResult(HelperId helperId)
        {
            return GetHelperConfig(helperId).numberBuyingResult;
        }

        public string GetUnlockCondition(HelperId helperId)
        {
            return "" + GetUnlockLevelHelper(helperId);
        }

        public Color GetTimeColorByRemainingTimeState(GameplayTimer.RemainingTimeState remainingTimeState)
        {
            switch (remainingTimeState)
            {
                case GameplayTimer.RemainingTimeState.Safe:
                    return safeTimeColor;
                case GameplayTimer.RemainingTimeState.Warning:
                    return warningTimeColor;
                case GameplayTimer.RemainingTimeState.Dangerous:
                    return dangerousTimeColor;
                default:
                    return safeTimeColor;
            }
        }

        public GameplayTimer.RemainingTimeState GetRemainingTimeState(float remainingTime, float maxTime)
        {
            if (remainingTime < maxTime * dangerousTime) return GameplayTimer.RemainingTimeState.Dangerous;
            if (remainingTime < maxTime * warningTime) return GameplayTimer.RemainingTimeState.Warning;
            return GameplayTimer.RemainingTimeState.Safe;
        }

        public int GetStarByRemainingTime(float time, float maxTime)
        {
            if (time <  maxTime * dangerousTime) return 1;
            if (time < maxTime * warningTime) return 2;
            return 3;
        }
    }

    [System.Serializable]
    public class HelperConfig
    {
        public HelperId id;
        public float activeTime;
        public int startNumber;
        public int unlockLevel;
        public int price;
        public int numberBuyingResult;
    }
}