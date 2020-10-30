using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class ScreenUI : _BaseBehaviour
    {
        public bool isHideAllOtherScreen;
        public List<ScreenUI> listHideHaveEffect;
        public List<ScreenUI> listHideNoEffect;

        public virtual void Show()
        {
            EffectUI.DoShowHide(gameObject, true);
        }

        public virtual void Hide()
        {
            EffectUI.DoShowHide(gameObject, false);
        }

        public virtual void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}