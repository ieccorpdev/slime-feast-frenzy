using System;
namespace IEC
{
    public interface IDailyMissionService
    {
        void SetConfig(DailyMissionConfig[] configs);

        void AddValue(string keyName, int value);

        void SetValue(string keyName, int value);

        void SetValueIfHigher(string keyName, int value);

        int GetValue(string keyName);

        int GetConditionValue(string keyName);

        bool IsProgressDone(string keyName);

        bool IsClaimedReward(string keyName);

        bool IsRewardUnClaim(string keyName);

        void ClaimReward(string keyName);

        int GetReward(string keyName);

        int GetSkipCost(string keyName);

        bool IsCanSkip(string keyName);

        uint GetSecondsRemainTime();

        void SaveData();

    }

    public class MissionData
    {
        public string KeyName;
        public int Value;
        public bool IsClaimed;
        public bool IsDone;

        public MissionData(string keyName)
        {
            KeyName = keyName;
            Value = 0;
            IsClaimed = false;
            IsDone = false;
        }
    }
}

