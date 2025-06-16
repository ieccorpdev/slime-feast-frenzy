using System;
using _IEC.Features.Common;
using _IEC.Features.Common.Extension;
using IEC;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class GroupLoseReason : BasePopup
    {
        [SerializeField] protected Button btnPlayOn;
        [SerializeField] protected TextMeshProUGUI txtNumberCoinPlayOn;
        [SerializeField] private bool isByBomb;
        [SerializeField] private GroupLoseReasonConfirm groupLoseReasonConfirm;
        [SerializeField] private RectTransform notEnoughGoldWarning;
        private Action m_PlayOnAction;
        private int goldCost => PlayConfig.gamePlayConfig.baseNumberCoinForRevive;
        public LoseReason typeScreen;
        protected override void Awake()
        {
            base.Awake();
            btnClose.onClick.AddListener(Hide);
            onHideDone.AddListener(groupLoseReasonConfirm.Show);
            SetEvent(btnPlayOn,OnBtnPlayOn);
        }

        public void OnBtnPlayOn()
        {
            if (PlayerData.GetPlayerCurrencyCount(ResourceType.Gold) >= goldCost)
            {
                PlayerData.SubtractCurrency(ResourceType.Gold,goldCost);
                m_PlayOnAction?.Invoke();
            }
            else
            {
                Locator.Main.PopupGoldOffer.Show();
            }
        }
        public void SetActionPlayOn(Action action)
        {
            m_PlayOnAction = action;
            groupLoseReasonConfirm.SetActionPlayOn(action);
        }

        public override void Show()
        {
            UpdateUI();
            notEnoughGoldWarning.gameObject.SetActive(false);
            base.Show();
        }
        
        private void UpdateUI()
        {
            txtNumberCoinPlayOn.text = PlayConfig.gamePlayConfig.baseNumberCoinForRevive.ToString();
        }
    }
}