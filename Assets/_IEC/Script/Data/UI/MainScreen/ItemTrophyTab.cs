using System.Collections;
using System.Collections.Generic;
using IEC;
using IEC.Common.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTrophyTab : MonoBehaviour
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

    public Button newsClaimButton;
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

    private bool isLocked;
    void Start()
    {
        infoButton.onClick.AddListener(OnClickInfoTrophy);   
        gameObject.OnEvent(EventList.Trophy.CLICK_INFO_TROPHY, ResetInfo);
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
        EventController.RaiseEvent(EventList.Trophy.CLICK_INFO_TROPHY);
        //Progress lock and info action
        groupsLock.gameObject.SetActive(isLocked);
        groupsNoti.gameObject.SetActive(!isLocked);
        lockTrophyNameTMP.text = GetRealNameTrophy();

        if (isLocked)
        {
            OnClickLocked();
        }
        else
        {
            OnClickInfoGroups();
        }
    }

    public void OnClickLocked()
    {
        levelUnlockTrophyNameTMP.text =
            "Unlock at level " + Locator.Main.ScreenMain.trophyTab.GetLevelUnlockTrophy(type);
    }
    public void OnClickInfoGroups()
    {
        groupsNoti.gameObject.SetActive(false);//Test
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
    }

    public string GetRealNameTrophy()
    {
        switch (type)
        {
            case TrophyType.GrandVoyage:
                return "Grand Voyage";
                break;
            case TrophyType.SharkFrenzyForce:
                return "Shark Frenzy Force";
                break;
            case TrophyType.UnderseaVoyage:
                return "Undersea Voyage";
                break;
            case TrophyType.SharkRace:
                return "Shark Race";
                break;
            case TrophyType.ChompOff:
                return "Chomp-off";
                break;
            case TrophyType.OceanGift:
                return "Ocean Gift";
                break;
            case TrophyType.StarRace:
                return "Star Race";
                break;
            case TrophyType.RelicHunt:
                return "Relic Hunt";
                break;
        }

        return "Error";
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
