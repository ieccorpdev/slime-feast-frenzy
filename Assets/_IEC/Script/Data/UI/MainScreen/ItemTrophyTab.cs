using System.Collections;
using System.Collections.Generic;
using IEC;
using IEC.Common.Event;
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

    public GameObject blurBG;
    public GameObject lightBg;
    
    public GameObject lockObj;
    public GameObject newsTagObj;
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
    
    public GameObject groupsLock;
    public TextMeshProUGUI lockTrophyNameTMP;
    public TextMeshProUGUI levelUnlockTrophyNameTMP;
    
    public GameObject groupsNoti;
    public TextMeshProUGUI infoTrophyNameTMP;
    public TextMeshProUGUI infoTrophyTMP;
    public TextMeshProUGUI timeUnlockEvent;
    [Header("Text Custom")]
    public TMP_FontAsset fontMain;
    public Material timesMaterial;
    public Material bronzeMaterial;
    private bool isLocked;
    private int targetToTrophy;
    void Start()
    {
        infoButton.onClick.AddListener(OnClickInfoTrophy);   
        gameObject.OnEvent(EventList.Trophy.CLICK_INFO_TROPHY, ResetInfo);
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
    // Update is called once per frame
    void Update()
    {
        
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
        if (state == ItemTrophyState.New)
        {
            ActionClaim();
            return;
        }

        if (state == ItemTrophyState.Locked )
        {
            groupsLock.gameObject.SetActive(true);
            OnClickLocked();
            //

        }
        else if ( state == ItemTrophyState.Missing)
        {
            groupsNoti.gameObject.SetActive(true);
            OnClickInfoGroups();
        }
        else
        {   
            Locator.Main.PopupTrophyDetails.ShowTrophy(type,state);
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
        //groupsNoti.gameObject.SetActive(false);//Test
        infoTrophyNameTMP.text = Locator.Main.ScreenMain.trophyTab.GetRealNameTrophy(type);
        
        string textRS = "";
        string targetReach = targetToTrophy.ToString();
        string trophy = GetTrophyTarget();
        infoTrophyTMP.text = "Complete the event <material=\"{materialTimesName}\">999 Times</material>";
        switch (type)
        {
            case TrophyType.GrandVoyage:
                textRS += "Clear " + targetReach + " to get the" + trophy + "!";
                break;
            case TrophyType.SharkFrenzyForce:
                textRS += "Unlock more " + targetReach + " skin to get the" + trophy + "!";
                break;
            case TrophyType.UnderseaVoyage:
                textRS += "Complete the pass " + targetReach + " times to get the" + trophy + "!";
                break;
            case TrophyType.SharkRace:
            case TrophyType.StarRace:
                textRS += "Complete the event and make top " + targetReach + " to get the" + trophy + "!";
                break;
            case TrophyType.ChompOff:
            case TrophyType.RelicHunt:
            case TrophyType.OceanGift:
                textRS += "Complete the event  " + targetReach + " times to get the" + trophy + "!";
                break;
            
        }
        infoTrophyTMP.text = textRS;
    }

    public string GetTrophyTarget()
    {
        //    Bronze, Silver, Gold, Diamond
        if (PlayerData.trophyData.GetCurrentTimeComplete(type) == 0)
        {
            return "Bronze";
        }
        switch (PlayerData.trophyData.GetCurrentCollectedTrophy(type))
        {
            case 0:
                return "Silver";
            case 1:
                return "Gold";
            case 2:
                return "Diamond";
            default:
                return "";
        }
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
        groupsLock.gameObject.SetActive(false);
        groupsNoti.gameObject.SetActive(false);
        this.state = state;
        isLocked = state == ItemTrophyState.Locked;
        lockObj.gameObject.SetActive(isLocked);
        newsTagObj.gameObject.SetActive(false);
        TurnBgCoral(!isLocked);
        TurnBlur(!isLocked);
        TurnLight(false);
        mainTrophy.gameObject.SetActive(false);
        DeactiveAllBar();
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
                mainTrophy.gameObject.SetActive(true);
                newsTagObj.gameObject.SetActive(true);
                claimBar.gameObject.SetActive(true);
                break;
            case ItemTrophyState.Complete:
                mainTrophy.gameObject.SetActive(true);
                completeBar.gameObject.SetActive(true);
                TurnLight(true);
                break;
        }
    }

    public void SetProgressBar(int current,int max)
    {
        progressText.text = current + "/" + max;
        fillImage.fillAmount = (float) current / (float) max;
        targetToTrophy = max - current;
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
