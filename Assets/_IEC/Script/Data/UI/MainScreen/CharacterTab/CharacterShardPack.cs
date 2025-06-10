using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class CharacterShardPack : _BaseBehaviour
    {
        public Image characterMainImage;
        public TextMeshProUGUI shardAmountText;

        [Header("Buy Group")] 
        public Button buyButton;
        public TextMeshProUGUI goldText;

        [Header("Bought Group")] 
        public GameObject boughtGroup;

        [Header("Lock Group")] 
        public GameObject lockGroup;
        public TextMeshProUGUI lockLevelText;
    }
}
