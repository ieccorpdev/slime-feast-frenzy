using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IEC
{
    public class PlayerStatUI : _BaseBehaviour
    {
        public PlayerGeneralStat statType;
        public TextMeshProUGUI statText;

        public void UpdateText(string text)
        {
            statText.text = text;
        }
    }
}
