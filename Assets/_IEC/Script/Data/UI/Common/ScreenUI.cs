using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.Events;

namespace IEC
{
    public class ScreenUI : _BaseBehaviour
    {
        public virtual bool IS_PAUSE_GAME => true;

        public bool isHideAllOtherScreen;
        public List<ScreenUI> listHideHaveEffect;
        public List<ScreenUI> listHideNoEffect;

        public bool IsShow { get; protected set; }
        public bool IsHide => !IsShow;
        public int ShowOrder { get; protected set; }

        [HideInInspector]
        public UnityEvent onShow;
        [HideInInspector]
        public UnityEvent onHide;

        [HideInInspector]
        public UnityEvent onShowDone;
        [HideInInspector]
        public UnityEvent onHideDone;

        protected List<Button> listButtonHideOnClick = new List<Button>();

        public virtual void LocateScreen()
        {
            Pool.Set(GetType(), this);
        }

        public virtual void Show()
        {
            IsShow = true;
            ShowButtonHideOnClick();
            EffectUI.DoShowHide(gameObject, true, (isShow) => onShowDone.Invoke());
            onShow.Invoke();
        }

        public virtual void Hide()
        {
            IsShow = false;
            EffectUI.DoShowHide(gameObject, false, (isShow) => onHideDone.Invoke());
            onHide.Invoke();
        }

        public virtual void SetActive(bool isActive)
        {
            IsShow = isActive;
            gameObject.SetActive(isActive);
            (isActive ? onShowDone : onHideDone).Invoke();
            (isActive ? onShow : onHide).Invoke();
        }

        public void SetShowOrder(int showOrder)
        {
            ShowOrder = showOrder;
        }

        [Button]
        public void TestShow()
        {
            Show();
        }

        [Button]
        public void TestHide()
        {
            Hide();
        }

        protected void ShowButtonHideOnClick()
        {
            foreach (var btn in listButtonHideOnClick)
            {
                btn.gameObject.SetActive(true);
            }
        }

        protected void SetEvent(UnityEngine.UI.Button button, System.Action action)
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                if (listButtonHideOnClick.Contains(button))
                {
                    button.onClick.AddListener(() =>
                    {
                        button.gameObject.SetActive(false);
                    });
                }
                button.onClick.AddListener(() =>
                {
                    action?.Invoke();
                });
            }
        }
    }
}
