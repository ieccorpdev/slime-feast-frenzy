using System;
using System.Collections.Generic;
using System.Linq;
using _IEC.Script.Config;
using IEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Script.Data.UI.Popup.RelicHunt
{
    public class VitrualProgressBar : MonoBehaviour
    {
        [SerializeField] List<LevelProgressNode> levelProgressNodes = new List<LevelProgressNode>();


        [SerializeField] private Image fillImage;
        [Header("Relic Config")] public ArtifactCollectorConfig config;

        private void Start()
        {
        }

        private void UnlockLevelNode(int index)
        {
            for (int i = 0; i < levelProgressNodes.Count; i++)
            {
                levelProgressNodes[i].SetLock(i > index);
            }
        }
        [Button]
        public void SetVitrualProgress(int currentToken, int currentLevelIndex)
        {
            if (currentLevelIndex >= config.ArtifactLevels.Count)
            {
                OnUnlockAll();
                return;
            }

            if (levelProgressNodes.Count != config.ArtifactLevels[currentLevelIndex].artifactLevelRewards.Count)
            {
                Debug.LogError("Level reward count is wrong");
                return;
            }

            var relicReward = config.ArtifactLevels[currentLevelIndex].artifactLevelRewards;
            var aboveNode = relicReward.FirstOrDefault(x => x.tokenCost > currentToken);
            if (aboveNode == null)
            {
                OnUnlockAll();
                return;
            }

            var aboveNodeIndex = relicReward.IndexOf(aboveNode);
            if (aboveNodeIndex >= 1)
            {
                float range = relicReward[aboveNodeIndex].tokenCost - relicReward[aboveNodeIndex - 1].tokenCost;
                float fillRange = currentToken - relicReward[aboveNodeIndex - 1].tokenCost;
                
                float fillAmountSum = levelProgressNodes[aboveNodeIndex - 1].vitrualPercent;
                fillAmountSum += (levelProgressNodes[aboveNodeIndex].vitrualPercent - levelProgressNodes[aboveNodeIndex - 1].vitrualPercent) * fillRange / range;
                
                UnlockLevelNode(aboveNodeIndex - 1);
                fillImage.fillAmount = fillAmountSum;
            }
            else
            {
                float range = relicReward[aboveNodeIndex].tokenCost;
                float fillRange = currentToken;
                float fillAmountSum = levelProgressNodes[aboveNodeIndex].vitrualPercent * fillRange / range;
                UnlockLevelNode(aboveNodeIndex - 1);
                fillImage.fillAmount = fillAmountSum;
            }
        }

        private void OnUnlockAll()
        {
            fillImage.fillAmount = 1;
            UnlockLevelNode(levelProgressNodes.Count);
        }
    }
}