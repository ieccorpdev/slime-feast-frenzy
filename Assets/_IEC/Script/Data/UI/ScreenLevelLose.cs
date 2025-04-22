using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Booster;
using _IEC.Features.Common;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using _IEC.Features.Gameplay.ObserverEventData;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace IEC
{
    public class ScreenLevelLose : ScreenUI
    {
        [SerializeField] private GroupLoseReason groupLoseReasonTimeUp;
        [SerializeField] private GroupLoseReasonConfirm groupLoseReasonConfirmTimeUp;
        [SerializeField] private GroupLoseReason groupLoseReasonBoom;
        [SerializeField] private GroupLoseReasonConfirm groupLoseReasonConfirmBoom;
        public GroupLevelFailed groupLevelFailed;

        private void Awake()
        {
            groupLevelFailed.OnClose += () =>
            {
                System.Action actionClose = () =>
                {
                    Hide();
                };
                
                if (Services.ShowFullAdsWithTimeAndCondition(Game.ShowFullAdsCondition, Game.timeDelayFullAds, actionClose))
                {
                    Services.PushEvent(new SFApostition("losescreen_exit"));
                }
            };

            groupLevelFailed.OnRetry += () =>
            {
                System.Action actionClose = () =>
                {
                    if (PlayerData.playerLife <= 0 && !PlayerData.IsInUnlimitedLife())
                    {
                        Hide();
                        //Back home
                        return;
                    }
                    
                    var selectedBoosters = groupLevelFailed.GetGroupBooster().GetSelectedBoosters();
                    EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
                    if (Game.isHaveBooster)
                    {
                        HandleBooster(selectedBoosters);
                    }
                    
                    Hide();
                };
                
                if (Services.ShowFullAdsWithTimeAndCondition(Game.ShowFullAdsCondition, Game.timeDelayFullAds, actionClose))
                {
                    Services.PushEvent(new SFApostition("losescreen_retry"));
                }
            };
        }

        private void HandleBooster(List<BoosterId> boosterIds)
        {
            // foreach (var selectedBooster in boosterIds)
            // {
            //     var boosterData = PlayerData.boosterData.GetDataModelById(selectedBooster);
            //     boosterData.ConsumeBooster();
            //     this.RaiseEvent(EventList.GamePlay.USE_BOOSTER, new UseBoosterEventData()
            //     {
            //         boosterId = selectedBooster
            //     });
            // }
        }

        private void Start()
        {
            SetUpForLoseReasonTimeUp();
            SetUpForLoseReasonBoom();
        }

        public override void Show()
        {
            base.Show();
        }

        public void SetupPanelShow(EventParam param)
        {
            LoseReason reason = LoseReason.Boom;
            if (param.p0 != null)
            {
                reason = (LoseReason) param.p0;
            }
            DisableAllScreen();
            SetUpLoseReason(reason);
        }
        public void SetUpLoseReason(LoseReason loseReason)
        {
            DisableAllScreen();
            switch (loseReason)
            {
                case LoseReason.None:
                    UI.ShowScreen(groupLevelFailed);
                    break;
                case LoseReason.TimeUp:
                    UI.ShowScreen(groupLoseReasonTimeUp);
                    break;
                case LoseReason.Boom:
                    UI.ShowScreen(groupLoseReasonBoom);
                    break;
            }
        }

        private void DisableAllScreen()
        {
            groupLevelFailed.gameObject.SetActive(false);
            groupLoseReasonBoom.gameObject.SetActive(false);
            groupLoseReasonConfirmBoom.gameObject.SetActive(false);
            groupLoseReasonTimeUp.gameObject.SetActive(false);
            groupLoseReasonConfirmTimeUp.gameObject.SetActive(false);
        }
        private void SetUpForLoseReasonTimeUp()
        {
            groupLoseReasonTimeUp.SetActionPlayOn(() =>
            {
                Hide();
                Locator.Game.ScreenInGame.Show();
                this.RaiseEvent(EventList.GamePlay.ADD_TIME_FOR_TIMER,new AddTimeForTimerData()
                {
                    time = Config.gamePlayConfig.reviveTimeForTimer
                });
                this.RaiseEvent(EventList.GamePlay.REVIVE_GAME);
            });
        }

        private void SetUpForLoseReasonBoom()
        {
            groupLoseReasonBoom.SetActionPlayOn(() =>
            {
                Hide();
                Locator.Game.ScreenInGame.Show();
                this.RaiseEvent(EventList.GamePlay.REVIVE_GAME);
            });
        }
    }
}