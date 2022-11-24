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
        public bool isShowBlackPanel = true;
        string TweenID => $"Anim{gameObject.GetInstanceID()}";
        float TWEEN_TIME = 0.3f;

        protected virtual void Awake()
        {
            SetEvent(btnClose, OnHideClick);
        }
        
        public override void Show()
        {
            DOTween.Kill(TweenID);
            IsShow = true;
            IsFocus = true;
            gameObject.SetActive(true);
            blackPanel.SetActive(isShowBlackPanel);

            mainPanel.transform.localScale = Vector3.one;
            if (mainPanel.gameObject.TryGetComponent<AspectRatioScaler>(out var scaler))
            {
                scaler.UpdateRect();
            }

            if (isHaveAnim)
            {
                var scale = mainPanel.transform.localScale;
                mainPanel.transform.localScale = Vector3.zero;
                mainPanel.transform.DOScale(scale, TWEEN_TIME)
                    .SetId(TweenID)
                    .SetEase(Ease.InQuad)
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
        }
        
        public override void Hide()
        {
            DOTween.Kill(TweenID);
            IsShow = false;
            IsFocus = false;
            if (isHaveAnim)
            {
                mainPanel.transform.DOScale(Vector3.zero, TWEEN_TIME)
                    .SetId(TweenID)
                    .SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                        onHideDone.Invoke();
                    });
            }
            else
            {
                gameObject.SetActive(false);
                onHideDone.Invoke();
            }
            onHide.Invoke();
            onLostFocus.Invoke(true);
        }

        protected virtual void OnHideClick()
        {
            Hide();
        }

        private void OnDestroy()
        {
            DOTween.Kill(TweenID);
        }
    }
}