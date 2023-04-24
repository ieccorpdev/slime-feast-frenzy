using System.Collections.Generic;

namespace IEC
{
    [System.Serializable]
    public class PlayerSecretSagaMap : IData
    {
        public List<int> listBonusLevelPlayed = new List<int>();
        public int unlockLevel;

        public bool IsHaveBossLevel => false;//=>Game.isHaveBossLevel;
        public bool IsHaveBonusLevel => false;//=>Game.isHaveBonusLevel;
        public int BossLevelSpacing => 5;//=>Game.bossLevelSpacing;

        public void SetUnlockLevel()
        {
            var playerData = this.PlayerData();
            if (playerData.level < unlockLevel) return;
            unlockLevel = playerData.level;
        }

        public void AddListBonusLevel(int i)
        {
            if (listBonusLevelPlayed.Contains(i)) return;
            listBonusLevelPlayed.Add(i);
        }

        public bool IsBonusLevel(int level)
        {
            return IsHaveBonusLevel && level > 3 && (level - 1) % 5 == 0;
        }

        public bool IsBossLevel(int level)
        {
            return IsHaveBossLevel && (level - 5) % BossLevelSpacing == 0;
        }
    }
}
