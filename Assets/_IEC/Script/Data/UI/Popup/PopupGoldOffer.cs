using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IEC.Common.Event;
using LightScrollSnap;
using UnityEngine;

namespace IEC
{
    public class PopupGoldOffer : BasePopup
    {
        public IAPShopScroll packScroll;
        public Transform rewardContainerTransform;
        
        public Sequence _showAnimSequence;

        private void Start()
        {
            onShowDone.AddListener(DoAnimationOpen);
            
            gameObject.OnEvent(EventList.GamePlay.BUY_NO_ADS, UpdateUI);
            gameObject.OnEvent(EventList.GamePlay.BUY_STARTER_PACK, UpdateUI);
        }

        private void UpdateUI()
        {
            packScroll.CheckListShowing();
        }

        public override void Show()
        {
            base.Show();
            SetScaleAnim();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void SetScaleAnim()
        {
            foreach (Transform child in rewardContainerTransform.transform)
            {
                child.localScale = Vector3.zero;
            }
        }

        public void DoAnimationOpen()
        {
            DOTween.Kill(_showAnimSequence);
            _showAnimSequence = DOTween.Sequence();

            float delay = 0.1f;
            
            foreach (Transform child in rewardContainerTransform.transform)
            {
                _showAnimSequence.InsertCallback(delay,
                    () =>
                    {
                        TweenControl.GetInstance().ScaleFromZero(child.gameObject,0.2f);
                    });
                delay += 0.1f;
            }

            _showAnimSequence.Play();
            //TweenControl.GetInstance().ScaleFromZero(noThanksBtn.gameObject,0.5f);
        }
    }
}
