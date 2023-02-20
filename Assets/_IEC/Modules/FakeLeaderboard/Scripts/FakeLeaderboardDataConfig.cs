using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC.FakeLeaderboard
{
    [CreateAssetMenu(fileName = "FakeLeaderboardDataConfig", menuName = "IEC/FakeLeaderboardDataConfig")]
    public class FakeLeaderboardDataConfig : ScriptableObject
    {
        [Header("NAME CONFIG")]
        [SerializeField]
        string nameInput;
        public List<string> nameList;
        public List<NameSuffix> nameSuffixList;
        public List<NationalData> nationalList;
        public List<LeaderboardRankData> ranksList;

        [Button]
        public void UpdateNames()
        {
            nameList = new List<string>();
            var nameListTemp = new List<string>(nameInput.Split(','));
            foreach (var item in nameListTemp)
            {
                nameList.Add(item.Trim());
            }
        }

        [Button]
        public string GeneratorNewName()
        {
            string part1 = nameList[Random.Range(0, nameList.Count)].Trim();
            Dictionary<string, float> part2RandomWeight = new Dictionary<string, float>();
            foreach (var item in nameSuffixList)
            {
                part2RandomWeight.Add(item.name, item.percent);
            }
            RandomPro pro = new RandomPro(Random.Range(0, 100));
            string part2 = pro.WeightRandom<string>(part2RandomWeight).Key;
            string finalName = (part1 + part2).Replace(" ", string.Empty);
            //  Debug.Log(finalName);
            return finalName;
        }

        [Button]
        public NationalData GeneratorNewNational()
        {
            Dictionary<NationalData, float> randomWeight = new Dictionary<NationalData, float>();
            foreach (var item in nationalList)
            {
                randomWeight.Add(item, item.percent);
            }
            RandomPro pro = new RandomPro(Random.Range(0, 100));
            NationalData output = pro.WeightRandom<NationalData>(randomWeight).Key;
            //  string finalName = (part1 + part2).Replace(" ", string.Empty);
            //  Debug.Log(JsonUtility.ToJson(output));

            return output;
        }

        public NationalData NationalByID(int id)
        {
            return nationalList.FirstOrDefault(x => x.index == id);
        }


        public List<int> GeneratorTrophiesList()
        {
            List<int> output = new List<int>();
            for (int i = 0; i < ranksList.Count; i++)
            {
                var rankInfor = ranksList[i];
                RandomPro random = new RandomPro();
                List<int> rankRandomTrophies = new List<int>();
                for (int j = 0; j < rankInfor.numberFakeUser; j++)
                    rankRandomTrophies.Add(random.RangeFromTo(rankInfor.minFakeTrophies, rankInfor.maxFakeTrophies));
                rankRandomTrophies = rankRandomTrophies.OrderBy(x => x).ToList();
                output.AddRange(rankRandomTrophies);
            }

            return output;
        }

        public LeaderboardRankData TrophyToRank(int trophy)
        {
            for (int i = ranksList.Count - 1; i >= 0; i--)
            {
                if (trophy >= ranksList[i].trophyToRankup)
                    return ranksList[i];
            }
            return new LeaderboardRankData() { rankName = FakeLeaderboardRankType.Unrank };
        }
    }

    [System.Serializable]
    public class NameSuffix
    {
        public string name;
        public float percent;
    }

    [System.Serializable]
    public class NationalData
    {
        public int index;
        public string name;
        public Sprite flag;
        public float percent;
    }

    [System.Serializable]
    public class FakeLeaderboardUserData
    {
        public int index { get; set; }
        public FakeLeaderboardUserData(bool isMyUser = false)
        {
            name = "";
            nationalID = 0;
            trophy = 0;
            if (isMyUser)
            {
                name = "You";
                nationalID = FakeLeaderboardController.GetCurrentCountryID();
            }
        }
        public string name;
        public int nationalID;
        public int trophy;
    }


    public enum FakeLeaderboardRankType
    {
        Novice, Apprentice, Master, GrandMaster, Elder, Sage, Unrank
    }

    [System.Serializable]
    public class LeaderboardRankData
    {
        public FakeLeaderboardRankType rankName;
        public int trophyToRankup;
        public int percentPlayer;
        public int numberFakeUser;
        public int minFakeTrophies;
        public int maxFakeTrophies;
        public Sprite rankView;
    }
}