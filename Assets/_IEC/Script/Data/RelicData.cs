using System;
using System.Collections.Generic;
using _IEC.Script.Config;

namespace IEC
{
    [Serializable]
    public class RelicData
    {
        public int relicItemIndex = 0;
        public int currentRelicTokenCount;
        public int curRelicLevel = 1;
        public List<int> claimedRewardIds = new();
        public DateTime lastTimeResetEvent;
        public DateTime eventTimeEnd;
        public bool isAvaiableToSpawn => AvaiableToSpawn();
        public bool hasJoinEvent;
        public bool hasDoneLevel;

        private bool AvaiableToSpawn()
        {
            return hasJoinEvent && !hasDoneLevel;
        }
        public void SaveTimeResetEvent()
        {
            lastTimeResetEvent = DateTime.Now;
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
        public Dictionary<ResourceType, int> GetAllUnClaimRelicLevelReward(ArtifactCollectorConfig config,
            bool isMaxLevel = false)
        {
            Dictionary<ResourceType, int> rs = new();
            rs = CalculateRewardInLevel(config, curRelicLevel);
            return rs;
        }

        private Dictionary<ResourceType, int> CalculateRewardTotalsLastLevel(ArtifactCollectorConfig config,
            int level
        )
        {
            Dictionary<ResourceType, int> rs = new();
            foreach (var x in config.ArtifactLevels[level - 1].artifactLevelRewards)
            {
                if (HasRewardedLevel(x.id)) continue;
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

                if (!claimedRewardIds.Contains(x.id)) claimedRewardIds.Add(x.id);
            }

            return rs;
        }

        public bool AbleTolevelUp(ArtifactCollectorConfig config)
        {
            if(curRelicLevel >= config.ArtifactLevels.Count) return false;
            var curLevelConfig = config.ArtifactLevels[curRelicLevel - 1];
            var totalTokenCost = config.GetStackTokenCost(curLevelConfig.artifactLevelRewards[^1].id);
            return currentRelicTokenCount >= totalTokenCost;
        }
        private Dictionary<ResourceType, int> CalculateRewardInLevel(ArtifactCollectorConfig config, int level)
        {
            Dictionary<ResourceType, int> rs = new();

            foreach (var x in config.ArtifactLevels[level - 1].artifactLevelRewards)
            {
                if (HasRewardedLevel(x.id)) continue;
                if (config.GetStackTokenCost(x.id)> currentRelicTokenCount) continue;
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

                if (!claimedRewardIds.Contains(x.id)) claimedRewardIds.Add(x.id);
            }

            return rs;
        }

        public bool HasRewardedLevel(int rewardLevelId)
        {
            return claimedRewardIds.Contains(rewardLevelId);
        }

        public void SetClaimReward(int rewardLevelId)
        {
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

        public void CheckResetTimeEvent(int daysToReset)
        {
            if ((DateTime.Now - lastTimeResetEvent).TotalDays >= daysToReset)
            {
                SaveTimeResetEvent();
                ResetTokenCount();
            }
        }
    }
}