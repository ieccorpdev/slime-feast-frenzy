using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    public class GroupPopup : BaseBehaviour
    {
       //Example public PopupSetting PopupSetting => SingleGetChild<PopupSetting>();
        public List<BasePopup> ListPopup =>
            SingleCreateIfNotExist(() => GetComponentsInChildren<BasePopup>(true).ToList());

    }
}