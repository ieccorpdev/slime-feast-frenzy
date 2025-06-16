using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    public class ToastGroup : _BaseBehaviour, IGroup
    {
        public List<BaseToast> ListToast => SingleCreateIfNotExist("ListToast", () => transform.GetComponentsInChildren<BaseToast>().ToList());

        public ToastNotify Notify => SingleGetChild<ToastNotify>();

        public void LocateGroup()
        {
            Pool.Set(this);
            HideAllToast();
        }

        public void HideAllToast()
        {
            foreach (var t in ListToast)
            {
                t.Hide();
            }
        }
    }
}