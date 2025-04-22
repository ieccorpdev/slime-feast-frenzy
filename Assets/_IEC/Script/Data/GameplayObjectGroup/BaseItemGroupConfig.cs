using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/ChestConfig")]
    public class BaseItemGroupConfig : ScriptableObject
    {
        public List<BaseItemGroup> listBaseItemGroup = new();
    }
}
