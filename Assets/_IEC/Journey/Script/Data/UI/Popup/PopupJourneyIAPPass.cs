using System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupJourneyIAPPass : BasePopup
    {
        public TextMeshProUGUI timePassLeft;

        private void Start()
        {
        }

        public override void Show()
        {
            base.Show();
            UpdateTimeLeftPass();
        }
        void UpdateTimeLeftPass()
        {
            string timeOutput = "";
            //GameController.GetEndTimeJourney(out timeOutput);
            timePassLeft.text = timeOutput;
        }

        private void OnBuyVipPassClick()
        {
            
        }
        private void OnBuySuperVipPassClick()
        {
            
        }
    }
}
