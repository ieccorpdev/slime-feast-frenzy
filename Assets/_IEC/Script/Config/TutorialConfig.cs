using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/Tutorial Config")]
    public class TutorialConfig : ScriptableObject
    {
        public int level;

        public List<MoveConfig> listMoveConfigs;
    }

    [System.Serializable]
    public class MoveConfig 
    {
        public int row, col;
        public int rotCount;
    }
}