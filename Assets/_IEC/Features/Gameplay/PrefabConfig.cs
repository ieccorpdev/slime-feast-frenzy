using IEC;
using TMPro;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    [CreateAssetMenu(menuName = "Gameplay/PrefabConfig")]
    public class PrefabConfig : ScriptableObject
    {
        public XpVFX xpVFX;
        public LevelObjectiveVfx levelObjectiveVfx;
        public BaseItemDirectorUI itemDirectorUI;
        public BombItem bombPrefab;
        public GameObject wallBouyOcean;
        public ParticleSystem vfxItemSplash;
    }
}