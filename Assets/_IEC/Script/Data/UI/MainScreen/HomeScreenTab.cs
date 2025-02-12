using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;
using UnityEngine.UI;
using _IECModules;
using TMPro;


public class HomeScreenTab : ScreenNavigateUI
{
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
    }

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
    }
    
}
