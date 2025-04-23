using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace IEC.Wind.Leaderboard
{
    public class PopupLeaderboard : BasePopup, IEnhancedScrollerDelegate
    {
        [SerializeField] EnhancedScroller scroller;

        public Image avatar;
        public TextMeshProUGUI userNameTxt;

        public Button completeBtn;
        public Button medalX2Btn;
        public Button editInfoBtn;
        public Button rankInfoBtn;

        public Image leagueImgText;
        public Image leagueImg;

        public TextMeshProUGUI timeX2MedalTxt;
        public TextMeshProUGUI timeRemainTxt;
        public TextMeshProUGUI lockLeaderBoardText;

        public RankItem botRankItem;
        public RankItem playerRankItem;
        
        LeagueRankData leagueData;
        List<UserData> allUsers = new List<UserData>();
        public GameObject lockObj;

        protected override void Awake()
        {
            base.Awake();
            isPlayAudioPopup = false;
        }

        private void Start()
        {
            SetEvent(completeBtn, () =>
            {
                //Game.isChallengeMode = false;
                EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
                //Services.PushEvent(new TrackingEvent.ClickPlayLevel());
                Hide();
            });
            SetEvent(medalX2Btn, () =>
            {
                Locator.Main.PopupDoubleMedal.Show();
            });
            SetEvent(editInfoBtn, () =>
            {
                Locator.Main.PopupEditInfo.Show();
            });
            SetEvent(rankInfoBtn, () =>
            {
                Locator.Main.PopupRankInfo.Show();
            });
            scroller.OnBeingDrag+=DisableRewardPopupMini ;
        }

        public void DisableRewardPopupMini()
        {
            EventController.RaiseEvent(EventList.GamePlay.SHOW_REWARD_INFO_LEADERBOARD);
        }
        public override void Show()
        {
            base.Show();
            Locator.Main.PopupRankInfo.Hide();
            LeaderboardController.CheckEndTournamentAndInit();
            Init();
            leagueImgText.SetNativeSize();
            if (Game.isHaveLeaderboard && PlayerData.level >= LeaderboardConfig.levelUnlock)
            {
                lockObj.gameObject.SetActive(false);
                scroller.gameObject.SetActive(true);
                UpdateUILeague();
                UpdateUI();
                ReloadData();
            }
            else
            {
                SetTextLockLeaderBoard();
                lockObj.gameObject.SetActive(true);
                scroller.gameObject.SetActive(false);
            }
            
            //JumpToPlayer();
        }

        public void SetTextLockLeaderBoard()
        {
            lockLeaderBoardText.text = "Unlock At Level " + LeaderboardConfig.levelUnlock;
        }
        private void UpdateUILeague()
        {
            leagueImg.sprite = LeaderboardConfig.GetSpirteCurrentLeague();
            leagueImgText.sprite = LeaderboardConfig.GetSpriteCurrentTextLeague();
            leagueImgText.SetNativeSize();
        }

        public void UpdateUI()
        {
            avatar.sprite = LeaderboardConfig.avatarSprites[LeaderboardData.avatarId];
            userNameTxt.text = LeaderboardData.myData.name;
            //if (LeaderboardData.medalMultiply <= 1)
            //{
            //    timeX2MedalTxt.text = "30min";
            //    medalX2Btn.interactable = true;
            //}
            //else
            //{
            //    medalX2Btn.interactable = false;
            //}
            //MeoMew temporary disable
            medalX2Btn.interactable = false;
        }

        public void Init()
        {
            leagueData = LeaderboardConfig.GetLeagueRankData(LeaderboardData.currentRankView);
            allUsers = LeaderboardData.GetAllUsers();
            if (scroller.Delegate == null)
            {
                scroller.Delegate = this;
            }
            else
            {
                LoadData();
            }
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var data = allUsers[dataIndex];
            int type = GetTypeByRank(data.rank);
            bool isActiveUpObj = leagueData.countRankUp > 0 && dataIndex == leagueData.countRankUp - 1;
            bool isActiveDownObj = leagueData.countRankDown > 0 && dataIndex == allUsers.Count - leagueData.countRankDown - 1;
            RankItem cellView;
            if (data.isPlayer)
            {
                cellView = scroller.GetCellView(playerRankItem) as RankItem;                
            }
            else
            {
                cellView = scroller.GetCellView(botRankItem) as RankItem;
            }

            var rewards = LeaderboardController.GetRewardDataByRank(dataIndex + 1);
            cellView.Init(data, type);
            cellView.SetRewardData(rewards);
            cellView.SetActiveRankUpObj(isActiveUpObj);
            cellView.SetActiveRankDownObj(isActiveDownObj);
            return cellView;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            if (leagueData.countRankUp > 0)
            {
                if (dataIndex == leagueData.countRankUp - 1)
                {
                    return 291;
                }
            }
            if (leagueData.countRankDown > 0)
            {
                if (dataIndex == allUsers.Count - leagueData.countRankDown - 1)
                {
                    return 291;
                }
            }
            if (allUsers[dataIndex].isPlayer)
            {
                return 192;
            }
            return 162;
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return allUsers.Count;
        }

        public void LoadData()
        {
            scroller.RefreshActiveCellViews();
        }

        public void ReloadData()
        {
            scroller.ReloadData();
            JumpToPlayer();
        }
        public void JumpToLast()
        {
            //int packNum = Game.isHavePacks ? (PlayerData.isBoughtStarterPack ? dataPacks.Count - 1 : dataPacks.Count) : 0;

            //Init();
            //scroller.JumpToDataIndex(Data.Count + packNum, 0, 0, true, EnhancedScroller.TweenType.easeInOutQuart, 0.2f);
        }
        public void JumpToFirst()
        {
            Init();
            scroller.JumpToDataIndex(0, 0, 0, true, EnhancedScroller.TweenType.easeInOutQuart, 0.2f);
        }
        private void JumpToPlayer()
        {
            Init();
            int index = allUsers.FindIndex(x => x.isPlayer);
            scroller.JumpToDataIndex(index, 0.5f, 0, true, EnhancedScroller.TweenType.easeInOutQuart, 0.2f);
        }
        private int GetTypeByRank(int rank)
        {
            if (rank <= leagueData.countRankUp)
                return 0;
            if (rank <= leagueData.countRankUp + leagueData.countSafe)
                return 1;
            return 2;
        }
        private void Update()
        {
            if (LeaderboardData.medalMultiply > 1)
            {
                var time = DateTime.Now - LeaderboardData.timeStartX2Medal;
                TimeSpan timeRemain = new TimeSpan(0, 30, 0) - time;
                timeX2MedalTxt.text = new DateTime(timeRemain.Ticks).ToString("mm:ss");
            }
            timeRemainTxt.text = LeaderboardController.GetTimeRemainText();
            var timeE = LeaderboardController.GetTimeLeaderboardRemain();
            if (timeE <= TimeSpan.Zero)
            {
                LeaderboardController.CheckEndTournamentAndInit();
            }
        }
    }
}