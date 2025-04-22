using System.Collections.Generic;
using nickeltin.SDF.Runtime;
using UnityEngine;

namespace _IEC.Features.Gameplay.ObserverEventData
{
    [System.Serializable]
    public class CollectItemData
    {
        public string itemID;
        public int number;
        public Vector3 position;
        public int xp;
        public Sprite sprite;

        public int GetCoinValue()
        {
            return _dictCoinByCoinId[itemID];
        }
        
        private Dictionary<string, int> _dictCoinByCoinId = new Dictionary<string, int>()
        {
            {"coin_small", 1},
            {"coin_big", 2},
            {"coin_cat", 5},
            {"coin_chinese", 10},
            {"coin_pig", 500},
        };
    }
}