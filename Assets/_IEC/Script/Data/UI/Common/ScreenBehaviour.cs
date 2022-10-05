using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenBehaviour : _BaseBehaviour
    {
        public virtual void SetEvent(Button button, System.Action action)
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    action?.Invoke();
                });
            }
        }
    }
}