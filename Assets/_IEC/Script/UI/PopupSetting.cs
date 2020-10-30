using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupSetting : MonoBehaviour
    {
        public Button btnClose, btnSound, btnVibrate;

        public GameObject soundOffObj, vibrateOffObj;

        public void SetStatus(bool isSoundOn, bool isVibrateOn)
        {
            soundOffObj.SetActive(!isSoundOn);
            vibrateOffObj.SetActive(!isVibrateOn);
        }
    }
}
