using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Script.Data.UI.Popup.RelicHunt
{
    public class LevelProgressNode : _BaseBehaviour
    {
        public GameObject lockGO;
        public GameObject unlockGO;
        public GameObject claimableGO;
        public Button claimBtn;
        public float vitrualPercent;
        public Action OnClaimReward;
        public bool hasClaimReward;
        private List<Reward> _rewards = new();

        [ValueDropdown("GetClaimRewardOptions")]
        public string animReward;

        private static string[] GetClaimRewardOptions()
        {
            return typeof(ClaimBonusRewardSkin)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                .Select(f => f.GetRawConstantValue() as string)
                .ToArray();
        }

        private void OnEnable()
        {
            claimBtn.onClick.RemoveAllListeners();
            claimBtn.onClick.AddListener(() =>
            {
                if (_rewards.Count == 0) return;
                hasClaimReward = true;
                ShowReward();
                OnClaimReward?.Invoke();
                SetState(RewardState.Claimed);
                Debug.Log($"OnClaimReward?.Invoke() {animReward}");
            });
        }

        private void ShowReward()
        {
            //Locator.Main.ClaimBonusRewardPanel.SetupTempTitle();
            if (!PlayerData.relicData.CheckIfHasLockReward(Config.relicConfig) && !PlayerData.relicData.CheckIfHasReward(Config.relicConfig))
            {
                PlayerData.trophyData.CollectTrophy(TrophyType.RelicHunt);
            }
            Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(_rewards, () =>
                {
                    Locator.Main.PopupRelicHunt.Show();
                },
                true);
            Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel(animReward);
            Locator.Main.PopupRelicHunt.Hide();
        }

        public void AddReward(Reward reward)
        {
            _rewards.Add(reward);
        }

        public void ClearReward()
        {
            _rewards.Clear();
            hasClaimReward = false;
        }

        public void SetState(RewardState state)
        {
            lockGO.SetActive(state == RewardState.Lock);
            unlockGO.SetActive(state == RewardState.Claimed);
            claimableGO.SetActive(state == RewardState.Claimable);
        }

        public enum RewardState
        {
            Lock,
            Claimable,
            Claimed,
        }
    }
}