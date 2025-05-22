using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/GameEvent/GameEventConfig")]
    public class GameEventConfig : ScriptableObject
    {
        public int levelUnlock;
        public EventType eventType;
        public float eventDurationInHours;
        public float timeUpdateEventProgressInHours;
        public bool updateEventAfterPlayerWinLevel;

        [Header("UI")] 
        public List<GameEventDisplaySprite> listDisplaySprites = new();

        public List<EventData> listEventData = new();
        public List<EventRewardTitleData> listTitleData = new();
        
        [Header("Load data from csv (file format speed|range + reward)")]
        public string csv1;
        
        [Header("Load data from csv (file format stages + reward)")]
        public string csv2;

        [Button]
        public void LoadData1()
        {
            var dataDefault = DownloadCsvFromAuthUrl(csv1);

            if (dataDefault.Count == 0)
                return;
            
            listEventData.Clear();

            List<EventBotData> listEventBotData = new();
            
            foreach (var row in dataDefault)
            {
                // Stop if row is empty or starts reward section
                if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0]) || row[0].Trim().ToLower() == "rank")
                    break;

                // Skip invalid rows
                if (row.Count < 2 || string.IsNullOrWhiteSpace(row[0]) || string.IsNullOrWhiteSpace(row[1]))
                    continue;

                if (float.TryParse(row[0], out float baseSpeed) && float.TryParse(row[1], out float speedRange))
                {
                    listEventBotData.Add(new EventBotData()
                    {
                        baseSpeed = baseSpeed,
                        speedRange = speedRange
                    });
                }
            }
            
            listEventData.Add(
                new EventData()
                {
                    listEventBotData = new List<EventBotData>(listEventBotData)
                });
            
            // Continue parsing reward data
            int rewardStartIndex = dataDefault.FindIndex(row => row.Contains("Rank") && row.Contains("Rewards"));
            if (rewardStartIndex < 0 || rewardStartIndex + 2 >= dataDefault.Count)
                return;

            // Map columns to ResourceTypes based on header
            var rewardHeader = dataDefault[rewardStartIndex + 1];
            Dictionary<int, ResourceType> rewardTypeMap = new();
            for (int i = 1; i < rewardHeader.Count; i++)
            {
                string header = rewardHeader[i].Trim().ToLowerInvariant();
                switch (header)
                {
                    case "gold":
                        rewardTypeMap[i] = ResourceType.Gold;
                        break;
                    case "magnet":
                        rewardTypeMap[i] = ResourceType.Vortex;
                        break;
                    case "compass":
                        rewardTypeMap[i] = ResourceType.Compass;
                        break;
                    case "enlarge":
                        rewardTypeMap[i] = ResourceType.Enlarge;
                        break;
                    case "time stop":
                        rewardTypeMap[i] = ResourceType.TimeStop;
                        break;
                    case "windsurf":
                        rewardTypeMap[i] = ResourceType.WindSurf;
                        break;
                    case "evolve pill":
                        rewardTypeMap[i] = ResourceType.EvolveStone;
                        break;
                    case "infinity lives":
                        rewardTypeMap[i] = ResourceType.UnlimitedLive;
                        break;
                }
            }

            // Attach reward data to the first EventData
            if (listEventData.Count == 0)
                listEventData.Add(new EventData());

            var firstEventData = listEventData[0];
            firstEventData.listEventRewardData = new List<EventRewardData>();

            for (int i = rewardStartIndex + 2; i < dataDefault.Count; i++)
            {
                var row = dataDefault[i];
                if (row.Count < 2 || string.IsNullOrWhiteSpace(row[0]))
                    continue;

                var rewardData = new EventRewardData();
                string rankString = row[0].Trim().ToLower();

                // Handle rank parsing
                if (rankString.Contains("-"))
                {
                    var parts = rankString.Replace("th", "").Split("-");
                    rewardData.rankStartReward = int.Parse(parts[0]);
                    rewardData.rankEndReward = int.Parse(parts[1].Replace("th", "").Replace("th+", "").Trim());
                }
                else if (rankString.Contains("+"))
                {
                    rewardData.rankStartReward = int.Parse(rankString.Replace("th+", "").Replace("th", "").Trim());
                    rewardData.rankEndReward = -1;
                }
                else
                {
                    rewardData.rankStartReward = int.Parse(rankString);
                    rewardData.rankEndReward = rewardData.rankStartReward;
                }

                rewardData.listReward = new List<Reward>();

                for (int j = 1; j < row.Count; j++)
                {
                    if (!rewardTypeMap.TryGetValue(j, out var type))
                        continue;

                    string val = row[j].Trim().ToLower();

                    if (val.EndsWith("m")) // booster duration
                    {
                        if (int.TryParse(val.Replace("m", ""), out int duration))
                            rewardData.listReward.Add(new Reward(duration, type));
                    }
                    else if (int.TryParse(val, out int amount) && amount > 0)
                    {
                        rewardData.listReward.Add(new Reward(amount, type));
                    }
                }

                firstEventData.listEventRewardData.Add(rewardData);
            }
        }

        [Button]
        public void LoadData2()
        {
            var dataDefault = DownloadCsvFromAuthUrl(csv2);

            if (dataDefault.Count == 0)
                return;

            listEventData.Clear();

            // === 1. Parse Bot Data ===
            float globalSpeedRange = 0f;

            // Search for header row that contains speed range
            foreach (var row in dataDefault)
            {
                if (row.Count > 7 && float.TryParse(row[7], out float parsedRange))
                {
                    globalSpeedRange = parsedRange;
                    break;
                }
            }

            // Start reading actual bot data rows (after headers)
            for (int i = 2; i < dataDefault.Count; i++)
            {
                var row = dataDefault[i];
                if (row.Count < 7 || string.IsNullOrEmpty(row[0])) break;

                var eventData = new EventData();
                eventData.eventTarget = int.Parse(row[1]); // Level Goal

                eventData.listEventBotData = new List<EventBotData>();

                // Parse bot speeds (Bot1 to Bot4)
                for (int j = 3; j < row.Count; j++)
                {
                    if (float.TryParse(row[j], out float botSpeed))
                    {
                        eventData.listEventBotData.Add(new EventBotData
                        {
                            baseSpeed = botSpeed,
                            speedRange = globalSpeedRange
                        });
                    }
                }

                listEventData.Add(eventData);
            }

            // === 2. Parse Reward Data ===
            // Step 1: find reward headers
            int rewardStartIndex = dataDefault.FindIndex(r => r.Count > 0 && r[0] == "Race" && r[1] == "Rank");

            if (rewardStartIndex == -1)
            {
                Debug.LogWarning("Reward section not found in data.");
                return;
            }

            var rewardHeaderRow = dataDefault[rewardStartIndex + 1];
            Dictionary<int, ResourceType> rewardColumnMap = new();

            for (int i = 2; i < rewardHeaderRow.Count; i++)
            {
                var header = rewardHeaderRow[i].Trim();
                if (TryMapHeaderToResourceType(header, out ResourceType type))
                {
                    rewardColumnMap[i] = type;
                }
            }

            // Step 2: parse rewards by race
            int lastRaceIndex = -1;

            for (int i = rewardStartIndex + 2; i < dataDefault.Count; i++)
            {
                var row = dataDefault[i];
                if (row.Count < 3 || rewardColumnMap.Count == 0)
                    continue;

                // Determine race index (stage)
                if (!string.IsNullOrEmpty(row[0]))
                {
                    if (!int.TryParse(row[0], out lastRaceIndex))
                        continue;
                }

                if (lastRaceIndex == -1)
                    continue; // Still no valid raceIndex seen yet

                // Validate bounds
                if (lastRaceIndex - 1 < 0 || lastRaceIndex - 1 >= listEventData.Count)
                    continue;

                var eventData = listEventData[lastRaceIndex - 1];

                // Parse reward rank and data
                var rewardData = new EventRewardData();
                rewardData.rankStartReward = int.Parse(row[1]);
                rewardData.rankEndReward = rewardData.rankStartReward;
                rewardData.listReward = new List<Reward>();

                foreach (var kvp in rewardColumnMap)
                {
                    int colIndex = kvp.Key;
                    var type = kvp.Value;
                    string val = row[colIndex].Trim();

                    if (type == ResourceType.UnlimitedLive || 
                        type == ResourceType.TimeUnlimitedWindSurf || 
                        type == ResourceType.TimeUnlimitedEvolveStone)
                    {
                        if (val.EndsWith("m") && int.TryParse(val.Replace("m", ""), out int duration))
                        {
                            rewardData.listReward.Add(new Reward(duration, type));
                        }
                    }
                    else
                    {
                        if (int.TryParse(val, out int amt) && amt > 0)
                        {
                            rewardData.listReward.Add(new Reward(amt, type));
                        }
                    }
                }

                if (eventData.listEventRewardData == null)
                    eventData.listEventRewardData = new List<EventRewardData>();

                eventData.listEventRewardData.Add(rewardData);
            }
            
        }

        public GameEventDisplaySprite GetDisplaySprite(int index)
        {
            if (listDisplaySprites.Count == 0)
                return null;
            
            index = Mathf.Clamp(index, 0, listDisplaySprites.Count);

            return listDisplaySprites[index];
        }

        public EventData GetEventData(int index)
        {
            index = Mathf.Clamp(index, 0, listEventData.Count - 1);
            return listEventData[index];
        }

        public Sprite GetRankSprite(int spriteSetIndex, int rank)
        {
            if (rank > GetDisplaySprite(spriteSetIndex).listRankSprites.Count)
                return null;

            return GetDisplaySprite(spriteSetIndex).listRankSprites[rank - 1];
        }
        
        public Sprite GetRewardButtonSprite(int spriteSetIndex, int rank)
        {
            if (rank > GetDisplaySprite(spriteSetIndex).listRewardButtonSprites.Count)
                return GetDisplaySprite(spriteSetIndex).listRewardButtonSprites[^1];

            return GetDisplaySprite(spriteSetIndex).listRewardButtonSprites[rank - 1];
        }

        public EventRewardTitleData GetRewardTitleData(int rank)
        {
            return listTitleData.Find(data => data.rankStartReward <= rank && (data.rankEndReward >= rank || data.rankEndReward == -1));
        }
        
        private List<List<string>> DownloadCsvFromAuthUrl(string url)
        {
            var jsoner = new IEC.JsonDotNetModule();
            var uwr = UnityEditorHelper.GetWebRequestCsvFromAuthUrl(url);
            uwr.SendWebRequest();
            while (!uwr.isDone)
            {
                //Debug.Log(uwr.downloadProgress);
            }

            if (uwr.isNetworkError)
            {
                Debug.LogError("Download Error");
                Debug.LogError($"Downloading: {url}");
            }

            Debug.Log("Data downloaded:\n" + uwr.downloadHandler.text);

            return jsoner.FromJson<List<List<string>>>(uwr.downloadHandler.text);
        }
        
        private bool TryMapHeaderToResourceType(string header, out ResourceType type)
        {
            type = ResourceType.Gold;

            switch (header.Trim().ToLowerInvariant())
            {
                case "gold": type = ResourceType.Gold; return true;
                case "magnet": type = ResourceType.Vortex; return true;
                case "compass": type = ResourceType.Compass; return true;
                case "enlarge": type = ResourceType.Enlarge; return true;
                case "time stop": type = ResourceType.TimeStop; return true;
                case "windsurf": type = ResourceType.TimeUnlimitedWindSurf; return true;
                case "evolve pill": type = ResourceType.TimeUnlimitedEvolveStone; return true;
                case "infinity lives": type = ResourceType.UnlimitedLive; return true;
                default: return false;
            }
        }
    }

    [System.Serializable]
    public class GameEventDisplaySprite
    {
        public Sprite normalFrameSprite;
        public Sprite normalProgressFrameSprite;
        public Sprite mainPlayerFrameSprite;
        public Sprite mainPlayerProgressFrameSprite;
        public List<Sprite> listRankSprites = new();
        public List<Sprite> listRewardButtonSprites = new();
    }

    [System.Serializable]
    public class EventData
    {
        public int eventTarget; //For event with multiple stages 
        public List<EventBotData> listEventBotData = new();
        public List<EventRewardData> listEventRewardData = new();
    }

    [System.Serializable]
    public class EventBotData
    {
        public float baseSpeed;
        public float speedRange;
    }

    [System.Serializable]
    public class EventRewardData
    {
        public int rankStartReward;
        public int rankEndReward;
        public List<Reward> listReward = new();
    }

    [System.Serializable]
    public class EventRewardTitleData
    {
        public int rankStartReward;
        public int rankEndReward;
        public Sprite claimRewardTitleSprite;
        [TextArea]public string claimRewardSubtitleText;
    }
}
