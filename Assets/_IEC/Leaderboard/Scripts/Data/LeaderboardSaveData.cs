using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC.Wind.Leaderboard
{
    [System.Serializable]
    public class LeaderboardSaveData : IData
    {
        public DateTime lastUpdateTime;
        public DateTime timeStart;
        public DateTime timeEnd;
        public DateTime timeStartX2Medal;

        public int medalMultiply = 1;
        public int avatarId = 0;
        public LeadboardCandidateData myData = new LeadboardCandidateData();
        public LeaderboardRankType currentRankView = LeaderboardRankType.Iron;
        public List<LeadboardBotData> candidateDatas = new List<LeadboardBotData>();

        public void SetCurrentRankView(LeaderboardRankType rank)
        {
            currentRankView = rank;
        }
        public void AddMedal(int medal)
        {
            myData.medal += medal;
            EventController.RaiseEvent(EventList.WindLeaderboard.UPDATE_MEDAL);
        }
        public void SetMedal(int medal)
        {
            myData.medal = medal;
            EventController.RaiseEvent(EventList.WindLeaderboard.UPDATE_MEDAL);
        }
        public void ChangeName(string name)
        {
            myData.name = name;
            EventController.RaiseEvent(EventList.WindLeaderboard.CHANGE_NAME);
        }
        public void ResetData()
        {
            myData.medal = 0;
            candidateDatas.Clear();
        }
        public void SetData(List<LeadboardBotData> data)
        {
            candidateDatas = data;
        }
        public List<UserData> GetAllUsers()
        {
            List<UserData> res = candidateDatas.Select(x=>new UserData()
            {
                data = x,
                isPlayer = false,
                rank = -1,
            }).ToList();
            res.Add(new UserData()
            {
                data = myData,
                isPlayer = true,
                rank = -1,
            });
            res.Sort((x, y) => {
                if (y.data.medal == x.data.medal)
                {
                    if (x.isPlayer) return -1;
                    if (y.isPlayer) return 1;
                }
                return y.data.medal.CompareTo(x.data.medal);
            });
            for(int i = 0; i < res.Count; i++)
            {
                res[i].rank = i + 1;
            }
            return res;
        }
        public int GetPlayerRank()
        {
            var allUsers = GetAllUsers();
            return allUsers.FirstOrDefault(x => x.isPlayer).rank;
        }
        public void UpDownRank(bool isUp = true)
        {
            if (isUp)
                currentRankView -= 1;
            else
                currentRankView += 1;
        }
        public void ActiveDoubleMedal()
        {
            timeStartX2Medal = DateTime.Now;
            medalMultiply = 2;
        }
        public void ChangeAvatar(int id)
        {
            avatarId = id;
            EventController.RaiseEvent(EventList.WindLeaderboard.CHANGE_NAME);
        }
    }
    [System.Serializable]
    public class LeadboardCandidateData
    {
        public string name = "Player467356";
        public int medal = 0;
    }
    [System.Serializable]
    public class LeadboardBotData : LeadboardCandidateData
    {
        public LeaderboardBotType botType;
    }
    [System.Serializable]
    public class UserData
    {
        public LeadboardCandidateData data;
        public bool isPlayer;
        public int rank;
    }
}
