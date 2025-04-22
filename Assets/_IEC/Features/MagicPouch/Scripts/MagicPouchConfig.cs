using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Gameplay;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/Features/Booster/MagicPouchConfig")]
    public class MagicPouchConfig : ScriptableObject
    {
        public int levelUnlock = 19;
        public float maxDistanceFromHole = 50f;
        public List<MagicPouchBooster> boosters;
    }

    [Serializable]
    public struct MagicPouchBooster
    {
        public MagicPouchBoosterType type;
        public float effectValue;
        public ItemDefinition itemDefinition;
    }
    
    public enum MagicPouchBoosterType
    {
        None,
        Chrono,
        Lightning,
    }
}
