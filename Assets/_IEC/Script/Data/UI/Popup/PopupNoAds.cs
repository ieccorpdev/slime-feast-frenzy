using IEC;
using IEC.TrackingEvent;
using UnityEngine;
using UnityEngine.UI;
namespace _IEC
{
    public class PopupNoAds : BasePopup
    {
        public Button buyAdsBtn,noThanksBtn;
        public BundleNoAdsUI configBundle;
        private bool m_IsShowHandTut;
        protected override void Awake()
        {
            base.Awake();
            SetEvent(buyAdsBtn,OnNoAdsButtonClick);
            SetEvent(noThanksBtn,OnNoThanksButtonClick);
            configBundle.OnPurchaseSuccess += () =>
            {
                noThanksBtn.gameObject.SetActive(false);
                Hide();
            };
        }
        public override void Show()
        {
            base.Show();
            SetScaleAnim();
            DoAnimationOpen();
        }
        public void SetScaleAnim()
        {
            buyAdsBtn.gameObject.transform.localScale = Vector3.zero;
            noThanksBtn.gameObject.transform.localScale = Vector3.zero;
        }
        public void DoAnimationOpen()
        {
            TweenControl.GetInstance().ScaleFromZero(buyAdsBtn.gameObject,0.5f);
            TweenControl.GetInstance().ScaleFromZero(noThanksBtn.gameObject,0.5f);
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void OnNoAdsButtonClick()
        {
            // TestBuy No Ads
        }
        public void OnNoThanksButtonClick()
        {
            Hide();
        }
    }
}
