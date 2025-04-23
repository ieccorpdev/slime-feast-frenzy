using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    [CreateAssetMenu(menuName = "Gameplay/LevelDataContainer")]
    public class LevelDataContainerSO : ScriptableObject
    {
        public List<LevelDataSO> listAllLevelData = new();
    }
}
