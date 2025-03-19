using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace IEC.Wind.Leaderboard
{
    [CreateAssetMenu(menuName = "Game/LeaderboardConfig")]
    public class LeaderboardConfig : BaseConfig
    {
        public int numMedalGetOnWin = 25;
        public int levelUnlock = 20;
        [Header("NAME CONFIG")]
        [SerializeField]
        string nameInput;
        public List<string> nameList = new List<string>();
        public List<Sprite> avatarSprites = new List<Sprite>();
        [Header("LEAGUE CONFIG")]
        public List<LeagueRankData> leagueRanks = new List<LeagueRankData>();
        public List<RankRewardData> rewardDatas = new List<RankRewardData>();

        [Header("SPRITE CONFIG")]
        public List<Sprite> rankSprites = new List<Sprite>();
        public List<Sprite> leagueSprites = new List<Sprite>();
        public List<Sprite> leagueSpritesLevelWin = new List<Sprite>();
        public List<Sprite> leagueIconRank = new List<Sprite>();
        public List<Sprite> textLeagueSprites = new List<Sprite>();
        public List<Sprite> frameSprites = new List<Sprite>();
        public List<Sprite> rewardBoxImage = new List<Sprite>();
        public Sprite playerFrameSprites;
        public Sprite botLeagueFrameSprite;
        public Sprite playerLeagueFrameSprite;
        [Header("COLOR CONFIG")]
        public List<ColorRankConfig> lstColor = new List<ColorRankConfig>();
        [Button]
        public void UpdateNames()
        {
            nameList = new List<string>();
            var nameListTemp = new List<string>(nameInput.Split(' '));
            foreach (var item in nameListTemp)
            {
                nameList.Add(item.Trim());
            }
        }

        [Button]
        public string GeneratorNewName()
        {
            string finalName;
            if (Random.Range(0, 2) == 0)
            {
                finalName = nameList[Random.Range(0, nameList.Count)].Trim();
            }
            else
            {
                string part1 = "Player";
                string part2 = Random.Range(0, 1000000).ToString("000000");
                finalName = (part1 + part2).Replace(" ", string.Empty);
            }
            return finalName;
        }
        public List<string> GeneratorAllNames(int num)
        {
            List<string> result = new List<string>();

            while (result.Count < num)
            {
                string name = GeneratorNewName();

                if (!result.Contains(name))
                {
                    result.Add(name);
                }
            }

            return result;
        }
        public LeagueRankData GetLeagueRankData(LeaderboardRankType rankType)
        {
            return leagueRanks.FirstOrDefault(x => x.type == rankType);
        }

        public Sprite GetSpirteCurrentLeague()
        {
            return leagueSprites[(int)Pool.Get<LeaderboardSaveData>().currentRankView];
        }      
        public Sprite GetSpirteCurrentLeagueLevelWin()
        {
            return leagueSpritesLevelWin[(int)Pool.Get<LeaderboardSaveData>().currentRankView];
        }      
        public Sprite GetSpriteCurrentTextLeague()
        {
            return textLeagueSprites[(int)Pool.Get<LeaderboardSaveData>().currentRankView];
        }      
        public Sprite GetLeagueIconRank()
        {
            return leagueIconRank[(int)Pool.Get<LeaderboardSaveData>().currentRankView];
        }
        public Sprite GetSpriteRank(int rank)
        {
            rank = Mathf.Clamp(rank, 1, rankSprites.Count);
            return rankSprites[rank - 1];
        }
        public Sprite GetBoxRewardSprite(int rank){
            switch (rank)
            {
                case 1:
                    return rewardBoxImage[0];
                case 2:
                    return rewardBoxImage[1];
                case 3:
                    return rewardBoxImage[2];
            }
            return rewardBoxImage[3];
        }
        public RankRewardData GetRewardData(LeaderboardRankType rankType)
        {
            return rewardDatas.FirstOrDefault(x => x.type == rankType);
        }
    }

    public enum LeaderboardRankType
    {
        Sage, Legend, Grandmaster, Master, Apprentice, Novice
    }
    public enum LeaderboardBotType
    {
        Rank1,
        Rank2,
        Rank3,
        Up,
        Down,
        Safe
    }
    [System.Serializable]
    public class LeagueRankData
    {
        public LeaderboardRankType type;
        public int countRankUp;
        public int countSafe;
        public int countRankDown;

        public List<BotData> botDatas = new List<BotData>();

        public int GetRandMedal(LeaderboardBotType botType)
        {
            BotData bot = botDatas.FirstOrDefault(x => x.botType == botType);

            var result = Random.Range(bot.minMedal, bot.maxMedal);
            result -= result % 5;
            return result;
        }

        public int Total => countRankUp + countSafe + countRankDown;
    }
    [System.Serializable]
    public class RankRewardData
    {
        public LeaderboardRankType type;
        public List<RewardData> rank1 = new List<RewardData>();
        public List<RewardData> rank2 = new List<RewardData>();
        public List<RewardData> rank3 = new List<RewardData>();
        public List<RewardData> rankUp = new List<RewardData>();
        public List<RewardData> rankSafe = new List<RewardData>();
        public List<RewardData> rankDown = new List<RewardData>();
    }
    [System.Serializable]
    public class BotData
    {
        public LeaderboardBotType botType;
        public int minMedal;
        public int maxMedal;
    }
    [System.Serializable]
    public class ColorRankConfig
    {
        public Color rankColor;
        public Color nameColor;
        public Material materialRankTxt;
    }
    public enum TypeFinish
    {
        Up,
        Safe,
        Down,
    }
    [System.Serializable]
    public class RewardData
    {
        public ResourceType type;
        public int amount;
    }
}