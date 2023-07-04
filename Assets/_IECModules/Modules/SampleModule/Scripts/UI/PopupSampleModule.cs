using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC.SampleModule
{
    public class PopupSampleModule : BasePopup
    {
        SampleModuleController Controller => Controllers.Get<SampleModuleController>();

        private void Start()
        {
        }
        public override void Show()
        {
            base.Show();
        }

    }
}