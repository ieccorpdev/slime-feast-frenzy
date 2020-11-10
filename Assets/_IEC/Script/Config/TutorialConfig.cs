using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/Tutorial Config")]
    public class TutorialConfig : ScriptableObject
    {
        public int level;

        public List<RotTutorialConfig> listRotConfigs;
    }

    [System.Serializable]
    public class RotTutorialConfig 
    {
        public int row, col;
        public int rotCount;
    }
}