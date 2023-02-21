using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace IEC.FakeLeaderboard
{
    public class FakeLeaderboardController : _BaseBehaviour, IController, IEventAwake, IEventPauseApp
    {
        [BoxGroup("Auto Load, NOT EDIT")]
        public FakeLeaderboardDataSave leaderDataSave; //=> leaderDataSave;
        private const string FAKE_LB_DATA_SAVE = "FakeLeaderboardController.FAKE_LB_DATA_SAVE";
        [BoxGroup("Auto Load, NOT EDIT")]
        public FakeLeaderboardDataConfig dataConfig;

        public void BehaviourAwake()
        {
            InitDataOnStartApp();
            InitDataOnNewDay();
        }

        public bool IsActive()
        {
            return true;
        }

        public void LocateController()
        {
            Controllers.Add(this);
        }

        private void InitDataOnStartApp()
        {
            leaderDataSave = Services.DataSecure()
                .GetObject(FAKE_LB_DATA_SAVE, new FakeLeaderboardDataSave());

            dataConfig = Resources.Load<FakeLeaderboardDataConfig>("Config/FakeLeaderboardDataConfig");
            
            if (leaderDataSave.userData == null)
                leaderDataSave.userData = new FakeLeaderboardUserData(true);
            leaderDataSave.userData.nationalID = GetCurrentCountryID();
            //init top3
            if (leaderDataSave.top3 == null || leaderDataSave.top3.Count != 3)
            {
                leaderDataSave.top3 = new List<FakeLeaderboardUserData>();
                leaderDataSave.top3.Add(new FakeLeaderboardUserData()
                {
                    name = "500bruh",
                    trophy = 22678,
                    nationalID = dataConfig.GeneratorNewNational().index
                });
                leaderDataSave.top3.Add(new FakeLeaderboardUserData()
                {
                    name = "Romeo8989",
                    trophy = 22969,
                    nationalID = dataConfig.GeneratorNewNational().index
                });
                leaderDataSave.top3.Add(new FakeLeaderboardUserData()
                {
                    name = "Tenor",
                    trophy = 23571,
                    nationalID = dataConfig.GeneratorNewNational().index

                });
            }

            //init OtherPlayer:
            if (leaderDataSave.otherPlayers == null)
            {
                //NUll, generate new ranking list;
                leaderDataSave.otherPlayers = new List<FakeLeaderboardUserData>();
                List<int> trophies = dataConfig.GeneratorTrophiesList();
                for (int i = 0; i < trophies.Count; i++)
                {
                    leaderDataSave.otherPlayers.Add(new FakeLeaderboardUserData()
                    {
                        trophy = trophies[i]
                    });
                }
            }

            //fake userData
            for (int i = 0; i < leaderDataSave.otherPlayers.Count; i++)
            {
                FakeLeaderboardUserData user = leaderDataSave.otherPlayers[i];
                user.name = dataConfig.GeneratorNewName();
                user.nationalID = dataConfig.GeneratorNewNational().index;
            }

        }

        private void InitDataOnNewDay()
        {

            //Debug.LogError(leaderDataSave.lastTime.ToString());
            bool needUpdateScore = false;
            var lastTime = leaderDataSave.lastTime;
            var now = System.DateTime.Now;
            if (leaderDataSave.lastTime.Year > 1 && now.Year == lastTime.Year && now.DayOfYear > lastTime.DayOfYear)
            {
                needUpdateScore = true;
            }

            leaderDataSave.lastTime = now;
            if (!needUpdateScore) return;
            Debug.Log("Update top3 score");
            leaderDataSave.top3.Find(x => x.name.Equals("Tenor")).trophy += Random.Range(5, 11);
            leaderDataSave.top3.Find(x => x.name.Equals("Romeo8989")).trophy += Random.Range(4, 7);
            leaderDataSave.top3.Find(x => x.name.Equals("500bruh")).trophy += Random.Range(2, 5);
            leaderDataSave.top3 = leaderDataSave.top3.OrderBy(x => x.trophy).ToList();
        }


        public int AddTrophy(bool isBonusLevel, bool isBlindMode, bool isInDailyMode)
        {
            Debug.Log(PlayerData.level + " is bonus " + isBonusLevel + " blind Mode " + isBlindMode + "_daily mode:" + isInDailyMode);
            if (PlayerData.level <= 2) return 0; //skip tutorial
            if (isInDailyMode) return 0; //skip daily mode
            if (leaderDataSave.userData == null)
                leaderDataSave.userData = new FakeLeaderboardUserData(true);
            int trophyAdd = 1;
            if (isBonusLevel)
                trophyAdd = 2;
            if (isBlindMode)
                trophyAdd = 3;

            if (PlayerData.level == 3 || PlayerData.level == 4 || PlayerData.level == 6 || PlayerData.level == 7)
                trophyAdd = 2;
            leaderDataSave.userData.trophy += trophyAdd;
            return trophyAdd;
        }

        public LeaderboardRankData TrophyToRank(int trophy)
        {
            return dataConfig.TrophyToRank(trophy);
        }

        public LeaderboardRankData UserRank()
        {
            return dataConfig.TrophyToRank(leaderDataSave.userData.trophy);
        }

        public int UserPosition()
        {
            return leaderDataSave.IndexOfPlayer;
        }

        public NationalData NationalByID(int id)
        {
            return dataConfig.NationalByID(id);
        }


        public void UpdateUsername(string userName)
        {

            if (leaderDataSave.userData == null)
                leaderDataSave.userData = new FakeLeaderboardUserData(true);
            if (userName.Trim().Length > 0)
                leaderDataSave.userData.name = userName;
        }


        public void UpdateMyUserCellView(FakeLeaderboardCellView m_cellMy)
        {
            //  leaderDataSave.userData.index = UserPosition();
            m_cellMy.LoadData(leaderDataSave.userData);
        }


        public static int GetCurrentCountryID()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Unknown:
                    return 0;
                    break;
                case SystemLanguage.Vietnamese:
                    return 7;
                    break;
                case SystemLanguage.Arabic:
                    return 1;
                    //india
                    break;
                case SystemLanguage.English://us
                    return 0;
                    break;
                case SystemLanguage.Portuguese://brazil
                    return 2;
                    break;
                case SystemLanguage.Spanish://mexico
                    return 3;
                    break;
                case SystemLanguage.Japanese://mexico
                    return 4;
                    break;
                case SystemLanguage.Chinese://mexico
                    return 5;
                    break;
                case SystemLanguage.ChineseSimplified://mexico
                    return 5;
                    break;
                case SystemLanguage.ChineseTraditional://mexico
                    return 5;
                    break;
                case SystemLanguage.Korean://mexico
                    return 6;
                    break;
                default:
                    break;
            }
            return 0;
        }

        public void ShowLeaderBoardPanelEndGame(PopupFakeLeaderboardEndgame leaderboardPanel, bool isBonusLevel, bool isBlindMode, bool isDailyMode)
        {
            var rankBFAdd = UserRank();
            var posBFAdd = UserPosition();
            int trophyAdd = AddTrophy(isBonusLevel, isBlindMode, isDailyMode);
            var rankATAdd = UserRank();
            var posATAdd = UserPosition();
            //  Debug.LogError(trophyAdd);
            if (trophyAdd <= 0)
                leaderboardPanel.gameObject.SetActive(false);
            else
            {
                if (rankBFAdd.rankName != rankATAdd.rankName && rankATAdd.rankName != FakeLeaderboardRankType.Unrank)
                {

                    //Rankup everytime
                    leaderboardPanel.ShowRankup(true);
                }
                else
                {
                    if ((posBFAdd != posATAdd) && (rankATAdd.rankName != FakeLeaderboardRankType.Unrank) && (PlayerData.level == 7 || PlayerData.level == 11 || PlayerData.level == 17 || PlayerData.level >= 17 && (PlayerData.level - 17) % 5 == 0))
                    {

                        leaderboardPanel.ShowRankup(false, posBFAdd != posATAdd);
                    }
                    else
                        leaderboardPanel.ShowWinTrophy(trophyAdd);
                }
            }
        }
        
        public void BehaviourPauseApp(bool isPause)
        {
            if (isPause)
            {
                if(Game.isDebugReset) return;
                Services.DataSecure().SetObject(FAKE_LB_DATA_SAVE, leaderDataSave);
                Services.SaveData();
            }
        }
    }
}
