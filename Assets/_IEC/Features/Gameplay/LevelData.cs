using System.Collections.Generic;
using UnityEngine.Serialization;

namespace _IEC.Features.Gameplay
{
    [System.Serializable]
    public class LevelData
    {
        public List<LevelObjective> levelObjectives;
        public float time;
        public LevelType levelType;
        public int level;
    }
}