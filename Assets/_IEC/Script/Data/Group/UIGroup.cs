using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace IEC
{
    public class UIGroup : _BaseBehaviour, IGroup
    {
        public Canvas canvas;
        public EventSystem eventSystem;
        public TMP_Text textHello;

        public void LocateGroup()
        {
            Pool.Set(this);
        }
    }
}