using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace IEC
{
    public class PopupSettingIngame : BasePopup
    {
        public Button btnContinue;
        [FormerlySerializedAs("btnGoHome")] public Button btnQuit;
        public Button privacyPolicy;
        public Button restorePurchaseBtn;
        public Button feedBackBtn;
        public Button closeBtn2;
        public List<SettingButton> settingButtons = new();

        private void Start()
        {
            SetEvent(btnContinue, () =>
            {
                // EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS, new EventParam(this, name));
                // Services.PushEvent(new IEC.TrackingEvent.ClickBtnReplay());
                Hide();
            });
            if (privacyPolicy)
            {
                SetEvent(privacyPolicy, () => { Services.OpenPrivacyPolicyURL(); });
            }
            if (restorePurchaseBtn)
            {
                SetEvent(restorePurchaseBtn, () =>
                {
                    Services.IAP().RestorePurchase((success) =>
                    {
                        if (success)
                        {
                            Locator.Service.PopupRestorePurchaseSuccess.Show();
                        }
                        else
                        {
                            Locator.Service.PopupRestorePurchaseFail.Show();
                        }

                    });
                });
            }

            if (feedBackBtn)
            {
                SetEvent(feedBackBtn, () =>
                {
                    Services.SendEmail();
                });
            }
            SetEvent(btnQuit, () =>
            {
                //EventController.RaiseEvent(EventList.GamePlay.GO_MAIN);
                if(Services.ShowFullAdsWithTime(Game.timeDelayFullAds, () =>
                {
                    PlayerData.DecreasePlayerLife();
                    if (GamePlay.IsInLevelHard)
                    {
                        PlayerData.currentCountLoseHardLevel++;
                    }

                    GameController.OpenScene(GameScene.Main);
                    EventController.RaiseEvent(EventList.Audio.BACKGROUND_HOME);
                }, () =>
                {
                    Services.PushEvent(new IEC.TrackingEvent.SFApostition($"skip_win_bonus_reward"));
                }));

            });
            SetEvent(closeBtn2, () => { Hide(); });
        }

        public override void Show()
        {
            base.Show();
            //GamePlay.SetPauseGame(true);

            foreach (var btn in settingButtons)
            {
                btn.UpdateUI();
            }
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}