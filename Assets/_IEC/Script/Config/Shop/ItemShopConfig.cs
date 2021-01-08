using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/Shop/Item Shop")]
    public class ItemShopConfig : ScriptableObject
    {
        public ItemShopType itemShopType;
        public string name;
        public Sprite spriteIcon;
    }
}