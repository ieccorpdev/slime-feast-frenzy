using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseToast : _BaseBehaviour
    {
        public virtual bool IsShowing { get; private set; } = false;

        public virtual void Show()
        {
            IsShowing = true;
            gameObject.SetActive(true);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public virtual void Hide()
        {
            IsShowing = false;
            gameObject.SetActive(false);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}