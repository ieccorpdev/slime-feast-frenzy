using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IEC.CloudSave;
namespace IEC
{
    public class PopupSettingInHome : PopupSettingIngame
    {
        public Image mainBg;
        public Button giftCode;
        private int countGiftCodeCheck;
        public SettingButton soundSettingButton;
        private Vector2 baseSettingSize;
        private int countShowGiftCode = 6;
        protected override void Awake()
        {
            base.Awake();
            baseSettingSize = mainBg.rectTransform.sizeDelta;
        }
        protected override void Start()
        {
            base.Start();
            SetEvent(giftCode,OnGiftCodeClick);
            soundSettingButton.actionClickSetting += OnSetSound;
            //Cloud Save
            SetEvent(btnCloudSave, OnBtnCloudSaveClick);

            //btnCloudSaveState.isOn = CloudSaveController.cloudSaveInstallData.isCloudSaveOn;
            //btnCloudSaveOn.SetActive(CloudSaveController.cloudSaveInstallData.isCloudSaveOn);
            //btnCloudSaveOff.SetActive(!CloudSaveController.cloudSaveInstallData.isCloudSaveOn);
        }

        public void OnGiftCodeClick()
        {
            Locator.Main.PopupGiftCode.Show();
        }

        public void OnSetSound()
        {
            countGiftCodeCheck++;
            if (countGiftCodeCheck >= countShowGiftCode)
            {
                // each time on off count 1
                ShowGiftCodeButton(true);
            }
        }

        public void ShowGiftCodeButton(bool stt)
        {
            if (baseSettingSize == Vector2.zero)
            {
                baseSettingSize = mainBg.rectTransform.sizeDelta;
            }
            Vector2 sizeD = stt ? new Vector2(baseSettingSize.x, baseSettingSize.y + 250) : baseSettingSize;
            giftCode.gameObject.SetActive(stt);
            mainBg.rectTransform.sizeDelta = sizeD;
        }
        public override void Show()
        {
            countGiftCodeCheck = 0;
            ShowGiftCodeButton(false);
            base.Show();
        }
        public override void Hide()
        {
            countGiftCodeCheck = 0;
            base.Hide();
        }

        #region CloudSaveFunction
        [Header("Buttons")]
        [SerializeField] private Button btnCloudSave;

        //[Header("Cloud Save")]
        //[SerializeField] private Toggle btnCloudSaveState;
        //[SerializeField] private GameObject btnCloudSaveOn;
        //[SerializeField] private GameObject btnCloudSaveOff;
        //[SerializeField] private CanvasGroup groupCloudSaveActivated;
        //[SerializeField] private CanvasGroup groupCloudSaveNotAvailable;
        //[SerializeField] private CanvasGroup groupCloudSaveNotBelong;

        private CloudSaveController CloudSaveController => Controllers.Get<CloudSaveController>();
        
        private void OnBtnCloudSaveClick()
        {
            if (!CloudSaveController.cloudSaveInstallData.isCloudSaveOn)
            {
                if (CloudSaveController.IsCloudSaveAvailableAndBelongToUser())
                {
                    CloudSaveController.ShowDifference();
                    CloudSaveController.cloudSaveInstallData.isCloudSaveOn = true;
                    CloudSaveController.SaveInstallData();

                    SetBtnCloudSaveState(true);
                }
                else if (CloudSaveController.IsUserSignedIn && !CloudSaveController.IsCloudSaveBelongToUser())
                {
                    OnCloudSaveNotBelongToUser();
                }
                else
                {
                    SyncUser();
                }
            }
            else
            {
                OnCloudSaveActivated();
            }
        }
        
        private void SetBtnCloudSaveState(bool isOn)
        {
            CloudSaveController.cloudSaveInstallData.isCloudSaveOn = isOn;
            //btnCloudSaveState.isOn = isOn;
            //btnCloudSaveOn.SetActive(isOn);
            //btnCloudSaveOff.SetActive(!isOn);
        }

        private void OnCloudSaveActivated()
        {
            DOTween.Kill("CloudSaveActivated");
           // groupCloudSaveActivated.alpha = 1;
            DOVirtual.DelayedCall(0.5f, () =>
            {
                //groupCloudSaveActivated.DOFade(0, 1f).SetEase(Ease.InQuad);
            }).SetId("CloudSaveActivated");

            SetBtnCloudSaveState(true);
        }

        private void OnCloudSaveNotBelongToUser()
        {
            Debug.LogError("[CLOUD SAVE] Not belong to user");
            DOTween.Kill("CloudSaveNotBelong");
            //groupCloudSaveNotBelong.alpha = 1;
            DOVirtual.DelayedCall(1.5f, () =>
            {
                //groupCloudSaveNotBelong.DOFade(0, 1f).SetEase(Ease.InQuad);
            }).SetId("CloudSaveNotBelong");

            SetBtnCloudSaveState(false);
        }
        
        private void OnCloudSaveNotAvailable()
        {
            Debug.LogError("[CLOUD SAVE] Not available");
            DOTween.Kill("CloudSaveNotAvailable");
            //groupCloudSaveNotAvailable.alpha = 1;
            DOVirtual.DelayedCall(1.5f, () =>
            {
                //groupCloudSaveNotAvailable.DOFade(0, 1f).SetEase(Ease.InQuad);
            }).SetId("CloudSaveNotAvailable");

            SetBtnCloudSaveState(false);
        }

        private void SyncUser()
        {
            CloudSaveController.Authenticate(OnSyncComplete);
        }

        private void OnSyncComplete()
        {
            if (CloudSaveController.IsCloudSaveAvailableAndBelongToUser())
            {
                CloudSaveController.ShowDifference();
                CloudSaveController.cloudSaveInstallData.isCloudSaveOn = true;
                CloudSaveController.SaveInstallData();

                SetBtnCloudSaveState(true);
            }
            else if (CloudSaveController.IsUserSignedIn && !CloudSaveController.IsCloudSaveBelongToUser())
            {
                OnCloudSaveNotBelongToUser();
            }
            else
            {
                Debug.LogError("[CLOUD SAVE] Sync Failed");
                OnCloudSaveNotAvailable();
            }
        }

        public void SetCloudSaveOnExternally()
        {
            CloudSaveController.cloudSaveInstallData.isCloudSaveOn = true;
            CloudSaveController.SaveInstallData();
            SetBtnCloudSaveState(true);
        }

        public void CheckCloudSaveBelongToUser()
        {
            // Only check case data not belong to user
            // If there is no user/user not signed in, do nothing since cant distinguish between no user and no network
            if (!CloudSaveController.IsCloudSaveBelongToUser())
            {
                Debug.LogError("[CLOUD SAVE] Not belong to user");
                SetBtnCloudSaveState(false);
            }
        }

        #endregion
    }
}
