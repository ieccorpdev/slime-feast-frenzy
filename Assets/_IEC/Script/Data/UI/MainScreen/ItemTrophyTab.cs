using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using IEC.Common.Event;
using IEC.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTrophyTab : _BaseBehaviour
{
    public TrophyType type;
    public ItemTrophyState state;
    [Header("Aquarium")]
    public GameObject normalBg;
    public GameObject coralBg;
    public ParticleSystem startFx;
    public GameObject blurBG;
    public GameObject lightBg;
    public Image chestImage;
    public GameObject lockObj;
    public GameObject newsTagObj;
    public Button newsClaimButton;
    public Sprite normalChest;
    public Sprite auraChest;
    [Header("ProgressBar")]
    public GameObject progressBar;
    public GameObject hiddenBar;
    public GameObject completeBar;
    public GameObject claimBar;
    public Image mainTrophy;
    public Image fillImage;

    public TextMeshProUGUI progressText;
    [Header("Popup Region")]
    public Button infoButton;
    
    public PopupInfoGroup groupsLock;
    public TextMeshProUGUI lockTrophyNameTMP;
    public TextMeshProUGUI levelUnlockTrophyNameTMP;
    
    public PopupInfoGroup groupsNoti;
    public GameObject groupsNotiTime;
    public Image bgGroupsNoti;
    public TextMeshProUGUI infoTrophyNameTMP;
    public TextMeshProUGUI infoTrophyTMP;
    public TextMeshProUGUI timeUnlockEvent;
    
    public TrophyEventData dataEventTime;
    private bool isLocked;
    private int targetToTrophy;
    private Sprite eventInfoSprite;
    public Sprite defaultInfoSprite;
    void Start()
    {
        infoButton.onClick.AddListener(OnClickInfoTrophy);   
        gameObject.OnEvent(EventList.Trophy.CLICK_INFO_TROPHY, ResetInfo);
        eventInfoSprite = bgGroupsNoti.sprite;
    }

    public void RegisterEventButton()
    {
        newsClaimButton.onClick.RemoveAllListeners();
        newsClaimButton.onClick.AddListener(ActionClaim);
    }
    public void ActionClaim()
    {
        int trophyCollect = PlayerData.trophyData.GetCurrentCollectedTrophy(type);
        var reward = Config.trophyRewardConfig.GetReward(type).listTrophyReward[trophyCollect].listTrophyReward;
        Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(reward,
            ()=>
            {
                PlayerData.trophyData.CollectTrophyReward(type);
                Locator.Main.ScreenMain.trophyTab.LoadData();
                Locator.Service.GameResourceDisplay.ToggleDisplayAllResources(false);
            },false); 
        Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel();
    }
    public void ResetInfo()
    {
        groupsLock.gameObject.SetActive(false);
        groupsNoti.gameObject.SetActive(false);
    }

    public void OnClickInfoTrophy()
    {
        if (groupsLock.gameObject.activeInHierarchy || groupsNoti.gameObject.activeInHierarchy)
        {
            ResetInfo();
            return;
        }
        EventController.RaiseEvent(EventList.Trophy.CLICK_INFO_TROPHY);
        //Progress lock and info action
        ResetInfo();
        if (state == ItemTrophyState.Locked )
        {
            groupsLock.gameObject.SetActive(true);
            groupsLock.Show();
            OnClickLocked();
        }
        else if ( state == ItemTrophyState.Missing || PlayerData.trophyData.GetCurrentCollectedTrophy(type) == 0)
        {
            groupsNoti.gameObject.SetActive(true);
            groupsNoti.Show();
            OnClickInfoGroups();
            UpdateUIByTimeDataEvent();
        }
        else
        {   
            //Meomew fix Task #71187 
            PlayerData.trophyData.SetNotiNewsTag(type,false);
            newsTagObj.gameObject.SetActive(false);
            Locator.Main.PopupTrophyDetails.ShowTrophy(type,state,dataEventTime);
        }
    }

    public void OnClickLocked()
    {
        lockTrophyNameTMP.text = Locator.Main.ScreenMain.trophyTab.GetRealNameTrophy(type);
        levelUnlockTrophyNameTMP.text =
            "Unlock at level " + Locator.Main.ScreenMain.trophyTab.GetLevelUnlockTrophy(type);
    }
    public void OnClickInfoGroups()
    {
        infoTrophyNameTMP.text = Locator.Main.ScreenMain.trophyTab.GetRealNameTrophy(type);
        //Allway  return bronze here
        string textRS = Locator.Main.ScreenMain.trophyTab.GetTextInfoTrophyMerged(type,targetToTrophy,true);
        infoTrophyTMP.text = textRS;
    }

    public void TurnBgCoral(bool stt)
    {
        normalBg.gameObject.SetActive(!stt);
        coralBg.gameObject.SetActive(stt);
    }

    public void TurnBlur(bool stt)
    {
        blurBG.gameObject.SetActive(stt);
    }
    public void TurnLight(bool stt)
    {
        lightBg.gameObject.SetActive(stt);
    }

    public void DeactiveAllBar()
    {
        progressBar.gameObject.SetActive(false);
        hiddenBar.gameObject.SetActive(false);
        completeBar.gameObject.SetActive(false);
        claimBar.gameObject.SetActive(false);
    }
    public void UpdateStateAquarium(ItemTrophyState state)
    {
        startFx.Stop();
        groupsLock.gameObject.SetActive(false);
        groupsNoti.gameObject.SetActive(false);
        this.state = state;
        isLocked = state == ItemTrophyState.Locked;
        lockObj.gameObject.SetActive(isLocked);
        newsTagObj.gameObject.SetActive(PlayerData.trophyData.NeedShowNewsTag(type));
        TurnBgCoral(!isLocked);
        TurnBlur(!isLocked);
        TurnLight(false);
        mainTrophy.gameObject.SetActive(false);
        DeactiveAllBar();
        UpdateMainTrophy();
        chestImage.sprite = normalChest;
        mainTrophy.color = new Color(1f, 1f, 1f,1f);
        switch (state)
        {
            case ItemTrophyState.Idle:
                TurnBgCoral(true);
                mainTrophy.gameObject.SetActive(true);
                progressBar.gameObject.SetActive(true);
                break;
            case ItemTrophyState.Locked:
                hiddenBar.gameObject.SetActive(true);
                break;
            case ItemTrophyState.Missing:
                mainTrophy.gameObject.SetActive(true);
                mainTrophy.color = new Color(0.1f, 0.1f, 0.1f,1);
                progressBar.gameObject.SetActive(true);
                break;
            case ItemTrophyState.New:
                if (PlayerData.trophyData.GetCurrentCollectedTrophy(type) == 0)
                {
                    //Not claim first trophy
                    newsTagObj.gameObject.SetActive(false);
                    mainTrophy.color = new Color(0.1f, 0.1f, 0.1f,1);
                } 
                
                mainTrophy.gameObject.SetActive(true);

                claimBar.gameObject.SetActive(true);
                chestImage.sprite = auraChest;
                break;
            case ItemTrophyState.Complete:
                mainTrophy.gameObject.SetActive(true);
                completeBar.gameObject.SetActive(true);
                TurnLight(true);
                break;
        }

        CheckPlayStarFx();
    }

    public void CheckPlayStarFx()
    {
        if (PlayerData.trophyData.GetCurrentCollectedTrophy(type) >= 4)
        {
            startFx.Play();
        }
    }
    public void UpdateMainTrophy()
    {
        int curTrophyCol = PlayerData.trophyData.GetCurrentCollectedTrophy(type);
        int index = Mathf.Clamp(curTrophyCol - 1, 0, 3);
        mainTrophy.sprite = Config.trophyRewardConfig.GetReward(type).GetSpriteByIndex(index);
    }
    public void SetProgressBar(int current,int max)
    {
        progressText.text = current + "/" + max;
        fillImage.fillAmount = (float) current / (float) max;
        targetToTrophy = max - current;
    }

    public void UpdateUIByTimeDataEvent()
    {
        if (dataEventTime == null || !dataEventTime.isEvent)
        {
            bgGroupsNoti.sprite = defaultInfoSprite;
            groupsNotiTime.gameObject.SetActive(false);
            //Non event item
        }
        else
        {
            if (dataEventTime.isEventActive)
            {
                bgGroupsNoti.sprite = defaultInfoSprite;
                groupsNotiTime.gameObject.SetActive(false);
            }
            else
            {
                bgGroupsNoti.sprite = eventInfoSprite;
                groupsNotiTime.gameObject.SetActive(true);
                if (timeEventCoroutine != null)
                {
                    StopCoroutine(timeEventCoroutine);
                }
                timeEventCoroutine = StartCoroutine(IUpdateTimeEvent());
                //timeUnlockEvent.text =  DateTimeHelper.GetFormattedTimeHMS((dataEventTime.timeStartEvent - DateTime.Now));
            }
        }
    }
    private Coroutine timeEventCoroutine;
    IEnumerator IUpdateTimeEvent()
    {
        while (groupsNotiTime.gameObject.activeSelf && !dataEventTime.isEventActive)
        {
            timeUnlockEvent.text = DateTimeHelper.GetFormattedTimeHMS((dataEventTime.timeStartEvent - DateTime.Now));
            yield return new WaitForSeconds(1f);
        }
    }

    public void Update()
    {
        if (dataEventTime != null && dataEventTime.isEvent && dataEventTime.isEventActive)
        {
            timeUnlockEvent.text =  DateTimeHelper.GetFormattedTimeHMS((dataEventTime.timeStartEvent - DateTime.Now));
        }
    }

}

public enum ItemTrophyState
{
    Idle, // unlocked, at lease 1 
    Locked,// not reach condition to unlock
    Missing,// unlocked, 0
    New,//Unlocked, >=1, not claim
    Complete,//Unlock Full
}
