using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [System.Serializable]
    public class PlayerSecretSagaMap
    {
        public List<int> listBonusLevelPlayed = new List<int>();
        public int unlockLevel;

        public void SetUnlockLevel()
        {
            if (Pool.Get<PlayerSecret>().level < unlockLevel) return;
            unlockLevel = Pool.Get<PlayerSecret>().level;
        }

        public void AddListBonusLevel(int i)
        {
            if (listBonusLevelPlayed.Contains(i)) return;
            listBonusLevelPlayed.Add(i);
        }
    }
}
