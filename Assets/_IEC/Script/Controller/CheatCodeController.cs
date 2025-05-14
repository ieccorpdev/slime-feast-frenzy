using System;
using UnityEngine;
using _IECModules;
using _IEC.Features.Gameplay;

namespace IEC
{
    public class CheatCodeController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart, IEventUpdate
    {
        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.NONE:
                    Debug.Log("event none");
                    break;
            }
        }

        public void BehaviourAwake()
        {
        }

        public void BehaviourStart()
        {
            ModuleController.cheatCode.positionActivate = CheatCode.PositionActivate.TopLeft;
            ModuleController.cheatCode.topOffset = 0f;
            Reporter.topOffset = 0f;
            ShowFPS.topOffset = 0f;

            ModuleController.cheatCode.AddCheatCodeButton("RESET Player Data!!!!!", cheatCodeParam =>
            {
                Game.isDebugReset = true;
                PlayerPrefs.DeleteAll();
                Application.Quit();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            });

            ModuleController.cheatCode.AddCheatCodeButton("RESET Player Data but same AB test", cheatCodeParam =>
            {
                Game.isDebugReset = true;
                Services.DataSecure().SetString(DataController.PLAYER_DATA_KEY, string.Empty);
                Services.DataSecure().Save();
                Application.Quit();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            });

            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Timer (Second)", param =>
            {
                GameplayTimer timer = Locator.Game.ScreenInGame.GetGameplayTimer();
                if (timer != null)
                {
                    timer.CurrentTime = int.Parse(param.inputParams[0].value.ToString());
                }
            }, new CheatCode.InputParam
            {
                value = 0
            });

            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Level", param =>
            {
                int level = int.Parse(param.inputParams[0].value.ToString());
                PlayerData.level = level;
                EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
            }, new CheatCode.InputParam
            {
                value = PlayerData.level
            });

            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set bonus combo", param =>
            {
                int count = int.Parse(param.inputParams[0].value.ToString());
                Locator.Game.ScreenInGame.comboUI.bonusCombo = count;
            }, new CheatCode.InputParam
            {
                value = 1
            });

            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Level Hard with id", param =>
            {
                int level = int.Parse(param.inputParams[0].value.ToString());
                if (Game.isUsingHardLevel)
                {
                    PlayerData.currentHardLevel = level;
                    GameController.OpenScene(GameScene.Game,
                        () => { EventController.RaiseEvent(EventList.GamePlay.NEW_GAME_HARD_LEVEL); },
                        false);
                }
            }, new CheatCode.InputParam
            {
                value = 0
            });
            
            ModuleController.cheatCode.AddCheatCodeButton("Set Level Hard", param =>
            {
                if (Game.isUsingHardLevel)
                {
                    GameController.OpenScene(GameScene.Game,
                        () => { EventController.RaiseEvent(EventList.GamePlay.NEW_GAME_HARD_LEVEL); },
                        false);
                }
            });
            
            ModuleController.cheatCode.AddCheatCodeButton("Set Next Level Hard", param =>
            {
                if (Game.isUsingHardLevel)
                {
                    PlayerData.currentHardLevel++;
                    GameController.OpenScene(GameScene.Game,
                        () => { EventController.RaiseEvent(EventList.GamePlay.NEW_GAME_HARD_LEVEL); },
                        false);
                }
            });
            
            ModuleController.cheatCode.AddCheatCodeButton("Spawn Level Test All Item", cheatCodeParam =>
            {
                Locator.Service.GameResourceDisplay.SetAllObjPosOut();
                GameController.OpenScene(GameScene.Game,
                    () => { EventController.RaiseEvent(EventList.GamePlay.NEW_GAME_LEVEL_ALL_ITEM); },
                    false);
            });
            
            ModuleController.cheatCode.AddCheatCodeButton("Check Valid All Level", cheatCodeParam =>
            {
                Config.levelConfig.CheckValidAllLevel();
            });
            
            ModuleController.cheatCode.AddCheatCodeButton("Set LEVEL_WIN", cheatCodeParam =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN);
            });

            ModuleController.cheatCode.AddCheatCodeButton("Set LEVEL_LOSE", cheatCodeParam =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_LOSE);
            });

            ModuleController.cheatCode.AddCheatCodeButton("Toggle NoAds", cheatCodeParam =>
            {
                var data = Pool.Get<ServiceData>();
                Services.ActiveNoAds(!data.isNoAds);
                data.isNoAdsReward = data.isNoAds;

                cheatCodeParam.buttonText = "NoAds: " + data.isNoAds;
            });
            
            ModuleController.cheatCode.AddCheatCodeButton("Send Feedback", cheatCodeParam =>
            {
                Services.SendEmail();
            });

            ModuleController.cheatCode.AddCheatCodeButton("Add Level Chest ( On Home)", param =>
            {
                AddChestProgress( ChestType.Level,1); 
            });
            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Add Star Chest ( On Home)", param =>
            {
                int gc = int.Parse(param.inputParams[0].value.ToString());
                AddChestProgress( ChestType.Star,gc);
            }, new CheatCode.InputParam
            {
                value = 1
            });  
            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Add Ocean Gift ( On Home)", param =>
            {
                //Check progress reward
                int gc = int.Parse(param.inputParams[0].value.ToString());
                PlayerData.AddProgressToProgressReward(gc);
                var reward = Config.progressRewardConfig.GetRewardData(PlayerData.currentProgressMilestone);
                while (PlayerData.CheckClaimProgressReward(reward))
                {
                    System.Collections.Generic.List<Reward> listReward = new() {reward.reward};
                    Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(listReward, null,true);
                    Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel("Chest_1");
                    reward = Config.progressRewardConfig.GetRewardData(PlayerData.currentProgressMilestone);
                }
                Locator.Main.HomeScreenTab.UpdateMainHome();
                Locator.Main.HomeScreenTab.progressRewardBar.DoAnimProgressBar();
            }, new CheatCode.InputParam
            {
                value = 1
            });    
            ModuleController.cheatCode.onCheatCodeOpen.AddListener(() =>
            {
                ModuleController.cheatCode.AddCheatCodeFromObject("Game", Game, () =>
                {
                    EventController.RaiseEvent(EventList.GamePlay.APPLY_AB_TEST);
                });
            });

            ModuleController.cheatCode.onCheatCodeStateChange.AddListener(() =>
            {
                if (ModuleController.cheatCode.showCheatCodeState == CheatCode.ShowCheatCodeState.ShowPasswordInput)
                {
                    Services.PushEvent(new TrackingEvent.OpenCheat());
                }
                else if (ModuleController.cheatCode.showCheatCodeState == CheatCode.ShowCheatCodeState.ShowCheatCode)
                {
                    Services.PushEvent(new TrackingEvent.UnlockCheat());
                }
            });
            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Add xp Hole", param =>
            {
                int gc = int.Parse(param.inputParams[0].value.ToString());
                GamePlay.levelController.GetHolePlayer().AddXp(gc);
            }, new CheatCode.InputParam
            {
                value = 0
            });            
            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Add Piggy Bank Gold", param =>
            {
                int gc = int.Parse(param.inputParams[0].value.ToString());
                Locator.Main.PopupPiggyBank.AddGoldToPiggyBank(gc);
                Locator.Main.PopupPiggyBank.UpdatePopup();
                Locator.Main.HomeScreenTab.UpdateMainHome();
            }, new CheatCode.InputParam
            {
                value = 0
            });
            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Add Player Gold", param =>
            {
                int gc = int.Parse(param.inputParams[0].value.ToString());
                PlayerData.AddCurrency(ResourceType.Gold, gc);
            }, new CheatCode.InputParam
            {
                value = 500
            });
            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Add Brick Build City", param =>
            {
                int gc = int.Parse(param.inputParams[0].value.ToString());
                PlayerData.screwCityBuild += gc;
            }, new CheatCode.InputParam
            {
                value = 500
            });

            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set UnlimitedLife Time ( Minutes)", param =>
            {
                int gc = int.Parse(param.inputParams[0].value.ToString());
                PlayerData.SetTimeUnlimitedLife(gc);
            }, new CheatCode.InputParam
            {
                value = 10
            });
            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Player Life", param =>
            {
                int gc = int.Parse(param.inputParams[0].value.ToString());
                PlayerData.playerLife = gc;
            }, new CheatCode.InputParam
            {
                value = 5
            });
            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Time Next Life", param =>
            {
                int gc = int.Parse(param.inputParams[0].value.ToString());
                PlayerData.SetTimeToNextLife(gc);
            }, new CheatCode.InputParam
            {
                value = 5
            });
            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Time Left StartedPack NoAds (s)", param =>
            {
                int time = int.Parse(param.inputParams[0].value.ToString());
                PlayerData.timeEndStaredPack = DateTime.Now.AddMinutes(time);
            }, new CheatCode.InputParam
            {
                value = (PlayerData.timeEndStaredPack - DateTime.Now).Seconds
            });
        }

        private static void AddChestProgress(ChestType type,int number)
        {
            PlayerData.AddChestProgress(Config.allChestConfig.GetChestConfig(type), number, out var isClaimChestReward);
            if (isClaimChestReward)
            {
                var rewardData = Config.allChestConfig.GetChestConfig(type)
                    .GetRewardData(PlayerData.GetChestMilestone(type));
                Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(rewardData.listReward, null,true);
                Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel(type == ChestType.Level ? "Chest_2": "Chest_1");
            }

            Locator.Main.HomeScreenTab.UpdateMainHome();
        }

        public void BehaviourUpdate(float deltaTime)
        {
            if (!Application.isMobilePlatform)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (!GamePlay.IsWin) PlayerData.level = Mathf.Max(1, PlayerData.level + 1);
                    EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    PlayerData.level = Mathf.Max(1, PlayerData.level - 1);
                    EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
                }
            }
        }
    }
}