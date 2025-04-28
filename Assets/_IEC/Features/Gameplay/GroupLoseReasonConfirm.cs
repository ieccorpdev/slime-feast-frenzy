using System;
using System.Collections.Generic;
using _IEC.Features.Booster;
using _IEC.Features.Common;
using _IEC.Features.Common.Extension;
using IEC;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class GroupLoseReasonConfirm : BasePopup
    {
        [SerializeField] private GroupCurrencyResult groupCurrencyResult;
        [SerializeField] private Button btnPlayOn;
        [SerializeField] protected TextMeshProUGUI txtNumberCoinPlayOn;
        [SerializeField] private bool isByBomb;
        [SerializeField] private GroupLevelFailed groupLevelFailed;
        [SerializeField] private RectTransform iapShopScroll;
        [SerializeField] private RectTransform notEnoughGoldWarning;
        private Action m_ActionPlayOn;
        private int goldCost => Config.gamePlayConfig.baseNumberCoinForRevive;
        protected override void Awake()
        {
            base.Awake();
            btnClose.onClick.AddListener(BtnCloseFunction);
            onHideDone.AddListener(() =>
            {
                groupLevelFailed.Show();
            });
            SetEvent(btnPlayOn,OnBtnPlayOn);
        }

        public void SetActionPlayOn(Action action)
        {
            m_ActionPlayOn = action;
        }

        public override void Show()
        {
            isPlayAudioPopup = false;
            UpdateUI();
            notEnoughGoldWarning.gameObject.SetActive(false);
            groupCurrencyResult.UpdateUI();
            base.Show();
        }
        public void OnBtnPlayOn()
        {
            EventController.RaiseEvent(EventList.Audio.ACTIVE_BUTTON_CLICK);
            if (PlayerData.GetPlayerCurrencyCount(ResourceType.Gold) >= goldCost)
            {
                PlayerData.SubtractCurrency(ResourceType.Gold,goldCost);
                m_ActionPlayOn?.Invoke();
            }
            else
            {
                Locator.Main.PopupGoldOffer.Show();
            }
        }
        private void UpdateUI()
        {
            txtNumberCoinPlayOn.text = Config.gamePlayConfig.baseNumberCoinForRevive.ToString();
        }


        private void BtnCloseFunction()
        {
            Locator.Game.ScreenLevelLose.groupLevelFailed.Show();
            Hide();
            EventController.RaiseEvent(EventList.Audio.ACTIVE_BUTTON_CLICK);
        }
    }
}