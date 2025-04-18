using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;
using UnityEngine.UI;
using _IECModules;
using DG.Tweening;
using TMPro;


public class HomeScreenTab : ScreenNavigateUI
{
    public RectTransform rightButtonBar, leftButtonBar;
    [Header("Avatar Button")] 
    public Button avatarButton;
    public Image avatarImage;
    [Header("LuckyWheel")] 
    public Button luckyWheel;
    public Image luckyWheelNotiImage;
    [Header("NoAdsPack")] 
    public Button noAdsPack;
    [Header("LifeOfferPack")] 
    public Button lifeOfferPack;
    public Image lifeOfferNotiImage; 
    [Header("Started Package")] 
    public Button startedPackageBtn;
    [Header("Piggy Bank")] 
    public Button btnPiggyBank;
    public GameObject goPiggyBankLock;
    public GameObject goPiggyBankUnlock;
    public Image piggyBankProgressBarFill;
    public Image piggyBankNotiImage;
    public TextMeshProUGUI piggyBankValue;
    [Header("Daily Reward")] 
    public Button btnDailyReward;
    public Image dailyRewardNotiImage;
    [Header("Progress Reward")] 
    public Button progressRewardButton;
    public ProgressRewardProgressBar progressRewardBar;
    [Header("Level Chest Reward")] 
    public Button btnLevelChestReward;
    [Header("Star Chest Reward")] 
    public Button btnStarChestReward;

    public GameObject starChestRewardLock;
    public GameObject starChestRewardUnlock;
    [Header("Play")]
    public RectTransform bottomBar;
    public Button playButton;
    public TextMeshProUGUI playButtonText;
    public Button playHardButton;
    public TextMeshProUGUI playHardButtonText;

    [Header("City Build")] 
    public Button buildButton;
    public BuildCity buildCityUI;
    public Image notiCityBuild;
    private List<UIMoveOutAnimation> _listObjMoveOut = new();
    private Sequence _uiAnimSequence;
    private GameConfig m_gameConfig => Pool.Get<GameConfig>();

    // Start is called before the first frame update
    void Start()
    {
        SetEvent(avatarButton,
            () =>
            {
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupAvatar.Show();
            });      
        SetEvent(luckyWheel,
            () =>
            {
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupLuckyWheel.Show();
            });       
        SetEvent(noAdsPack,
            () =>
            {
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupNoAds.Show();
            });       
        SetEvent(lifeOfferPack,
            () =>
            {
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupLifeOffer.Show();
            });       
        SetEvent(startedPackageBtn,
            () =>
            {
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupStarterPack.Show();
            });        
        SetEvent(btnPiggyBank,
            () =>
            {
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupPiggyBank.Show();
            });
        SetEvent(btnDailyReward,
            () =>
            {
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupDailyReward.Show();
            });
        SetEvent(progressRewardButton,
            () =>
            {
                Locator.Main.PopupProgressReward.Show();
            });
        SetEvent(btnLevelChestReward,
            () =>
            {
                Locator.Main.PopupLevelChestReward.Setup(
                    Config.allChestConfig.GetChestConfig(ChestType.Level).GetRewardData(PlayerData.GetChestMilestone(ChestType.Level)), 
                    PlayerData.GetChestProgress(ChestType.Level));
                Locator.Main.PopupLevelChestReward.Show();
            });
        SetEvent(btnStarChestReward,
            () =>
            {
                bool isEnoughLevel = PlayerData.level >= m_gameConfig.allChestConfig.levelUnlock;
                if(!isEnoughLevel) return;
                Locator.Main.PopupStarChestReward.Setup(
                    Config.allChestConfig.GetChestConfig(ChestType.Star).GetRewardData(PlayerData.GetChestMilestone(ChestType.Star)), 
                    PlayerData.GetChestProgress(ChestType.Star));
                Locator.Main.PopupStarChestReward.Show();
            });
        SetEvent(playButton,
            () =>
            {
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
        
        SetEvent(buildButton,
            () =>
            {
                DoMoveOutAllObj(0.3f,
                    () =>
                    {
                        Locator.Main.HomeScreenTab.buildCityUI.Show();
                    });
            });
        
        InitMoveOutAnimation();
        UpdateMainHome();
        
        EventController.RaiseEvent(EventList.Audio.BACKGROUND_HOME);
                
        DoMoveBackAllObj();
    }
    
    #region Change Screen Anim
        
        private void InitMoveOutAnimation()
        {
            _listObjMoveOut = new List<UIMoveOutAnimation>();
            _listObjMoveOut.Add(new UIMoveOutAnimation(leftButtonBar,UIElementMoveDir.LEFT)); 
            
            _listObjMoveOut.Add(new UIMoveOutAnimation(rightButtonBar,UIElementMoveDir.RIGHT));
            
            _listObjMoveOut.Add(new UIMoveOutAnimation(bottomBar,UIElementMoveDir.DOWN));
            
            foreach (RectTransform child in Locator.Main.ScreenMain.bottomBar.transform)
            {
                _listObjMoveOut.Add(new UIMoveOutAnimation(child,UIElementMoveDir.DOWN));
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
            progressRewardButton.transform.localScale = new Vector3(0, 1, 1);
        }
        
        public void DoMoveOutAllObj(float timeAnim,Action onComplete = null, float delay = 0f)
        {
            DOTween.Kill(_uiAnimSequence);

            _uiAnimSequence = DOTween.Sequence();

            _uiAnimSequence.InsertCallback(delay,
                () =>
                {
                    for (int i = 0; i < _listObjMoveOut.Count; i++)
                    {
                        DOTween.Kill($"MV_T{_listObjMoveOut[i].target.GetInstanceID()}");
                        _listObjMoveOut[i].MoveOut($"MV_T{_listObjMoveOut[i].target.GetInstanceID()}",timeAnim);
                    }
                    
                    Locator.Service.GameResourceDisplay.DoMoveOutAnim();
                });


            delay += 0.1f;
            
            _uiAnimSequence.InsertCallback(delay,
                () =>
                {
                    DOTween.Kill(progressRewardButton.transform);
                    progressRewardButton.transform.DOScaleX(0, 0.2f).SetEase(Ease.InQuad);
                });
            
            delay += 0.4f;
            //
            // _uiAnimSequence.InsertCallback(delay,
            //     () =>
            //     {
            //         buildCityUI.HideScale();
            //     });


            // delay += 0.6f;
            
            _uiAnimSequence.InsertCallback(delay,
                () =>
                {
                    onComplete?.Invoke();
                });

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
            progressRewardButton.transform.localScale = new Vector3(0, 1, 1);

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
                    progressRewardButton.transform.DOScaleX(1, 0.2f).SetEase(Ease.OutQuad);
                });

            delay += 0.3f;
            
            _uiAnimSequence.InsertCallback(delay,
                () =>
                {
                    for (int i = 0; i < _listObjMoveOut.Count; i++)
                    {
                        _listObjMoveOut[i].SetOutMoveBack($"MV_B{_listObjMoveOut[i].target.GetInstanceID()}",0.2f);
                    }
                    
                    Locator.Service.GameResourceDisplay.DoMoveBackAnim();
                });
            
            delay += 0.3f;

            _uiAnimSequence.InsertCallback(delay,
                () =>
                {
                    onComplete?.Invoke();
                });

            _uiAnimSequence.Play();
        }
        
        #endregion

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        //UpdateMainHome();

    }
    
