using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SocialPlatforms;

namespace IEC
{
    public class ScreenInGame : ScreenUI
    {
        public override bool IS_PAUSE_GAME => false;

        public TextMeshProUGUI txtLevel;
        public GameObject normalLevelGroup, hardLevelGroup;
        public Button btnRestart;
        public Button btnSetting;

        [Header("Debug")]
        public Button btnLevelWin;
        public Button btnLevelLose;

        private void Start()
        {
            //listButtonHideOnClick.Add(btnRestart);

            SetEvent(btnRestart, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS, new EventParam(this, "restart_in_game"));
                Services.PushEvent(new TrackingEvent.ClickBtnReplay());
            });
            
            SetEvent(btnLevelWin, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN);
            });
            
            SetEvent(btnLevelLose, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_LOSE);
            });

            SetEvent(btnSetting, () =>
            {
                Locator.Game.PopupSettingIngame.Show();
            });
        }

        public void SetLevel(int level)
        {
            txtLevel.text = "LEVEL " + level.ToString();
            normalLevelGroup.SetActive(true);
            if(hardLevelGroup != null)
                hardLevelGroup.SetActive(false);
        }
        
        public void SetLevelHard()
        {
            if (!Game.isUsingHardLevel)
                return;
            
            normalLevelGroup.SetActive(false);
            if(hardLevelGroup != null)
                hardLevelGroup.SetActive(true);
        }

        public override void Show()
        {
            base.Show();
            btnSetting.gameObject.SetActive(PlayerData.level > 1);
            UpdateUIHelper();
        }

        public void UpdateUIHelper()
        {

        }
    }
}