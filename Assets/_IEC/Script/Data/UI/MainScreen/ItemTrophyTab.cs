using System.Collections;
using System.Collections.Generic;
using IEC;
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
        bool isLocked = state == ItemTrophyState.Locked;
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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
