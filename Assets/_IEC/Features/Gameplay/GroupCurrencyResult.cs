
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
        [SerializeField] private TextMeshProUGUI txtNumberRelicObject;
        [SerializeField] private TextMeshProUGUI txtNumberCoin;
        [SerializeField] private Image imgIconSpecialObject;
        [SerializeField] private Image imgIconRelicObject;
        [SerializeField] private GridLayoutGroup parent;
        public void UpdateUI()
        {
            txtNumberHeart.text = 1.ToString();
            UpdateStarDisplay();
            UpdateGoldDisplay();
            UpdateSpecialItemDisplay();
            UpdateRelicItemDisplay();
            SetSizeGroup();
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

        public void SetSizeGroup()
        {
            int childCount = Utilities.CountActiveImmediateChildren(parent.transform);

            int result = 2;
            switch (childCount)
            {
                case 5:
                case 6:
                    result = 3;
                    break;
                case >=7:
                    result = 4;
                    break;
            }
            parent.constraintCount = result;
        }
        void UpdateGoldDisplay()
        {
            int gold = GamePlay.levelController.GoldCollected;
            txtNumberCoin.text =gold.ToString();
            txtNumberCoin.transform.parent.gameObject.SetActive(gold > 0);
        }

        void UpdateSpecialItemDisplay()
        {
            int specialCount = GamePlay.levelController.SpecialProgressRewardObjCollected;
            txtNumberSpecialObject.text = specialCount.ToString();
            imgIconSpecialObject.sprite = Config.specialObjectConfig.GetSpecialItemDefinition(PlayerData.currentSpecialItemIndex).GetIcon();
            txtNumberSpecialObject.transform.parent.gameObject.SetActive(specialCount >0);
        }
        void UpdateRelicItemDisplay()
        {
             txtNumberRelicObject.text = GamePlay.levelController.RelicObjCollected.ToString();
             imgIconRelicObject.gameObject.SetActive(PlayerData.relicData.isAvaiableToSpawn && PlayerData.level >= Config.relicConfig.levelUnlockFeature);
        }
    }
}