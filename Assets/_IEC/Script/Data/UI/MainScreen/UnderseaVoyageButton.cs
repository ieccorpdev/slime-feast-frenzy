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

        public void SetState(UnderseaVoyageState state)
        {
            bool showNoti = state == UnderseaVoyageState.Normal || state == UnderseaVoyageState.Claim;
            notiGroup.SetActive(showNoti);
            if (showNoti) SetNotiCount();
            currentState = state;
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
                    break;

                case UnderseaVoyageState.Finish:
                    PlayerData.journeyData.wasOpenJourney = false;
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
            int count = PlayerData.journeyData.currentFuelJourney;
            bool hasNoti = count > 0;
            txtNotiCount.text = count.ToString();
            notiGroup.SetActive(hasNoti);
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

        private void InitStatusButton()
        {
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
                if (PlayerData.journeyData.IsJourneyExpired)
                {
                    SetState(UnderseaVoyageState.Finish);
                }
                else
                {
                    if (PlayerData.journeyData.AnyItemCanClaimed())
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

        private IEnumerator Start()
        {
            button = GetComponent<Button>();    
            button.onClick.AddListener(HandleEventClickShowPopupUnderseaVoyage);
            InitStatusButton();
            while (true)
            {
                if (txtTimer != null)
                {
                    txtTimer.text = PlayerData.journeyData.GetTimeRemainingString();
                    if (PlayerData.journeyData.wasOpenJourney)
                    {
                        if (txtTimer.text.Equals("Finished"))
                        {
                            SetState(UnderseaVoyageState.Finish);
                        }
                        else if (currentState == UnderseaVoyageState.Finish)
                        {
                            SetState(UnderseaVoyageState.Unlock);
                        }
                    }
                }
                yield return new WaitForSeconds(1f);
            }
        }

        private void HandleEventClickShowPopupUnderseaVoyage()
        {
            if (currentState == UnderseaVoyageState.Finish || currentState == UnderseaVoyageState.Lock)
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
