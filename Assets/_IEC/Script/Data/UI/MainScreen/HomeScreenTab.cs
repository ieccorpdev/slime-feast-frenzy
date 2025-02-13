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
    [Header("NoAdsPack")] 
    public Button noAdsPack;
    [Header("Started Package")] 
    public Button startedPackageBtn;
    [Header("Piggy Bank")] 
    public Button btnPiggyBank;
    public Image piggyBankProgressBarFill;
    public Image piggyBankNotiImage;
    public TextMeshProUGUI piggyBankValue;
    [Header("Daily Reward")] 
    public  Button btnDailyReward;
    public Image dailyRewardNotiImage;
    [Header("Play")] 
    public Button playButton;

    [Header("City Build")] 
    public Button buildButton;
    public BuildCity buildCityUI;
    
    private List<UIMoveOutAnimation> _listObjMoveOut = new();
    private Sequence _uiAnimSequence;
    
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
        SetEvent(playButton,
            () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
            });
        
        SetEvent(buildButton,
            () =>
            {
                DoMoveOutAllObj(
                    () =>
                    {
                        Locator.Main.HomeScreenTab.buildCityUI.Show();
                    });
            });
        
        InitMoveOutAnimation();
    }
    
    #region Change Screen Anim
        
        private void InitMoveOutAnimation()
        {
            _listObjMoveOut = new List<UIMoveOutAnimation>();
            _listObjMoveOut.Add(new UIMoveOutAnimation(leftButtonBar,UIElementMoveDir.LEFT)); 
            
            _listObjMoveOut.Add(new UIMoveOutAnimation(rightButtonBar,UIElementMoveDir.RIGHT));
            
            _listObjMoveOut.Add(new UIMoveOutAnimation(buildButton.GetComponent<RectTransform>(),UIElementMoveDir.DOWN));
            _listObjMoveOut.Add(new UIMoveOutAnimation(playButton.GetComponent<RectTransform>(),UIElementMoveDir.DOWN));


            foreach (RectTransform child in Locator.Main.ScreenMain.bottomBar.transform)
            {
                _listObjMoveOut.Add(new UIMoveOutAnimation(child,UIElementMoveDir.DOWN));
            }
            
            foreach (RectTransform child in Locator.Main.ScreenMain.topBar.transform)
            {
                _listObjMoveOut.Add(new UIMoveOutAnimation(child,UIElementMoveDir.UP));
            }
            // _listObjMoveOut.Add(new UIMoveOutAnimation(Locator.Main.ScreenMain.bottomBar.GetComponent<RectTransform>(),UIElementMoveDir.DOWN));
            //_listObjMoveOut.Add(new UIMoveOutAnimation(Locator.Main.ScreenMain.topBar.GetComponent<RectTransform>(),UIElementMoveDir.UP));
            
        }
        
        private void DoMoveOutAllObj(Action onComplete = null)
        {
            DOTween.Kill(_uiAnimSequence);

            _uiAnimSequence = DOTween.Sequence();

            float delay = 0f;

            _uiAnimSequence.InsertCallback(delay,
                () =>
                {
                    for (int i = 0; i < _listObjMoveOut.Count; i++)
                    {
                        _listObjMoveOut[i].MoveOut($"MV_T{_listObjMoveOut[i].target.GetInstanceID()}");
                    }
                });


            delay += 0.3f;
            
            _uiAnimSequence.InsertCallback(delay,
                () =>
                {
                    onComplete?.Invoke();
                });

            _uiAnimSequence.Play();
        }
        
        public void DoMoveBackAllObj(Action onComplete = null)
        {
            DOTween.Kill(_uiAnimSequence);
            
            foreach (var obj in _listObjMoveOut)
            {
                obj.SetObjPosOut();
            }

            _uiAnimSequence = DOTween.Sequence();
            
            float delay = 0f;

            _uiAnimSequence.InsertCallback(delay,
                () =>
                {
                    for (int i = 0; i < _listObjMoveOut.Count; i++)
                    {
                        _listObjMoveOut[i].SetOutMoveBack($"MV_B{_listObjMoveOut[i].target.GetInstanceID()}");
                    }
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

    private void OnEnable()
    {
        UpdateMainHome();
    }
    public void UpdatePiggyBankButton()
    {
        piggyBankProgressBarFill.fillAmount = Locator.Main.PopupPiggyBank.PiggyBankFillAmount;
        piggyBankNotiImage.gameObject.SetActive(Locator.Main.PopupPiggyBank.IsFull);
        piggyBankValue.text = PlayerData.piggyBankGold.ToString();
        //piggyBankGoldGroup.UpdateGold(PlayerData.piggyBankGold);
    }
    public void UpdateMainHome()
    {
        avatarImage.sprite = Config.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName);
        luckyWheel.gameObject.SetActive(PlayerData.level >= Config.dataRewardLuckyWheelConfig.levelUnlock);
        noAdsPack.gameObject.SetActive(!PlayerData.isNoAds);
        startedPackageBtn.gameObject.SetActive(!PlayerData.isBoughtStarterPack);
        dailyRewardNotiImage.gameObject.SetActive(Locator.Main.PopupDailyReward.RewardAvailable);
        UpdatePiggyBankButton();
        EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI);
    }
    
}
