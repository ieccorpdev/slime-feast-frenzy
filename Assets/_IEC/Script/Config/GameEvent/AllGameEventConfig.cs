using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/AllGameEventConfig")]
    public class AllGameEventConfig : ScriptableObject
    {
        public List<GameEventConfig> listAllGameEventConfig = new();
        public List<string> listRandomNames = new();
        public ChompOffProgressConfig chompOffProgressConfig;
        public int diffLevelShow = 10;
        public GameEventConfig GetGameEventConfig(EventType eventType)
        {
            return listAllGameEventConfig.Find(config => config.eventType == eventType);
        }
        public int GetLevelUnlockEvent(EventType type)
        {
            var data = listAllGameEventConfig.Find(x => x.eventType == type);
            if (data)
            {
                return data.levelUnlock;
            }
            return 999;
        }
        public List<string> GenerateAllNames(int num, string nameToIgnore = "")
        {
            List<string> result = new List<string>();

            while (result.Count < num)
            {
                string name = GenerateNewName();

                if (!result.Contains(name) && !nameToIgnore.Equals(name))
                {
                    result.Add(name);
                }
            }

            return result;
        }
        
        public string GenerateNewName()
        {
            string finalName;
            if (Random.Range(0, 2) == 0)
            {
                finalName = listRandomNames[Random.Range(0, listRandomNames.Count)].Trim();
            }
            else
            {
                // string part1 = "Player";
                // string part2 = Random.Range(0, 1000000).ToString("000000");
                // finalName = (part1 + part2).Replace(" ", string.Empty);

                finalName = GenerateNewPlayerName();
            }
            return finalName;
        }

        public string GenerateNewPlayerName()
        {
            string part1 = "Player";
            string part2 = Random.Range(0, 1000000).ToString("000000");
            return (part1 + part2).Replace(" ", string.Empty);
        }
    }
}
