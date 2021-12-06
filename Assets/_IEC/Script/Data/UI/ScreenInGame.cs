using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IEC
{
    public class ScreenInGame : ScreenUI
    {
        public override bool IS_PAUSE_GAME => false;

        public TextMeshProUGUI txtLevel;
        public Button btnRestart;

        [Header("Debug")]
        public Button btnLevelWin;
        public Button btnLevelLose;
        public Button btnShowInter;
        public Button btnShowReward;

        private void Start()
        {
            listButtonHideOnClick.Add(btnRestart);

            SetEvent(btnRestart, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS);
            });
            
            SetEvent(btnLevelWin, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN);
            });
            
            SetEvent(btnLevelLose, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_LOSE);
            });

            SetEvent(btnShowInter, () =>
            {
                Services.ShowFullAds(() =>
                {
                    Debug.LogError("Done inter");
                });
            });

            SetEvent(btnShowReward, () =>
            {
                Services.ShowRewardAds(() =>
                {
                    Debug.LogError("Done reward");
                });
            });
        }

        public void SetLevel(int level)
        {
            txtLevel.text = "LEVEL " + level.ToString();
        }
    }
}