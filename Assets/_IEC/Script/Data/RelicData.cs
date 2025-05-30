using System;
using System.Collections.Generic;
using System.Linq;
using _IEC.Script.Config;
using UnityEngine;

namespace IEC
{
    [Serializable]
    public class RelicData
    {
        public int currentRelicTokenCount;
        public int curRelicLevel = 1;
        public List<int> claimedRewardIds = new();
        public DateTime eventTimeEnd;
        public bool isAvaiableToSpawn => AvaiableToSpawn();
        public bool hasJoinEvent;
        public bool hasDoneLevel;
        public List<Reward> lastEventRewards = new();
        public int relicItemIndex;

        public bool IsEventInRange()
        {
            return eventTimeEnd > DateTime.Now;
        }

        public bool IsDoneLevel(ArtifactCollectorConfig config)
        {
            bool rs = true;
            if (curRelicLevel <= config.ArtifactLevels.Count)
            {
                var lastNodeCurLevel = config.ArtifactLevels[curRelicLevel - 1].artifactLevelRewards[^1];
                rs = (currentRelicTokenCount >= config.GetStackTokenCost(lastNodeCurLevel.id));
            }

            hasDoneLevel = rs;
            return rs;
        }

        private bool AvaiableToSpawn()
        {
            return hasJoinEvent && !hasDoneLevel;
            //return true;
        }

        public void OnResetEvent(ArtifactCollectorConfig config)
        {
            var rw = GetAllUnClaimRelicLevelReward(config);
            if (rw.Count > 0)
            {
                foreach (var keyValuePair in rw)
                {
                    lastEventRewards.Add(new(keyValuePair.Value, keyValuePair.Key));
                }
            }

            hasJoinEvent = false;
            curRelicLevel = 1;
            claimedRewardIds.Clear();

        }

        public bool CheckIfHasReward(ArtifactCollectorConfig config)
        {
            var level = curRelicLevel;
            foreach (var x in config.ArtifactLevels[level - 1].artifactLevelRewards)
            {
                if (HasRewardedLevel(x.id)) continue;
                if (config.GetStackTokenCost(x.id) > currentRelicTokenCount) continue;
                if (!claimedRewardIds.Contains(x.id)) return true;
            }

            return false;
        }
        public bool CheckIfHasLockReward(ArtifactCollectorConfig config)
        {
            var level = curRelicLevel;
            foreach (var x in config.ArtifactLevels[level - 1].artifactLevelRewards)
            {
                if (HasRewardedLevel(x.id)) continue;
                if (config.GetStackTokenCost(x.id) > currentRelicTokenCount) return true;
            }
            return false;
        }

        public Dictionary<ResourceType, int> GetAllUnClaimRelicLevelReward(ArtifactCollectorConfig config)
        {
            Dictionary<ResourceType, int> rs = new();
            rs = CalculateAndSetClaimRewardInLevel(config, curRelicLevel);
            return rs;
        }

        // private Dictionary<ResourceType, int> CalculateRewardTotalsLastLevel(ArtifactCollectorConfig config,
        //     int level
        // )
        // {
        //     Dictionary<ResourceType, int> rs = new();
        //     foreach (var x in config.ArtifactLevels[level - 1].artifactLevelRewards)
        //     {
        //         if (HasRewardedLevel(x.id)) continue;
        //         foreach (var artifactRewardData in x.artifactRewards)
        //         {
        //             if (!rs.ContainsKey(artifactRewardData.artifactRewardType))
        //             {
        //                 rs.Add(artifactRewardData.artifactRewardType, artifactRewardData.value);
        //             }
        //             else
        //             {
        //                 rs[artifactRewardData.artifactRewardType] += artifactRewardData.value;
        //             }
        //         }
        //
        //         if (!claimedRewardIds.Contains(x.id)) claimedRewardIds.Add(x.id);
        //     }
        //
        //     return rs;
        // }

        public bool AbleTolevelUp(ArtifactCollectorConfig config)
        {
            if (curRelicLevel >= config.ArtifactLevels.Count) return false;
            var curLevelConfig = config.ArtifactLevels[curRelicLevel - 1];
            var totalTokenCost = config.GetStackTokenCost(curLevelConfig.artifactLevelRewards[^1].id);
            bool hasAvailableReward = CheckClaimRewardInLevel(config, curRelicLevel);
            return currentRelicTokenCount >= totalTokenCost && !hasAvailableReward;
        }

        private Dictionary<ResourceType, int> CalculateAndSetClaimRewardInLevel(ArtifactCollectorConfig config, int level)
        {
            Dictionary<ResourceType, int> rs = new();

            foreach (var x in config.ArtifactLevels[level - 1].artifactLevelRewards)
            {
                if (HasRewardedLevel(x.id)) continue; 
                if (config.GetStackTokenCost(x.id) > currentRelicTokenCount) continue;
                foreach (var artifactRewardData in x.artifactRewards)
                {
                    if (!rs.ContainsKey(artifactRewardData.artifactRewardType))
                    {
                        rs.Add(artifactRewardData.artifactRewardType, artifactRewardData.value);
                    }
                    else
                    {
                        rs[artifactRewardData.artifactRewardType] += artifactRewardData.value;
                    }
                }

                if (!claimedRewardIds.Contains(x.id)) SetClaimReward(x.id);
            }

            return rs;
        }
        private bool CheckClaimRewardInLevel(ArtifactCollectorConfig config, int level)
        {
            foreach (var x in config.ArtifactLevels[level - 1].artifactLevelRewards)
            {
                if (HasRewardedLevel(x.id)) continue; 
                if (config.GetStackTokenCost(x.id) > currentRelicTokenCount) continue;
                return true;
            }

            return false;
        }
        public bool HasRewardedLevel(int rewardLevelId)
        {
            return claimedRewardIds.Contains(rewardLevelId);
        }

        public void SetClaimReward(int rewardLevelId)
        {
            Debug.Log($"Claiming relic level {rewardLevelId}");
            if (!claimedRewardIds.Contains(rewardLevelId)) claimedRewardIds.Add(rewardLevelId);
        }

        public bool LevelUp(int tokenToLevelUp, int maxLevel)
        {
            curRelicLevel++;
            if (curRelicLevel > maxLevel)
            {
                curRelicLevel = maxLevel;
                return false;
            }

            currentRelicTokenCount -= tokenToLevelUp;
            return true;
        }

        public void ResetTokenCount()
        {
            currentRelicTokenCount = 0;
        }

        public void AddTokenCount(int amount)
        {
            currentRelicTokenCount += amount;
        }

        public void ResetEvent(ArtifactCollectorConfig config)
        {
            OnResetEvent(config);
            ResetTokenCount();
        }
    }
}