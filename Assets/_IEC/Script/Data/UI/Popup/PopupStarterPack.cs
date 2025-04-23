using _IEC;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class PopupStarterPack : BasePopup
    {
        public Action actionOneTimeClosePopup;
        
        public List<Transform> listRewardImg = new();

        [FormerlySerializedAs("buyAdsBtn")] public Button buyBtn;
        
        //public SpineAnimOT anim;
        
        public Button btnX;

        public PackUI configBundle;
        
        public Sequence _showAnimSequence;
        protected override void Awake()
        {
            base.Awake();
   
            SetEvent(btnX, OnHideClick);
            AudioEventOpen = EventList.Audio.STARTER_PACK_POPUP_OPEN;
            
            onShow.AddListener(SetScaleAnim);
            onShowDone.AddListener(DoAnimationOpen);
        }

        public override void Show()
        {
            base.Show();
            // anim.SetAnimation("1-Starter_Pack_In", false, () =>
            // {
            //     anim.SetAnimation("2-Starter_Pack_Idle", true);
            // });
            configBundle.UpdateUI();
            // SetScaleAnim();
            // DoAnimationOpen();
        }

        public override void Hide()
        {
            base.Hide();
            if (actionOneTimeClosePopup != null)
            {
                actionOneTimeClosePopup.Invoke();
                actionOneTimeClosePopup = null;
            }
        }
        
        public void SetScaleAnim()
        {
            buyBtn.gameObject.transform.localScale = Vector3.zero;
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
                        //TweenControl.GetInstance().ScaleFromZero(img.gameObject,0.2f);
                    });
                delay += 0.1f;
            }
            
            delay += 0.1f;
            _showAnimSequence.InsertCallback(delay,
                () =>
                {
                    //TweenControl.GetInstance().ScaleFromZero(buyBtn.gameObject,0.5f);
                });
            //TweenControl.GetInstance().ScaleFromZero(noThanksBtn.gameObject,0.5f);
        }
    }
}
