using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenMain : ScreenUI
    {
        [SerializeField] private Button m_BtnSetting;

        private void Start()
        {
            SetEvent(m_BtnSetting, () =>
            {
                UI.ShowScreen(UI.PopupSetting);
            });
        }
    }
}