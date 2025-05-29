using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupUnderseaVoyageIntro : BasePopup
    {
        [SerializeField] private List<GameObject> activeGroup, inactiveGroup;
        [SerializeField] private TMP_Text timerTxt;
        [SerializeField] private Button buttonStart;
        [SerializeField] private Button buttonClose;
        public UnderseaVoyageState currentState = UnderseaVoyageState.Lock;
        private bool isFinished;

        public override void Show()
        {
            base.Show();
            if (PlayerData.journeyData.IsJourneyExpired)
            {
                SetState(UnderseaVoyageState.Finish);
            }
            else
            {
                SetState(UnderseaVoyageState.Unlock);
            }
        }

        private void SetState(UnderseaVoyageState state)
        {
            currentState = state;
            bool isFinish = state == UnderseaVoyageState.Finish;
            foreach(GameObject group in activeGroup)
            {
                group.SetActive(!isFinish);
            }
            foreach (GameObject group in inactiveGroup)
            {
                group.SetActive(isFinish);
            }
            buttonClose.gameObject.SetActive(isFinish);
        }

        private void HandleEventClickShowPopupUnderseaVoyage()
        {
            Locator.Main.PopupJourney.Show();
            Hide();
        }

        public override void Hide()
        {
            base.Hide();
            if (isFinished)
            {
                isFinished = false;
                if (PlayerData.journeyData.CheckFinishedDay())
                    Locator.Main.PopupJourney.ClaimAllItem();
            }
        }

        private IEnumerator Start()
        {
            buttonStart.onClick.AddListener(HandleEventClickShowPopupUnderseaVoyage);
            buttonClose.onClick.AddListener(Hide);
            while (true)
            {
                if (timerTxt != null)
                {
                    timerTxt.text = PlayerData.journeyData.GetTimeRemainingString();
                    if (currentState != UnderseaVoyageState.Finish && timerTxt.text.Equals("Finished"))
                    {
                        SetState(UnderseaVoyageState.Finish);
                        isFinished = true;
                        yield break;
                    }
                    else if (currentState == UnderseaVoyageState.Finish)
                    {
                        SetState(UnderseaVoyageState.Unlock);
                    }
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