    public override void Show()
    {
        UpdateMainHome();
        
    }
    public void UpdatePiggyBankButton()
    {
        bool isEnoughLevel = PlayerData.level >= Config.dataPiggyBankConfig.levelUnlocked;
        goPiggyBankLock.SetActive(!isEnoughLevel);
        goPiggyBankUnlock.SetActive(isEnoughLevel);
        piggyBankProgressBarFill.fillAmount = Locator.Main.PopupPiggyBank.PiggyBankFillAmount;
        luckyWheel.gameObject.SetActive(Locator.Main.PopupPiggyBank.IsFull && !PlayerData.dailyShowPiggyBank);
        piggyBankValue.text = PlayerData.piggyBankGold.ToString();
        if (Locator.Main.PopupPiggyBank.IsFull && !PlayerData.wasShowPiggyBankFirstTime)
        {
            PlayerData.wasShowPiggyBankFirstTime = true;
            btnPiggyBank.onClick.Invoke();
        }
        //piggyBankGoldGroup.UpdateGold(PlayerData.piggyBankGold);
    }

    public void UpdateBuildCity()
    {
        buildButton.gameObject.SetActive(PlayerData.level>= Config.buildCityConfig.levelUnlock);
        notiCityBuild.gameObject.SetActive(PlayerData.screwCityBuild > 0 && PlayerData.currentCityBuild < Config.buildCityConfig.dataCity.Count);
        //buildCityUI.ShowScale();
    }
    public void UpdateMainHome()
    {
        avatarImage.sprite = Config.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName);
        
        noAdsPack.gameObject.SetActive(!PlayerData.isNoAds);
        startedPackageBtn.gameObject.SetActive(!PlayerData.isBoughtStarterPack);
        UpdateDailyReward();
        UpdateLuckyWheel();
        UpdatePiggyBankButton();
        UpdateBuildCity();
        UpdateLifeOffer();
        UpdateStarChest();
        progressRewardBar.Setup();
        playButtonText.text = "Level " + PlayerData.level;
        playHardButtonText.text = "Level " + PlayerData.level;
        LeaderboardController.StartTournament();//Once when start home game
        playButton.gameObject.SetActive(!PlayerData.IsHardLevel());
        playHardButton.gameObject.SetActive(PlayerData.IsHardLevel());
        EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI);
    }

    private void UpdateStarChest()
    {
        bool isEnoughLevel = PlayerData.level >= m_gameConfig.allChestConfig.levelUnlock;
        starChestRewardLock.gameObject.SetActive(!isEnoughLevel);
        starChestRewardUnlock.gameObject.SetActive(isEnoughLevel);
    }
    private void UpdateLifeOffer()
    {
        lifeOfferNotiImage.gameObject.SetActive(PlayerData.playerLife <= 0 && !PlayerData.dailyShowLifeOfferPack);
    }
    private void UpdateLuckyWheel()
    {
        int levelUnlock = Config.dataRewardLuckyWheelConfig.levelUnlock;
        luckyWheel.gameObject.SetActive(PlayerData.level >= levelUnlock);
        luckyWheelNotiImage.gameObject.SetActive(PlayerData.luckWheelDailyAdsSpin < Config.dataRewardLuckyWheelConfig.numberSpinAdsEachDay);
    }
    private void UpdateDailyReward()
    {
        dailyRewardNotiImage.gameObject.SetActive(Locator.Main.PopupDailyReward.RewardAvailable);
        btnDailyReward.gameObject.SetActive(PlayerData.level >= Config.dataDailyRewardConfig.levelUnlock);
    }

}
