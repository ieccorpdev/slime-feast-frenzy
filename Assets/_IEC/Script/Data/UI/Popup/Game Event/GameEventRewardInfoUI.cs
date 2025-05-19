using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IEC
{
    public class GameEventRewardInfoUI : _BaseBehaviour
    {
        public ResourceType resourceType;
        public TextMeshProUGUI amountText;

        public void UpdateAmountText(string text)
        {
            switch (resourceType)
            {
                case ResourceType.Gold:
                    amountText.text = text;
                    break;
                case ResourceType.UnlimitedLive:
                    amountText.text = text + "m";
                    break;
                case ResourceType.TimeUnlimitedWindSurf:
                    amountText.text = text + "m";
                    break;
                case ResourceType.TimeUnlimitedEvolveStone:
                    amountText.text = text + "m";
                    break;
                default:
                    amountText.text = "x" + text;
                    break;
            }
        }
    }
}
