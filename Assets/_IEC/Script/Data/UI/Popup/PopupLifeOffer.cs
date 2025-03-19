using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupLifeOffer : BasePopup
    {
        public GameObject mainImage;
        public Transform rewardContainerTransform;
        public Button buyBtn;
        public PackUI configBundle;
        public SpineAnimOT mainAnimLifeOffer;
        public Sequence _showAnimSequence;

        protected override void Awake()
        {
            base.Awake();
            AudioEventOpen = EventList.Audio.VITALITY_PACK_POPUP_OPEN;
            
            configBundle.OnPurchaseSuccess += () =>
            {
                Locator.Main.HomeScreenTab.UpdateMainHome();
                Hide();
                Services.SaveData();
            };
        }

        private void Start()
        {
            onShowDone.AddListener(DoAnimationOpen);
        }

        public override void Show()
        {
            base.Show();
            SetScaleAnim();
            //mainAnimLifeOffer.ske.Clear();
            mainAnimLifeOffer.SetAnimation("Start",false, () =>
            {
                mainAnimLifeOffer.SetAnimation("Idle2", true);
            });
            if (PlayerData.playerLife <= 0)
            {
                PlayerData.dailyShowLifeOfferPack = true;// Reset daily noti each 24h                
            }
            Locator.Main.HomeScreenTab.UpdateMainHome();
        }
        
        public void SetScaleAnim()
        {
            buyBtn.gameObject.transform.localScale = Vector3.zero;
            //mainImage.gameObject.transform.localScale = Vector3.zero;
            //noThanksBtn.gameObject.transform.localScale = Vector3.zero;

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
            
            //_showAnimSequence.InsertCallback(delay,
            //    () =>
            //    {
            //        TweenControl.GetInstance().ScaleFromZero(mainImage,0.2f);
            //    });

            //delay += 0.2f;
            
            foreach (Transform child in rewardContainerTransform.transform)
            {
                _showAnimSequence.InsertCallback(delay,
                    () =>
                    {
                        TweenControl.GetInstance().ScaleFromZero(child.gameObject,0.2f);
                    });
                delay += 0.1f;
            }
            
            delay += 0.1f;
            _showAnimSequence.InsertCallback(delay,
                () =>
                {
                    TweenControl.GetInstance().ScaleFromZero(buyBtn.gameObject,0.5f);
                });
            
            _showAnimSequence.Play();
            //TweenControl.GetInstance().ScaleFromZero(noThanksBtn.gameObject,0.5f);
        }
    }
}
