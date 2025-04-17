using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/AllBoosterConfig")]
    public class AllBoosterConfig : ScriptableObject
    {
        public int levelUnlockStreak;

        public List<BoosterData> listBoosterData = new();
        
        public List<BoosterStateSprite> listBoosterStateSprite;

        public Sprite GetBgSprite(BoosterBaseState boosterBaseState)
        {
            return listBoosterStateSprite.Find(state => state.state == boosterBaseState).bgSprite;
        }

        public BoosterData GetBoosterData(ResourceType boosterType)
        {
            return listBoosterData.Find(data => data.boosterType == boosterType);
        }
    }

    [Serializable]
    public class BoosterData
    {
        public ResourceType boosterType;
        public int levelUnlock;

    }

    [Serializable]
    public class BoosterStateSprite
    {
        public BoosterBaseState state;
        public Sprite bgSprite;
    }
}
