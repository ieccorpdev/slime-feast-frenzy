using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEC;
using UnityEngine;
using UnityEngine.UI;
using _IECModules;
using DG.Tweening;
using Sirenix.OdinInspector;
using IEC.Common.Event;
using TMPro;
using EventType = IEC.EventType;

public class HomeScreenTab : ScreenNavigateUI
{
    public RectTransform rightButtonBar, leftButtonBar;
    [Header("NoAdsPack")] public Button noAdsPack;
    [Header("LifeOfferPack")] public Button lifeOfferPack;
    public Image lifeOfferNotiImage;
    [Header("Started Package")] public Button startedPackageBtn;
    [Header("Piggy Bank")] public Button btnPiggyBank;
    public GameObject goPiggyBankLock;
    public GameObject goPiggyBankUnlock;
    public Image piggyBankProgressBarFill;
    public Image piggyBankNotiImage;
    public TextMeshProUGUI piggyBankValue;
    [Header("Daily Reward")] public Button btnDailyReward;
    public Image dailyRewardNotiImage;
    public DailyRewardHomeButton dailyRewardHomeButton;
    [Header("Progress Reward")] public Button progressRewardButton;
    public ProgressRewardProgressBar progressRewardBar;
    [Header("Level Chest Reward")] public Button btnLevelChestReward;
    public BaseHomeFeatureButton levelChestHomeFeature;
    [Header("Star Chest Reward")] public Button btnStarChestReward;
    public BaseHomeFeatureButton starChestHomeFeature;
    [Header("Star Chase Event")] 
    public GameEventHomeButton starChaseButton;
    [Header("Shark Race Event")] 
    public GameEventHomeButton sharkRaceButton;
    [Header("Chomp Off Event")] 
    public GameEventHomeButton chompOffButton;
    [Header("Relic Hunt")] 
    public Button btnRelicHunt;
    public RelicButtonEvent RelicHuntEvent;
    [Space(10)]

    [Space(10)] public GameObject starChestRewardLock;
    public GameObject starChestRewardUnlock;
    [Header("Play")] public RectTransform bottomBar;
    public Button playButton;
    public TextMeshProUGUI playButtonText;
    public Button playHardButton;
    public TextMeshProUGUI playHardButtonText;

    [Header("City Build")] public Button buildButton;
    public BuildCity buildCityUI;
    public Image notiCityBuild;
    private List<UIMoveOutAnimation> _listObjMoveOut = new();
    private Sequence _uiAnimSequence;
    private GameConfig m_gameConfig => Pool.Get<GameConfig>();

    private Action onOpenSceneActionOnce;

