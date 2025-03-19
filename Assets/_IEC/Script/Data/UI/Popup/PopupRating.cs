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
            Services.PushEvent(new TrackingEvent.RatingClickLoveIt());
            PlayerData.isClickedRate = true;
            Services.RateUs();
            Hide();
        }

        void OnNoThankClick()
        {
            Services.PushEvent(new TrackingEvent.RatingClickNoThank());
            PlayerData.isClickedRate = true;
            Hide();
        }

        protected override void OnHideClick()
        {
            Services.PushEvent(new TrackingEvent.RatingClickClose());
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