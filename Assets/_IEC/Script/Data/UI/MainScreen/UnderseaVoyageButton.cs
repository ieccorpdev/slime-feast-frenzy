using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    [RequireComponent(typeof(Button))]
    public class UnderseaVoyageButton : _BaseBehaviour
    {
        [SerializeField] private Sprite sprIconLock, sprIconUnlock;
        [SerializeField] private Image imgMainIcon;
        [SerializeField] private GameObject lockGroup, claimGroup, normalGroup, notiGroup;
        [SerializeField] private TMP_Text txtNotiCount, txtTimer, txtTitle, levelUnlockTxt;
        private Button button;
        public UnderseaVoyageState currentState = UnderseaVoyageState.Lock;
        private int notiCount = 0;

        public void SetState(UnderseaVoyageState state)
        {
            currentState = state;
            bool showNoti = state == UnderseaVoyageState.Normal || state == UnderseaVoyageState.Claim;
            notiGroup.SetActive(showNoti && notiCount > 0);
            switch (state)
            {
                case UnderseaVoyageState.Normal:
                    SetGroupActive(Lock: false, claim: false, normal: true);
                    SetIcon(sprIconUnlock);
                    break;

                case UnderseaVoyageState.Lock:
                    levelUnlockTxt.text = $"Lvl.{Config.specialObjectConfig.kingdomSpecialObjectSpawnData.levelStartSpawn + 1}";
                    SetGroupActive(Lock: true, claim: false, normal: false);
                    SetIcon(sprIconLock);
                    break;

                case UnderseaVoyageState.Unlock:
                    SetGroupActive(Lock: false, claim: true, normal: false);
                    SetIcon(sprIconUnlock);
                    txtTitle.text = "Join";
                    if (!PlayerData.journeyData.isShowHandtut)
                    {
                        Locator.Main.HighLightCanvas.HighLightUI(this.GetComponent<RectTransform>(), true, true);
                    }
                    break;

                case UnderseaVoyageState.Finish:
                    SetGroupActive(Lock: false, claim: false, normal: true);
                    SetIcon(sprIconLock);
                    txtTimer.text = "Finished";
                    break;

                case UnderseaVoyageState.Claim:
                    SetGroupActive(Lock: false, claim: true, normal: false);
                    SetIcon(sprIconUnlock);
                    txtTitle.text = "Claim";
                    break;
            }
        }

        private void SetNotiCount()
        {
            notiCount = Locator.Main.PopupJourney.GetAvailableClaimCount();
            txtNotiCount.text = notiCount.ToString();
        }

        private void SetGroupActive(bool Lock, bool claim, bool normal)
        {
            lockGroup.SetActive(Lock) ;
            claimGroup.SetActive(claim);
            normalGroup.SetActive(normal);
        }

        private void SetIcon(Sprite icon)
        {
            imgMainIcon.sprite = icon;
        }

        public void InitStatusButton()
        {
            SetNotiCount();
            if (PlayerData.level <= Config.specialObjectConfig.kingdomSpecialObjectSpawnData.levelStartSpawn)
            {
                SetState(UnderseaVoyageState.Lock);
            }
            else if (!PlayerData.journeyData.wasOpenJourney)
            {
                SetState(UnderseaVoyageState.Unlock);
            }
            else
            {
                if (PlayerData.journeyData.isEndEvent)
                {
                    SetState(UnderseaVoyageState.Finish);
                }
                else
                {
                    if (notiCount > 0)
                    {
                        SetState(UnderseaVoyageState.Claim);
                    }
                    else
                    {
                        SetState(UnderseaVoyageState.Normal);
                    }
                }
            }
        }

        private void Awake()
        {
            Pool.Set(this);
        }

        private void Start()
        {
            button = GetComponent<Button>();    
            button.onClick.AddListener(HandleEventClickShowPopupUnderseaVoyage);
            InitStatusButton();
        }

        private void Update()
        {
            if (txtTimer != null)
            {
                if (PlayerData.journeyData.wasOpenJourney)
                {
                    txtTimer.text = PlayerData.journeyData.GetTimeRemainingString();
                    if (txtTimer.text.Equals("Finished"))
                    {
                        if (currentState != UnderseaVoyageState.Finish)
                        {
                            SetState(UnderseaVoyageState.Finish);
                            if (PlayerData.journeyData.CheckFinishedDay())
                                Locator.Main.PopupUnderseaVoyageIntro.Show();
                        }
                    }
                    else if (currentState == UnderseaVoyageState.Finish)
                    {
                        SetState(UnderseaVoyageState.Unlock);
                    }
                }
            }
        }

        private void HandleEventClickShowPopupUnderseaVoyage()
        {
            if (currentState == UnderseaVoyageState.Unlock)
            {
                Locator.Main.HighLightCanvas.RemoveCurrentHighLight();
            }
            if (currentState == UnderseaVoyageState.Finish)
            {
                Locator.Main.PopupUnderseaVoyageIntro.Show();
                return;
            }
            if (currentState == UnderseaVoyageState.Lock)
            {
                return;
            }
            if (PlayerData.journeyData.wasOpenJourney)
            {
                Locator.Main.PopupJourney.Show();
            }
            else
            {
                PlayerData.journeyData.SetStartJourneyTimeNow();
                Locator.Main.PopupUnderseaVoyageIntro.Show();
            }
            InitStatusButton();
        }
    }

    public enum UnderseaVoyageState
    {
        Lock,
        Unlock,
        Finish,
        Claim,
        Normal
    }
}
