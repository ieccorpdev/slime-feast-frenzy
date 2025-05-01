using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace IEC
{
    public class ScreenUI : ScreenBehaviour
    {
        public virtual bool IS_PAUSE_GAME => true;

        public bool isHideAllOtherScreen;
        public List<ScreenUI> listHideHaveEffect;
        public List<ScreenUI> listHideNoEffect;

        public bool IsShow { get; protected set; }
        public bool IsHide => !IsShow;
        public int ShowOrder { get; protected set; }
        public bool IsFocus { get; protected set; }

        [HideInInspector]
        public UnityEvent onShow;
        [HideInInspector]
        public UnityEvent onHide;

        [HideInInspector]
        public UnityEvent onShowDone;
        [HideInInspector]
        public UnityEvent onHideDone;
        
        [HideInInspector]
        public UnityEvent tempOnHideDone;

        [HideInInspector]
        public UnityBoolEvent onHaveFocus;    // bool isOnShow
        [HideInInspector]
        public UnityBoolEvent onLostFocus;    // bool isOnHide

        protected List<Button> listButtonHideOnClick = new List<Button>();

        public virtual void LocateScreen()
        {
            Pool.Set(GetType(), this);
            IsShow = gameObject.activeSelf;
        }

        public virtual void Show()
        {
            if (IsShow) return;

            IsShow = true;
            IsFocus = true;
            ShowButtonHideOnClick();
            EffectUI.DoShowHide(gameObject, true, (isShow) =>
            {
                onShowDone?.Invoke();
            });
            onShow?.Invoke();
            onHaveFocus?.Invoke(true);
        }

        public virtual void Hide()
        {
            if (IsHide) return;

            IsShow = false;
            IsFocus = false;
            EffectUI.DoShowHide(gameObject, false, (isShow) =>
            {
                onHideDone.Invoke();
                tempOnHideDone.Invoke();
                tempOnHideDone?.RemoveAllListeners();
            });
            onHide.Invoke();
            onLostFocus.Invoke(true);
        }

        public virtual void LostFocus()
        {
            if (!IsFocus) return;

            IsFocus = false;
            onLostFocus.Invoke(false);
        }

        public virtual void HaveFocus()
        {
            if (IsFocus) return;

            IsFocus = true;
            onHaveFocus.Invoke(false);
        }

        public override void SetActive(bool isActive)
        {
            IsShow = isActive;
            IsFocus = isActive;
            gameObject.SetActive(isActive);
            (isActive ? onShowDone : onHideDone).Invoke();
            (isActive ? onShow : onHide).Invoke();
            (isActive ? onHaveFocus : onLostFocus).Invoke(false);
        }

        public void SetShowOrder(int showOrder)
        {
            ShowOrder = showOrder;
        }

        public void TestShow()
        {
            Show();
        }

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

        public override void SetEvent(Button button, System.Action action)
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

        public void AddActionOnceOnClose(Action action)
        {
            tempOnHideDone.AddListener(()=>
            {
                action?.Invoke();
            });
        }
    }
}
