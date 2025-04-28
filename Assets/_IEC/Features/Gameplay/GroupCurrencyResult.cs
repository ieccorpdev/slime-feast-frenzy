
using System;
using IEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class GroupCurrencyResult : _BaseBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtNumberHeart;
        [SerializeField] private TextMeshProUGUI txtNumberStar;
        [SerializeField] private TextMeshProUGUI txtNumberSpecialObject;
        [SerializeField] private TextMeshProUGUI txtNumberCoin;
        [SerializeField] private Image imgIconSpecialObject;
        public void UpdateUI()
        {
            txtNumberHeart.text = 1.ToString();
            UpdateStarDisplay();
            UpdateGoldDisplay();
            UpdateSpecialItemDisplay();
           //var remainingTime = UI.ScreenInGame.GetGameplayTimer().GetRemainingTime();
           //txtNumberStar.text = Config.gamePlayConfig.GetStarByRemainingTime(remainingTime, UI.ScreenInGame.GetGameplayTimer().GetMaxTime()).ToString();
           //txtNumberCoin.text = GamePlay.gamePlayData.GetAchievementItemCount(LevelAchievementData.ID_COIN).ToString();
           //txtNumberSpecialObject.text = GamePlay.gamePlayData.GetAchievementItemCount(ProgressRewardHelper.GetCurrentSpecialObjectIdInString()).ToString();
           //imgIconSpecialObject.sprite = ProgressRewardHelper.GetCurrentSpecialObjectSprite();
           ////imgIconSpecialObject.rectTransform.ForceToWidth(imgIconSpecialObject.sprite,178);
           //
           //if (!Game.enableTreasureHunt || PlayerData.level < Config.treasureHuntProgressConfig.unlockLevel || TreasureHuntController.treasureHuntData.currentProgressIndex >= Config.treasureHuntProgressConfig.rewardProgressModels.Count)
           //{
           //    imgIconSpecialObject.gameObject.SetActive(false);
           //    txtNumberSpecialObject.gameObject.SetActive(false);
           //}
           //else
           //{
           //    imgIconSpecialObject.gameObject.SetActive(true);
           //    txtNumberSpecialObject.gameObject.SetActive(true);
           //}
        }

        void UpdateStarDisplay()
        {
            int starLost = 0;
            switch (Locator.Game.ScreenInGame.GetGameplayTimer().CurrentRemainingTimeState)
            {
                case GameplayTimer.RemainingTimeState.None:
                    break;
                case GameplayTimer.RemainingTimeState.Safe:
                    starLost = 3;
                    break;
                case GameplayTimer.RemainingTimeState.Warning:
                    starLost = 2;
                    break;
                case GameplayTimer.RemainingTimeState.Dangerous:
                    starLost = 1;
                    break;
                default:
                    starLost = 0;
                    break;
            }
            
            txtNumberStar.text = starLost.ToString();
        }

        void UpdateGoldDisplay()
        {
            txtNumberCoin.text = GamePlay.levelController.GoldCollected.ToString();
        }

        void UpdateSpecialItemDisplay()
        {
            txtNumberSpecialObject.text = GamePlay.levelController.SpecialProgressRewardObjCollected.ToString();
            imgIconSpecialObject.sprite = Config.specialObjectConfig.GetSpecialItemDefinition(PlayerData.currentSpecialItemIndex).GetIcon();
        }
    }
}