using System;
using System.Collections.Generic;
using System.Linq;
using _IEC.Script.Config;
using DG.Tweening;
using IEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Script.Data.UI.Popup.RelicHunt
{
    public class RelicProgressBar : MonoBehaviour
    {
        public List<LevelProgressNode> levelProgressNodes = new List<LevelProgressNode>();


        [SerializeField] private Image fillImage;
        [Header("Relic Config")] public ArtifactCollectorConfig config;

        
        public void OnNewLevel()
        {
            foreach (var levelProgressNode in levelProgressNodes)
            {
                levelProgressNode.ClearReward();
                levelProgressNode.SetState(LevelProgressNode.RewardState.Lock);
            }
        }


        [Button]
        public void SetVitrualProgress(int currentToken, int currentLevelIndex, bool isMax = false)
        {
            if (isMax)
            {
                fillImage.fillAmount = 1;
                return;
            }
            if (currentLevelIndex >= config.ArtifactLevels.Count)
            {
                return;
            }

            if (levelProgressNodes.Count != config.ArtifactLevels[currentLevelIndex].artifactLevelRewards.Count)
            {
                Debug.LogError("Level reward count is wrong");
                return;
            }

            var relicReward = config.ArtifactLevels[currentLevelIndex].artifactLevelRewards;
            var aboveNode = relicReward.FirstOrDefault(x => config.GetStackTokenCost(x.id) > currentToken);
            if (aboveNode == null)
            {
                return;
            }

            var aboveNodeIndex = relicReward.IndexOf(aboveNode);
            float fillAmountSum = 0f;
            if (aboveNodeIndex >= 1)
            {
                float range = config.GetStackTokenCost(relicReward[aboveNodeIndex].id) -  config.GetStackTokenCost(relicReward[aboveNodeIndex-1].id);
                float fillRange = currentToken - config.GetStackTokenCost(relicReward[aboveNodeIndex - 1].id);

                fillAmountSum = levelProgressNodes[aboveNodeIndex - 1].vitrualPercent;
                fillAmountSum +=
                    (levelProgressNodes[aboveNodeIndex].vitrualPercent -
                     levelProgressNodes[aboveNodeIndex - 1].vitrualPercent) * fillRange / range;

                fillImage.DOFillAmount(fillAmountSum, 0.5f);

            }
            else
            {
                float range =  config.GetStackTokenCost(relicReward[aboveNodeIndex].id);
                float fillRange = currentToken;
                fillAmountSum = levelProgressNodes[aboveNodeIndex].vitrualPercent * fillRange / range;
                fillImage.fillAmount = fillAmountSum;
            }
        }

      
    }
}