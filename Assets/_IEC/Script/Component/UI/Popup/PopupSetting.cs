using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupSetting : _BaseBehaviour, IUIGroup
    {
        public Button btnClose, btnSound, btnVibrate, btnRestorePurchase;

        public GameObject soundOffObj, vibrateOffObj;

        public void OnInit()
        {
            btnClose.onClick.AddListener(() => {
                UI.popupSettings.gameObject.SetActive(false);
                Game.isShowPopup = false;
            });

            btnSound.onClick.AddListener(() => {
                PlayerData.isSoundOn = !PlayerData.isSoundOn;
                SetStatus(PlayerData.isSoundOn, PlayerData.isVibrateOn);
            });

            btnVibrate.onClick.AddListener(() => {
                PlayerData.isVibrateOn = !PlayerData.isVibrateOn;
                SetStatus(PlayerData.isSoundOn, PlayerData.isVibrateOn);
            });

            btnRestorePurchase.onClick.AddListener(() => {
                EventController.RaiseEvent(EventList.GamePlay.IAP_RESTORE_NO_ADS);
            });
        }

        public void SetStatus(bool isSoundOn, bool isVibrateOn)
        {
            soundOffObj.SetActive(!isSoundOn);
            vibrateOffObj.SetActive(!isVibrateOn);

            Sound.SetupAudio();
        }

        public void SetupAbUI()
        {
            btnRestorePurchase.gameObject.SetActive(Game.isHaveNoAdsInMainScreen);
        }
    }
}
