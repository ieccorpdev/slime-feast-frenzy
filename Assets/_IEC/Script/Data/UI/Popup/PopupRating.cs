using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupRating : BasePopup
    {
        public Button btnNoThank;
        public Button btnLoveIt;

        private System.Action m_OnHide;
        
        private void Start()
        {
            SetEvent(btnLoveIt, OnLoveItClick);
            SetEvent(btnNoThank, OnNoThankClick);
        }

        void OnLoveItClick()
        {
            PlayerData.isClickedRate = true;
            Hide();
        }

        void OnNoThankClick()
        {
            PlayerData.isClickedRate = true;
            Hide();
        }

        protected override void OnHideClick()
        {
            base.OnHideClick();
        }

        public override void Hide()
        {
            base.Hide();
            m_OnHide?.Invoke();
        }

        public void SetEventOnHide(System.Action onHide)
        {
            m_OnHide = onHide;
        }
    }
}