using IEC;
using TMPro;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    [CreateAssetMenu(menuName = "Game/Features/Gameplay/PrefabConfig")]
    public class PrefabConfig : ScriptableObject
    {
        public XpVFX xpVFX;
        public LevelObjectiveVfx levelObjectiveVfx;
        public BaseItemDirectorUI itemDirectorUI;
    }
}