    // Start is called before the first frame update
    void Start()
    {
        SetEvent(noAdsPack,
            () =>
            {
                Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("click_No_Ads_Pack"));
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupNoAds.Show();
            });
        SetEvent(lifeOfferPack,
            () =>
            {
                Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("click_Life_Offer_pack"));
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupLifeOffer.Show();
            });
        SetEvent(startedPackageBtn,
            () =>
            {
                Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("click_Started_Pack"));
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupStarterPack.Show();
            });
        SetEvent(btnPiggyBank,
            () =>
            {
                Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("click_Piggy_pack"));
                bool isEnoughLevel = PlayerData.level >= Config.dataPiggyBankConfig.levelUnlocked;
                if (!isEnoughLevel) return;
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupPiggyBank.Show();
            });
        SetEvent(btnDailyReward,
            () =>
            {
                Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("click_Daily_Reward"));
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupDailyReward.Show();
            });
        SetEvent(progressRewardButton,
            () =>
            {
                Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("click_Ocean_Gift"));
                Locator.Main.PopupProgressReward.Show();
            });
        SetEvent(btnLevelChestReward,
            () =>
            {
                Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("click_Star_Chest"));
                Locator.Main.PopupLevelChestReward.Setup(
                    Config.allChestConfig.GetChestConfig(ChestType.Level)
                        .GetRewardData(PlayerData.GetChestMilestone(ChestType.Level)),
                    PlayerData.GetChestProgress(ChestType.Level));
                Locator.Main.PopupLevelChestReward.Show();
            });
        SetEvent(btnStarChestReward,
            () =>
            {
                // bool isEnoughLevel = PlayerData.level >= m_gameConfig.allChestConfig.levelUnlock;
                // if(!isEnoughLevel) return;
                Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("click_Level_Chest"));
                Locator.Main.PopupStarChestReward.Setup(
                    Config.allChestConfig.GetChestConfig(ChestType.Star)
                        .GetRewardData(PlayerData.GetChestMilestone(ChestType.Star)),
                    PlayerData.GetChestProgress(ChestType.Star));
                Locator.Main.PopupStarChestReward.Show();
            });
        SetEvent(playButton,
            () =>
            {
                OnClickPlayButton();
            });

        SetEvent(playHardButton,
            () =>
            {
                if (PlayerData.playerLife <= 0 && !PlayerData.IsInUnlimitedLife())
                {
                    Locator.Main.PopupOutOfLive.Show();
                }
                else
                {
                    Locator.Main.PopupHardLevel.Show();
                }
            });

        //SetEvent(buildButton,
        //    () =>
        //    {
        //        DoMoveOutAllObj(0.3f,
        //            () =>
        //            {
        //                Locator.Main.HomeScreenTab.buildCityUI.Show();
        //            });
        //    });

        SetEvent(starChaseButton.button,
            () =>
            {
                Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("click_Star_Chase"));
                if (Locator.Main.PopupStarChase.CheckShowStartPopup())
                {
                    Locator.Main.PopupStartStarChaseEvent.Show();
                }
                else
                {
                    Locator.Main.PopupStarChase.Show();
                }
            });
        SetEvent(sharkRaceButton.button,
            () =>
            {
                Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("click_Shark_Chase"));
                if (Locator.Main.PopupSharkRace.CheckShowStartPopup())
                {
                    Locator.Main.PopupStartSharkRaceEvent.Show();
                }
                else
                {
                    Locator.Main.PopupSharkRace.Show();
                }
            });
        SetEvent(chompOffButton.button,
            () =>
            {
                Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("click_Chomp_Off"));
                // if (Locator.Main.PopupChompOff.CheckShowStartPopup())
                // {
                //     //Locator.Main.PopupStartSharkRaceEvent.Show();
                // }
                // else
                {
                    Locator.Main.PopupChompOff.Show();
                }
            });
         SetEvent(btnRelicHunt,
            () =>
            {
                if(PlayerData.relicData.hasJoinEvent == false) PlayerData.relicData.hasJoinEvent = true;
                
                Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("click_Relic_Hunt"));
                if (Locator.Main.PopupRelicHunt.CheckShowPopup())
                {
                    Locator.Main.PopupRelicHunt.Show();
                }
            });
        SetupUIHome();
        gameObject.OnEvent(EventList.GamePlay.UPDATE_GAME_EVENT_UI, CheckAllEventsAvailable);
    }

    public void OnClickPlayButton()
    {
        //Block play button when have reward on way claim
        if (Locator.Main.ScreenMain.listClaimRewardData.Count > 0 ||
            Locator.Main.ClaimBonusRewardPanel.IsHaveShowClaimRewardAnim()) return;
        if (PlayerData.playerLife <= 0 && !PlayerData.IsInUnlimitedLife())
        {
            Locator.Main.PopupOutOfLive.Show();
        }
        else
        {
            //EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
            // DoMoveOutAllObj(0.3f,
            //     () =>
            //     {
            //         Services.PushEvent(new IEC.TrackingEvent.ClickPlayHomeScreen(PlayerData.GetPlayerCurrencyCount(ResourceType.Gold).ToString()));
            //         GameController.OpenScene(GameScene.Game);
            //     });

            //Services.PushEvent(new IEC.TrackingEvent.ClickPlayHomeScreen(PlayerData.GetPlayerCurrencyCount(ResourceType.Gold).ToString()));
            Locator.Main.PopupBooster.Show();
        }
    }

    public void SetupUIHome()
    {
        InitMoveOutAnimation();
        UpdateMainHome();
        EventController.RaiseEvent(EventList.Audio.BACKGROUND_HOME);
        DoMoveBackAllObj(0.8f,
            () =>
            {
                if (onOpenSceneActionOnce == null)
                {
                    CheckAllEventsClaimableReward();
                    if (!PlayerData.isShowPackPopupsInHomeOnStartGame)
                    {
                        PlayerData.isShowPackPopupsInHomeOnStartGame = true;
                        CheckAllPacksPopupOnStartHome();
                    }

                    ShowNextPopup();
                }
                else
                {
                    onOpenSceneActionOnce.Invoke();
                    onOpenSceneActionOnce = null;
                }

            });
    }

    #region Change Screen Anim

    private void InitMoveOutAnimation()
    {
        _listObjMoveOut = new List<UIMoveOutAnimation>();
        _listObjMoveOut.Add(new UIMoveOutAnimation(leftButtonBar, UIElementMoveDir.LEFT));

        _listObjMoveOut.Add(new UIMoveOutAnimation(rightButtonBar, UIElementMoveDir.RIGHT));

        _listObjMoveOut.Add(new UIMoveOutAnimation(bottomBar, UIElementMoveDir.DOWN));

        foreach (RectTransform child in Locator.Main.ScreenMain.bottomBar.transform)
        {
            _listObjMoveOut.Add(new UIMoveOutAnimation(child, UIElementMoveDir.DOWN));
        }

        // foreach (RectTransform child in Locator.Main.ScreenMain.topBar.transform)
        // {
        //     _listObjMoveOut.Add(new UIMoveOutAnimation(child,UIElementMoveDir.UP));
        // }
        // _listObjMoveOut.Add(new UIMoveOutAnimation(Locator.Main.ScreenMain.bottomBar.GetComponent<RectTransform>(),UIElementMoveDir.DOWN));
        //_listObjMoveOut.Add(new UIMoveOutAnimation(Locator.Main.ScreenMain.topBar.GetComponent<RectTransform>(),UIElementMoveDir.UP));
    }

    public void DoMoveOutAllObjNoAnim()
    {
        for (int i = 0; i < _listObjMoveOut.Count; i++)
        {
            _listObjMoveOut[i].SetObjPosOut();
        }

        progressRewardButton.transform.localScale = new Vector3(0, 0, 1);
    }

    public void DoMoveOutAllObj(float timeAnim, Action onComplete = null, float delay = 0f)
    {
        DOTween.Kill(_uiAnimSequence);

        _uiAnimSequence = DOTween.Sequence();

        _uiAnimSequence.InsertCallback(delay,
            () =>
            {
                for (int i = 0; i < _listObjMoveOut.Count; i++)
                {
                    DOTween.Kill($"MV_T{_listObjMoveOut[i].target.GetInstanceID()}");
                    _listObjMoveOut[i].MoveOut($"MV_T{_listObjMoveOut[i].target.GetInstanceID()}", timeAnim);
                }

                Locator.Service.GameResourceDisplay.DoMoveOutAnim();
            });


        delay += 0.1f;
        _uiAnimSequence.InsertCallback(delay,
            () =>
            {
                DOTween.Kill(progressRewardButton);
                Sequence sequence = DOTween.Sequence();
                sequence.Append(progressRewardButton.transform.DOScale(new Vector3(1.1f, 1.1f, 1), 0.1f))
                    .Join(progressRewardButton.GetComponent<Image>().DOFade(0.5f, 0.1f))
                    .Append(progressRewardButton.transform.DOScale(new Vector3(0.9f, 0.9f, 1), 0.1f))
                    .Join(progressRewardButton.GetComponent<Image>().DOFade(0.3f, 0.1f))
                    .Append(progressRewardButton.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InQuad))
                    .Join(progressRewardButton.GetComponent<Image>().DOFade(0, 0.1f));
            });
        //_uiAnimSequence.InsertCallback(delay,
        //        () =>
        //        {
        //            DOTween.Kill(progressRewardButton.transform);
        //            progressRewardButton.transform.DOScaleX(0, 0.2f).SetEase(Ease.InQuad);
        //        });

        delay += 0.4f;
        //
        // _uiAnimSequence.InsertCallback(delay,
        //     () =>
        //     {
        //         buildCityUI.HideScale();
        //     });


        // delay += 0.6f;

        _uiAnimSequence.InsertCallback(delay,
            () => { onComplete?.Invoke(); });

        _uiAnimSequence.Play();
    }

    public void DoMoveBackAllObj(float delayTime = 0f, Action onComplete = null)
    {
        DOTween.Kill(_uiAnimSequence);

        foreach (var obj in _listObjMoveOut)
        {
            obj.SetObjPosOut();
        }

        //buildCityUI.HideScale(false);
        progressRewardButton.transform.localScale = new Vector3(0, 0, 1);

        _uiAnimSequence = DOTween.Sequence();

        float delay = delayTime;

        // _uiAnimSequence.InsertCallback(delay,
        //     () =>
        //     {
        //         //buildCityUI.ShowScale();
        //         buildCityUI.InitBuildCity();
        //     });
        //
        // delay += 0.2f;
        _uiAnimSequence.InsertCallback(delay,
            () =>
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(progressRewardButton.transform.DOScale(new Vector3(1.1f, 1.1f, 1), 0.2f))
                    .Join(progressRewardButton.GetComponent<Image>().DOFade(1, 0.2f))
                    .Append(progressRewardButton.transform.DOScale(new Vector3(1, 1, 1), 0.1f).SetEase(Ease.OutQuad));
            });
        //_uiAnimSequence.InsertCallback(delay,
        //        () =>
        //        {
        //            progressRewardButton.transform.DOScaleX(1, 0.2f).SetEase(Ease.OutQuad);
        //        });

        delay += 0.1f;

        _uiAnimSequence.InsertCallback(delay,
            () =>
            {
                for (int i = 0; i < _listObjMoveOut.Count; i++)
                {
                    _listObjMoveOut[i].SetOutMoveBack($"MV_B{_listObjMoveOut[i].target.GetInstanceID()}", 0.2f);
                }

                Locator.Service.GameResourceDisplay.DoMoveBackAnim();
            });

        delay += 0.1f;

        _uiAnimSequence.InsertCallback(delay,
            () => { progressRewardBar.DoAnimProgressBar(); });

        delay += 0.3f;
        _uiAnimSequence.InsertCallback(delay,
            () =>
            {
                progressRewardBar.DoAnimProgressBar();
                onComplete?.Invoke();
            });

        _uiAnimSequence.Play();
    }

    public void SpawnCollectItemOnEndGameEffect(List<KeyValuePair<ResourceType, int>> listCollectedItems,
        Action onComplete = null)
    {
        if (listCollectedItems.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        foreach (var collectedItem in listCollectedItems)
        {
            Locator.Main.ScreenMain.AddRewardClaimEffect(collectedItem.Key, collectedItem.Value);
        }

        Locator.Main.ScreenMain.DoAnimAllAvailableReward(onComplete);
    }

    public void AddOpenSceneAction(Action onOpen = null)
    {
        onOpenSceneActionOnce = onOpen;
    }

    #endregion

    public void OnEnable()
    {
        //UpdateMainHome();
        EventController.RaiseEvent(EventList.Audio.HOMEPAGE_IDLE);
    }

    private void OnDisable()
    {
        SoundController.StopSound(EventList.Audio.HOMEPAGE_IDLE);
    }

    public override void Show()
    {
        UpdateMainHome();
    }

    public void UpdatePiggyBankButton()
    {
        btnPiggyBank.gameObject.SetActive(false);
        //bool isEnoughLevel = PlayerData.level >= Config.dataPiggyBankConfig.levelUnlocked;
        //goPiggyBankLock.SetActive(!isEnoughLevel);
        //goPiggyBankUnlock.SetActive(isEnoughLevel);
        //piggyBankProgressBarFill.fillAmount = Locator.Main.PopupPiggyBank.PiggyBankFillAmount;
        //luckyWheel.gameObject.SetActive(Locator.Main.PopupPiggyBank.IsFull && !PlayerData.dailyShowPiggyBank);
        //piggyBankValue.text = PlayerData.piggyBankGold.ToString();
        //if (Locator.Main.PopupPiggyBank.IsFull && !PlayerData.wasShowPiggyBankFirstTime)
        //{
        //    PlayerData.wasShowPiggyBankFirstTime = true;
        //    btnPiggyBank.onClick.Invoke();
        //}
        //piggyBankGoldGroup.UpdateGold(PlayerData.piggyBankGold);
    }

    private void UpdateButtonRelic()
    {
        RelicHuntEvent.UpdateUI();
    }

    public void UpdateBuildCity()
    {
        buildButton.gameObject.SetActive(PlayerData.level >= Config.buildCityConfig.levelUnlock);
        buildCityUI.gameObject.SetActive(PlayerData.level >= Config.buildCityConfig.levelUnlock);
        notiCityBuild.gameObject.SetActive(PlayerData.screwCityBuild > 0 &&
                                           PlayerData.currentCityBuild < Config.buildCityConfig.dataCity.Count);
        //buildCityUI.ShowScale();
    }

    public void UpdateMainHome()
    {
        noAdsPack.gameObject.SetActive(!PlayerData.isNoAds);
        startedPackageBtn.gameObject.SetActive(!PlayerData.isBoughtStarterPack);
        UpdateDailyReward();
        UpdateLuckyWheel();
        UpdatePiggyBankButton();
        UpdateButtonRelic();
        //UpdateBuildCity();
        UpdateLifeOffer();
        UpdateStarChest();
        UpdateGameEventButton();
        CheckAllEventsAvailable();
        progressRewardBar.Setup();
        levelChestHomeFeature.SetLock(PlayerData.level <
                                      Config.allChestConfig.GetChestConfig(ChestType.Level).levelUnlock);
        levelChestHomeFeature.SetProgressBar(
            PlayerData.GetChestProgress(ChestType.Level),
            Config.allChestConfig.GetChestConfig(ChestType.Level)
                .GetRewardData(PlayerData.GetChestMilestone(ChestType.Level)).progressToReward
        );
        starChestHomeFeature.SetLock(
            PlayerData.level < Config.allChestConfig.GetChestConfig(ChestType.Star).levelUnlock);
        starChestHomeFeature.SetProgressBar(
            PlayerData.GetChestProgress(ChestType.Star),
            Config.allChestConfig.GetChestConfig(ChestType.Star)
                .GetRewardData(PlayerData.GetChestMilestone(ChestType.Star)).progressToReward
        );
        playButtonText.text = "Level " + PlayerData.level;
        playHardButtonText.text = "Level " + PlayerData.level;
        //LeaderboardController.StartTournament();//Once when start home game
        playButton.gameObject.SetActive(!PlayerData.IsHardLevel());
        playHardButton.gameObject.SetActive(PlayerData.IsHardLevel());
        EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI);
    }

    #region Check show popups on update home
    
    Queue<PopupData> popupQueue = new Queue<PopupData>();

    public void AddPopupToShow(BasePopup popup, Action onOpen, Action onClose)
    {
        popupQueue.Enqueue(new PopupData(popup, onOpen, onClose));
    }

    public void ShowNextPopup()
    {
        if (popupQueue.Count > 0)
        {
            var popupData = popupQueue.Dequeue();
            ShowPopup(popupData.popup, popupData.onOpen, popupData.onClose);
        }
    }
    
    private void ShowPopup(BasePopup popup,Action onOpen, Action onClosed)
    {
        if (popup != null)
        {
            popup.AddActionOnceOnClose(() =>
            {
                onClosed?.Invoke(); // Called when popup is closed
            });
            onOpen?.Invoke();
        }
        else
        {
            //Custom rating thread
            onOpen?.Invoke();
            onClosed?.Invoke();
        }
    }
    
    public void CheckAllPacksPopupOnWin()
    {
        if (Config.listLevelShowPackNoAdsHome.Contains(PlayerData.level) && !PlayerData.isNoAds)
        {
            AddPopupToShow(Locator.Main.PopupNoAds,
                () =>
                {
                    Locator.Main.PopupNoAds.Show();
                },
                ShowNextPopup);
        }

    }

    public void CheckAllPacksPopupOnStartHome()
    {
        if (PlayerData.level >= Config.specialObjectConfig.kingdomSpecialObjectSpawnData.levelStartSpawn
                && !PlayerData.journeyData.isBuyIAP && PlayerData.journeyData.wasOpenJourney)
        {
            PlayerData.isShowStartedPackInStartGame = !PlayerData.isShowStartedPackInStartGame;
            if (PlayerData.isShowStartedPackInStartGame)
            {
                AddPopupToShow(Locator.Main.PopupVoyagePassIAPInfor,
                   () =>
                   {
                       Locator.Main.PopupVoyagePassIAPInfor.Show();
                   },
                   ShowNextPopup);
            }
        }
        if (!PlayerData.isBoughtStarterPack && !PlayerData.isShowStartedPackInStartGame)
        {
            AddPopupToShow(Locator.Main.PopupStarterPack,
                () =>
                {
                    Locator.Main.PopupStarterPack.Show();
                },
                ShowNextPopup);
        }
    }

    public void CheckAllEventsClaimableReward()
    {
        //queueActionsClaimReward = new();

        if (PlayerData.level > Config.AllGameEventConfig.GetGameEventConfig(EventType.StarChase).levelUnlock
            && Locator.Main.PopupStarChase.IsEndEventStage()
            && PlayerData.GetEventSaveData(EventType.StarChase).listPlayerData.Count > 0
            && !PlayerData.GetEventSaveData(EventType.StarChase).claimedReward)
        {
            AddPopupToShow(Locator.Main.PopupStarChase,
                () =>
                {
                    Locator.Main.PopupStarChase.Show();
                    Locator.Main.PopupStarChase.SetActionOnceOnClaimReward(ShowNextPopup);
                },
                null);
            // queueActionsClaimReward.Enqueue(
            //     () =>
            //     {
            //         Locator.Main.PopupStarChase.Show();
            //         Locator.Main.PopupStarChase.SetActionOnceOnClaimReward(ShowNextPopup);
            //     });
        }

        if (PlayerData.level > Config.AllGameEventConfig.GetGameEventConfig(EventType.SharkRace).levelUnlock
            && Locator.Main.PopupSharkRace.IsEndEventStage()
            && PlayerData.GetEventSaveData(EventType.SharkRace).listPlayerData.Count > 0
            && !PlayerData.GetEventSaveData(EventType.SharkRace).claimedReward)
        {
            AddPopupToShow(Locator.Main.PopupSharkRace,
                () =>
                {
                    Locator.Main.PopupSharkRace.Show();
                    Locator.Main.PopupSharkRace.SetActionOnceOnClaimReward(ShowNextPopup);
                },
                null);
            // queueActionsClaimReward.Enqueue(
            //     () =>
            //     {
            //         Locator.Main.PopupSharkRace.Show();
            //         Locator.Main.PopupSharkRace.SetActionOnceOnClaimReward(ShowNextPopup);
            //     });
        }

        if (PlayerData.level >= Config.AllGameEventConfig.GetGameEventConfig(EventType.ChompOff).levelUnlock
            && Locator.Main.PopupChompOff.IsEndEventStage()
            && PlayerData.GetEventSaveData(EventType.ChompOff).listPlayerData.Count > 0
            && !PlayerData.GetEventSaveData(EventType.ChompOff).claimedReward)
        {
            AddPopupToShow(Locator.Main.PopupSharkRace,
                () =>
                {
                    Locator.Main.PopupChompOff.Show();
                    Locator.Main.PopupChompOff.SetActionOnceOnClaimReward(ShowNextPopup);
                },
                null);
            // queueActionsClaimReward.Enqueue(
            //     () =>
            //     {
            //         Locator.Main.PopupChompOff.Show();
            //         Locator.Main.PopupChompOff.SetActionOnceOnClaimReward(ShowNextPopup);
            //     });
        }

        //ShowNextPopup();
    }

    #endregion

    public void CheckAllEventsAvailable()
    {
        int daysPlayed = PlayerData.GetDaysSinceDownload();

        //Check chomp-off
        var chompOffData = PlayerData.GetEventSaveData(EventType.ChompOff);
        int daysLastChompOff = PlayerData.GetDaysFromDownloadToTime(chompOffData.timeStart);
        int chompOffDuration = Mathf.FloorToInt(Config.AllGameEventConfig.GetGameEventConfig(EventType.ChompOff)
            .eventDurationInHours / 24);
        bool active = Config.eventLiveOpsConfig.IsEventAvailable(
            EventType.ChompOff,
            chompOffDuration,
            daysPlayed) && chompOffButton.IsEventCanShow();

        if (Locator.Main.PopupChompOff.IsResetChompOff() )
        {
            PlayerData.ResetEventSaveData(EventType.ChompOff);
        }
        
        if (chompOffData.claimedReward)
        {
            chompOffButton.gameObject.SetActive(active);
        }
    }

    private void UpdateStarChest()
    {
        // bool isEnoughLevel = PlayerData.level >= m_gameConfig.allChestConfig.levelUnlock;
        // starChestRewardLock.gameObject.SetActive(!isEnoughLevel);
        // starChestRewardUnlock.gameObject.SetActive(isEnoughLevel);
        // btnStarChestReward.interactable = isEnoughLevel;
    }

    [Button]
    private void TestRelic()
    {
        Locator.Main.PopupRelicHunt.Show();
    }

    [Button]
    private void TestAddRelic(int amount)
    {
        PlayerData.relicData.AddTokenCount(amount);
    }

    [Button]
    private void ResetRelic()
    {
        PlayerData.ResetRelicData();
    }

    private void UpdateGameEventButton()
    {
        starChaseButton.InitUI();
        sharkRaceButton.InitUI();
        chompOffButton.InitUI();
    }

    private void UpdateLifeOffer()
    {
        lifeOfferNotiImage.gameObject.SetActive(PlayerData.playerLife <= 0 && !PlayerData.dailyShowLifeOfferPack);
    }

    private void UpdateLuckyWheel()
    {
        //int levelUnlock = Config.dataRewardLuckyWheelConfig.levelUnlock;
        //luckyWheel.gameObject.SetActive(PlayerData.level >= levelUnlock);
        //luckyWheel.gameObject.SetActive(false);
        //uckyWheelNotiImage.gameObject.SetActive(PlayerData.luckWheelDailyAdsSpin < Config.dataRewardLuckyWheelConfig.numberSpinAdsEachDay);
    }

    private void UpdateDailyReward()
    {
        dailyRewardHomeButton.UpdateUI();
        //dailyRewardNotiImage.gameObject.SetActive(Locator.Main.PopupDailyReward.RewardAvailable);
        btnDailyReward.gameObject.SetActive(PlayerData.level >= Config.dataDailyRewardConfig.levelUnlock &&
                                            Game.haveDailyReward);
    }

}

public class PopupData
{
    public BasePopup popup;
    public Action onOpen;
    public Action onClose;

    public PopupData(BasePopup popup, Action onOpen = null, Action onClose = null)
    {
        this.popup = popup;
        this.onOpen = onOpen;
        this.onClose = onClose;
    }
}
