using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace IEC.DailyTask
{
    [CreateAssetMenu(menuName = "Game/DailyTasksConfig")]
    public class DailyTasksConfig : BaseConfig
    {
        public string inputDailyTaskUrlConfig;
        public enum DailyTaskType
        {
            WatchAds, Deploy, UpgradeBarrack, UpgradeZombie, UpgradeIdleMoney, UpgradeTalent, Kill, DestroyGate, DestroyHQ,
            Complete, WatchAdsInfinity
        }
        public enum DailyTaskDifficult
        {
            Easy, Normal, Hard, WatchAds, FinalTask, CulmulativeTask, None
        }
        public List<DailyTaskTypeConfig> taskTypesConfig;
        public List<DailyTaskDiffConfig> taskDiffConfig;
        public List<DailyTaskConfig> tasksConfig;

        [Button]
        private void LoadDataConfig()
        {
            LoadGoogleSheetConfig(inputDailyTaskUrlConfig, tasksConfig);

        }

        private void LoadGoogleSheetConfig(string url, IList list)
        {
            GoogleSheetUtility.ParseGoogleDocToScriptableObject(url, this, list);
        }
        public DailyTaskConfig GetTaskByType(DailyTaskType type)
        {
            return tasksConfig.Find(x => x.taskType == type);
        }
        public List<DailyTaskConfig> GetTaskByDiff(DailyTaskDifficult diff)
        {
            return tasksConfig.Where(x => x.taskDiff == diff).Cast<DailyTaskConfig>().ToList();
        }
        public DailyTaskConfig GetTaskByID(string id)
        {
            return tasksConfig.Find(x => x.id.Equals(id));
        }

        public DailyTaskTypeConfig GetTaskDescByTaskType(DailyTaskType type)
        {
            return taskTypesConfig.Find(x => x.taskType == type);
        }
    }

    [System.Serializable]
    public class DailyTaskTypeConfig
    {
        public DailyTasksConfig.DailyTaskType taskType;
        public string taskText;
        public string taskCountText;
    }

    [System.Serializable]
    public class DailyTaskDiffConfig
    {
        public DailyTasksConfig.DailyTaskDifficult taskDiff;
        public int missionPerDay;
    }

    [System.Serializable]
    public class DailyTaskConfig
    {
        public string id;
        public DailyTasksConfig.DailyTaskType taskType;
        public DailyTasksConfig.DailyTaskDifficult taskDiff;
        public int count;
        public int rewardMoney;
        public int rewardGem;

        public int SaveSlot { get; set; }
        public DailyTaskConfig Clone(int saveSlot)
        {
            return new DailyTaskConfig()
            {
                id = this.id,
                taskType = this.taskType,
                taskDiff = this.taskDiff,
                count = this.count,
                rewardGem = this.rewardGem,
                rewardMoney = this.rewardMoney,
                SaveSlot = saveSlot
            };
        }
    }
}