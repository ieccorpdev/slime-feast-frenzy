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
        float TWEEN_TIME = 0.5f;

        protected virtual void Awake()
        {
            SetEvent(btnClose, OnHideClick);
        }
        
        public override void Show()
        {
            IsShow = true;
            gameObject.SetActive(true);
            blackPanel.SetActive(isShowBlackPanel);
            if (isHaveAnim)
            {
                mainPanel.transform.localScale = Vector3.zero;
                mainPanel.transform.DOScale(Vector3.one, TWEEN_TIME)
                    .SetId(TweenID)
                    .SetEase(Ease.OutBack)
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
        }
        
        public override void Hide()
        {
            IsShow = false;
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