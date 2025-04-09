using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ItemUI : _BaseBehaviour
    {
        public Image img;
        public TextMeshProUGUI quantityText;
        
        public void SetData(Sprite sprite, string txt)
        {
            img.sprite = sprite;
            quantityText.text = "x" + txt;
        }
    }
}