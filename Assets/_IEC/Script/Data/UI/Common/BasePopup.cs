using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace IEC
{
    public class BasePopup : ScreenUI
    {
        public Button btnClose;
        public GameObject blackPanel;
        public GameObject mainPanel;
        public bool isHaveAnim = true;
        public bool isHaveAnimClose = true;
        public bool isShowBlackPanel = true;
        public bool isShowTopResourceBar = false;
        protected Ease showEase = Ease.OutBack;
        string TweenID => $"Anim{gameObject.GetInstanceID()}";
        protected float TWEEN_TIME = 0.2f;

        protected bool isPlayAudioPopup = true;
        private bool isAudioOpenPlayed = false;
        protected string AudioEventOpen = EventList.Audio.POPUP_OPEN;
        protected string AudioEventClose = EventList.Audio.POPUP_CLOSE;

        private bool isResoucreBarShownBeforeShowPopup = false;

        protected virtual void Awake()
        {
            SetEvent(btnClose, OnHideClick);
        }

        public override void Show()
        {
            HandleShowTopResourceBar();
            
            if(GamePlay) GamePlay.userInput.CanClick = false;
            DOTween.Kill(TweenID);
            IsShow = true;
            IsFocus = true;
            gameObject.SetActive(true);
            if (blackPanel != null)
                blackPanel.SetActive(isShowBlackPanel);

            mainPanel.transform.localScale = Vector3.one;
            if (mainPanel.gameObject.TryGetComponent<AspectRatioScaler>(out var scaler))
            {
                scaler.UpdateRect();
            }

            if (isHaveAnim)
            {
                var scale = mainPanel.transform.localScale;
                mainPanel.transform.localScale = Vector3.one*0.8f;
                mainPanel.transform.DOScale(scale, TWEEN_TIME)
                    .SetId(TweenID)
                    .SetEase(showEase)
                    .OnComplete(() =>
                    {
                        onShowDone.Invoke();
                    });
            }
            else
            {
                onShowDone.Invoke();
            }
            onShow.Invoke();
            onHaveFocus.Invoke(true);

            if (isPlayAudioPopup)
            {
                EventController.RaiseEvent(AudioEventOpen);
                isAudioOpenPlayed = true;
            }
        }
        
        public override void Hide()
        {
            DOTween.Kill(TweenID);
            IsShow = false;
            IsFocus = false;
            if (isHaveAnim)
            {
                //if (isHaveAnimClose)
                //{
                //    mainPanel.transform.DOScale(Vector3.zero, TWEEN_TIME)
                //    .SetId(TweenID)
                //    .SetEase(Ease.InBack)
                //    .OnComplete(() =>
                //    {
                //        gameObject.SetActive(false);
                //        onHideDone.Invoke();
                //    });
                //}
                //else
                {
                    gameObject.SetActive(false);
                    onHideDone.Invoke();
                }
                
            }
            else
            {
                gameObject.SetActive(false);
                onHideDone.Invoke();
            }
            onHide.Invoke();
            onLostFocus.Invoke(true);

            if (isPlayAudioPopup && isAudioOpenPlayed)
            {
                EventController.RaiseEvent(AudioEventClose);
                isAudioOpenPlayed = false;
            }
        }

        protected virtual void OnHideClick()
        {
            Hide();
        }

        private void OnDestroy()
        {
            DOTween.Kill(TweenID);
        }

        private void HandleShowTopResourceBar()
        {
            isResoucreBarShownBeforeShowPopup = Locator.Service.GameResourceDisplay.IsShow;
            if (isShowTopResourceBar)
            {
                Locator.Service.GameResourceDisplay.ToggleDisplayAllResources(true);
            }
            else
            {
                Locator.Service.GameResourceDisplay.HideDisplayUnderBG();
            }
        }

        private void HandleHideTopResourceBar()
        {
            if (!isResoucreBarShownBeforeShowPopup)
            {
                Locator.Service.GameResourceDisplay.ToggleDisplayAllResources(false);
            }
        }
    }
}