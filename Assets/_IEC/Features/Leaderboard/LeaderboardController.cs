using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = System.Random;

namespace IEC.Wind.Leaderboard
{
    public class LeaderboardController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart, IEventUpdate
    {
        public const string LEADERBOARD_DATA_SECRET = "LEADERBOARD_DATA_SECRET";

        public LeaderboardSaveData leaderboardSaveData;
        public LeaderboardConfig leaderboardConfig;
        public void LocateController()
        {
            Controllers.Add(this);
        }

        private bool wasStart = false;
        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.WindLeaderboard.CHANGE_AVATAR:
                    Locator.Main.PopupLeaderboard.UpdateUI();
                    Locator.Main.PopupEditInfo.UpdateUI();
                    break;
                case EventList.WindLeaderboard.CHANGE_NAME:
                    Locator.Main.PopupLeaderboard.UpdateUI();
                    Locator.Main.PopupLeaderboard.ReloadData();
                    break;
                case EventList.GamePlay.LEVEL_WIN:
                    if (Game.isHaveLeaderboard && PlayerData.level >= LeaderboardConfig.levelUnlock)
                    {
                        int medal = leaderboardConfig.numMedalGetOnWin; 
                        medal *= LeaderboardData.medalMultiply;
                        Locator.Game.ScreenLevelWin.SetMedalReward(medal);
                        Locator.Game.ScreenLevelWinHard.SetMedalReward(medal);
                        LeaderboardData.AddMedal(medal);
                        Game.isHaveAnimMedal = true;
                    }
                    else
                    {
                        Locator.Game.ScreenLevelWin.DeactiveMedalObj();
                        Locator.Game.ScreenLevelWinHard.DeactiveMedalObj();
                    }
                    CheckEndTournamentAndInit();
                    break;
            }
        }

        public void BehaviourAwake()
        {
            var dataController = Controllers.Get<DataController>();
            leaderboardSaveData = dataController.LoadData<LeaderboardSaveData>(LEADERBOARD_DATA_SECRET);
            Pool.Set(leaderboardSaveData);
            dataController.SetAutoSaveObject(LEADERBOARD_DATA_SECRET, leaderboardSaveData);
            Pool.Set(leaderboardConfig);
        }

        public void BehaviourStart()
        {
            //StartTournament();
        }

        public void InitData()
        {
            leaderboardSaveData.ResetData();
            LeaderboardRankType rankType = leaderboardSaveData.currentRankView;
            var leagueRank = leaderboardConfig.GetLeagueRankData(rankType);
            int totalBot = leagueRank.Total - 1;
            var lstName = leaderboardConfig.GeneratorAllNames(totalBot);
            List<LeadboardBotData> bots = lstName.Select(x => new LeadboardBotData()
            {
                name = x,
                medal =0,
            }).ToList();
            for (int i=0;i< leagueRank.countRankDown; i++)
            {
                bots[i].botType = LeaderboardBotType.Down;
            }
            for(int i = 0; i < leagueRank.countSafe; i++)
            {
                if (i + leagueRank.countRankDown >= bots.Count) break;
                bots[i + leagueRank.countRankDown].botType = LeaderboardBotType.Safe;
            }
            for (int i = leagueRank.countRankDown + leagueRank.countSafe; i < bots.Count; i++)
            {
                bots[i].botType = LeaderboardBotType.Up;
            }

            foreach (var bot in bots)
            {
                bot.avatarId = UnityEngine.Random.Range(0, LeaderboardConfig.avatarSprites.Count);
            }
            bots.Shuffle();
            var temps = bots.Take(3).ToList();
            temps[0].botType = LeaderboardBotType.Rank1;
            temps[1].botType = LeaderboardBotType.Rank2;
            temps[2].botType = LeaderboardBotType.Rank3;           
            leaderboardSaveData.SetData(bots);
        }

        public void StartTournament()
        {
            if (!wasStart)
            {
                //Delay call, call from home Scene,
                wasStart = true;
                CheckEndTournamentAndInit();
            }

        }
        public void CheckEndTournamentAndInit()
        {
            if (!Game.isHaveLeaderboard) return;

            var doW = (int)DateTime.Now.DayOfWeek;
            int daysUntilLastMonday = ((int)DayOfWeek.Monday - doW - 7) % 7;

            var lstMonday = DateTime.Now.AddDays(daysUntilLastMonday);
            var startTime = new DateTime(lstMonday.Year, lstMonday.Month, lstMonday.Day, 0, 0, 0, 0);
            if (leaderboardSaveData.candidateDatas.Count <= 0 || startTime > leaderboardSaveData.timeStart)
            {
                if (leaderboardSaveData.candidateDatas.Count > 0)
                {
                    UpdateMedalOfBots();
                    ShowResultLastWeek();
                }

                //new weeks
                leaderboardSaveData.timeStart = startTime;
                leaderboardSaveData.timeEnd = startTime.AddDays(7) - new TimeSpan(0, 0, 1);
                leaderboardSaveData.lastUpdateTime = startTime;
                InitData();
            }
            UpdateMedalOfBots();
        }
        public void UpdateMedalOfBots()
        {
            if (leaderboardSaveData.candidateDatas.Count <= 0) return;
            LeaderboardRankType rankType = leaderboardSaveData.currentRankView;
            var leagueRank = leaderboardConfig.GetLeagueRankData(rankType);
            var timeEnd = DateTime.Now;
            if (timeEnd > leaderboardSaveData.timeEnd)
            {
                timeEnd = leaderboardSaveData.timeEnd;
            }
            var time = timeEnd - leaderboardSaveData.lastUpdateTime;

            int num = (int)(time.TotalHours / 4);

            if (num >= 1)
            {
                leaderboardSaveData.lastUpdateTime = leaderboardSaveData.lastUpdateTime.AddHours(num * 4);

                for (int i = 0; i < num; i++)
                {
                    leaderboardSaveData.candidateDatas.ForEach(x => x.medal += leagueRank.GetRandMedal(x.botType));
                }
            }
        }

        public void ShowResultLastWeek()
        {
            LeaderboardRankType rankType = leaderboardSaveData.currentRankView;
            var leagueRank = leaderboardConfig.GetLeagueRankData(rankType);
            int rank = leaderboardSaveData.GetPlayerRank();
            TypeFinish typeFinish = TypeFinish.Safe;

            if (leagueRank.countRankUp > 0 && rank <= leagueRank.countRankUp)
            {
                typeFinish = TypeFinish.Up;
            }
            else if (leagueRank.countRankDown > 0 && rank > leagueRank.countRankUp + leagueRank.countSafe)
            {
                typeFinish = TypeFinish.Down;
            }

            RankRewardData rewardData = leaderboardConfig.GetRewardData(rankType);

            List<RewardData> rewards = new List<RewardData>();

            switch (rank)
            {
                case 1:
                    rewards = rewardData.rank1;
                    break;
                case 2:
                    rewards = rewardData.rank2;
                    break;
                case 3:
                    rewards = rewardData.rank3;
                    break;
                default:
                    rewards = typeFinish switch
                    {
                        TypeFinish.Safe => rewardData.rankSafe,
                        TypeFinish.Up => rewardData.rankUp,
                        TypeFinish.Down => rewardData.rankDown,
                        _ => rewardData.rankSafe,
                    };
                    break;
            }
            if (typeFinish == TypeFinish.Down || typeFinish == TypeFinish.Up)
                leaderboardSaveData.UpDownRank(typeFinish == TypeFinish.Up);

            //foreach(var rw in rewards)
            //{
            //    PlayerData.AddReward(rw.type, rw.amount);
            //}
            //UI.ScreenInGame.UpdateButtonHelper();
            //Locator.Main.HomeScreenTab.ScreenHome.UpdateUILeaderboard();
            Locator.Main.ClaimLeaderBoardFinish.SetData(rankType, typeFinish, rewards);
            Locator.Main.ClaimLeaderBoardFinish.ShowingRewardPanel();
        }

        public TimeSpan GetTimeLeaderboardRemain()
        {
            var now = DateTime.Now;
            if (now >= leaderboardSaveData.timeEnd) return new TimeSpan(0);
            return leaderboardSaveData.timeEnd - now;
        }
        public string GetTimeRemainText()
        {
            var time = GetTimeLeaderboardRemain();
            string txt = "";
            if (time.TotalDays > 1)
            {
                txt = time.ToString("d'd 'h'h'");
            }
            else if (time.TotalHours > 1)
            {
                txt = time.ToString("h'h 'm'm'");
            }
            else
            {
                txt = time.ToString("m'm 's's'");
            }
            return txt;
        }
        public List<RewardData> GetRewardDataByRank(int rank)
        {
            LeaderboardRankType rankType = leaderboardSaveData.currentRankView;
            var leagueRank = leaderboardConfig.GetLeagueRankData(rankType);

            TypeFinish typeFinish = TypeFinish.Safe;

            if (leagueRank.countRankUp > 0 && rank <= leagueRank.countRankUp)
            {
                typeFinish = TypeFinish.Up;
            }
            else if (leagueRank.countRankDown > 0 && rank > leagueRank.countRankUp + leagueRank.countSafe)
            {
                typeFinish = TypeFinish.Down;
            }

            RankRewardData rewardData = leaderboardConfig.GetRewardData(rankType);
            List<RewardData> rewards = new List<RewardData>();
            switch (rank)
            {
                case 1:
                    rewards = rewardData.rank1;
                    break;
                case 2:
                    rewards = rewardData.rank2;
                    break;
                case 3:
                    rewards = rewardData.rank3;
                    break;
                default:
                    rewards = typeFinish switch
                    {
                        TypeFinish.Safe => rewardData.rankSafe,
                        TypeFinish.Up => rewardData.rankUp,
                        TypeFinish.Down => rewardData.rankDown,
                        _ => rewardData.rankSafe,
                    };
                    break;
            }
            return rewards;
        }

        public void BehaviourUpdate(float deltaTime)
        {
            if (Game.isHaveLeaderboard && leaderboardSaveData.medalMultiply > 1)
            {
                var time = DateTime.Now - leaderboardSaveData.timeStartX2Medal;

                if (time.TotalMinutes >= 30)
                {
                    leaderboardSaveData.medalMultiply = 1;
                    Locator.Main.PopupLeaderboard.UpdateUI();
                    //UI.ScreenHome.UpdateLeaderboard();
                }
            }
        }
    }
}