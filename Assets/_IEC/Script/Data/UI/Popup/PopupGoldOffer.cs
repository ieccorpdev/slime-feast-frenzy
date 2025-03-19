using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace IEC
{
    public class PopupGoldOffer : BasePopup
    {
        public Transform rewardContainerTransform;
        public Transform offerBundleUI;
        
        public Sequence _showAnimSequence;

        private void Start()
        {
            onShowDone.AddListener(DoAnimationOpen);
        }

        public override void Show()
        {
            base.Show();
            SetScaleAnim();
            TweenControl.GetInstance().ScaleFromZero(offerBundleUI.gameObject, TWEEN_TIME, null, EaseType.InQuad);
        }

        public override void Hide()
        {
            base.Hide();
            TweenControl.GetInstance().ScaleFromOne(offerBundleUI.gameObject, TWEEN_TIME, null, EaseType.InBack);
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
