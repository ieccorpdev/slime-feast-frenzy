using System.Collections.Generic;
using DG.Tweening;
using IEC;
using IEC.TrackingEvent;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
namespace _IEC
{
    public class PopupNoAds : BasePopup
    {
        public Button buyAdsBtn,noThanksBtn;
        public BundleNoAdsUI configBundle;
        public List<Transform> listRewardImg = new();
        private bool m_IsShowHandTut;
        public SpineAnimOT spinAnim;
        public Sequence _showAnimSequence;
        
        protected override void Awake()
        {
            base.Awake();
            SetEvent(buyAdsBtn,OnNoAdsButtonClick);
            SetEvent(noThanksBtn,OnNoThanksButtonClick);
            configBundle.OnPurchaseSuccess += () =>
            {
                noThanksBtn?.gameObject.SetActive(false);
                Hide();
            };
            
            AudioEventOpen = EventList.Audio.NO_ADS_PACK_POPUP_OPEN;
            
            onShow.AddListener(SetScaleAnim);
            onShowDone.AddListener(DoAnimationOpen);
        }
        public override void Show()
        {
            base.Show();
            configBundle.UpdateUI();
            spinAnim?.SetAnimation("1_start",false, () =>
            {
                spinAnim?.SetAnimation("2_idle", true);
            });
        }
        public void SetScaleAnim()
        {
            buyAdsBtn.gameObject.transform.localScale = Vector3.zero;
            //noThanksBtn.gameObject.transform.localScale = Vector3.zero;

            foreach (Transform img in listRewardImg)
            {
                img.localScale = Vector3.zero;
            }
        }
        public void DoAnimationOpen()
        {
            DOTween.Kill(_showAnimSequence);
            _showAnimSequence = DOTween.Sequence();

            float delay = 0.1f;
            
            foreach (Transform img in listRewardImg)
            {
                _showAnimSequence.InsertCallback(delay,
                    () =>
                    {
                        TweenControl.GetInstance().ScaleFromZero(img.gameObject,0.2f);
                    });
                delay += 0.1f;
            }
            
            delay += 0.1f;
            _showAnimSequence.InsertCallback(delay,
                () =>
                {
                    TweenControl.GetInstance().ScaleFromZero(buyAdsBtn.gameObject,0.5f);
                });
            //TweenControl.GetInstance().ScaleFromZero(noThanksBtn.gameObject,0.5f);
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
