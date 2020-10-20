using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace IEC
{
    public class LevelConfigData : MonoBehaviour
    {
        [BoxGroup("Prefab")]
        public GameObject wallObj, gridObj;

        [BoxGroup("Group")]
        public Transform[] wallParents;   // TOP, BOT, LEFT, RIGHT
        public Transform gridParent;

        public int size;

        public void DestroyAll()
        {
            for (int i = 0; i < wallParents.Length; i++)
            {
                var child = wallParents[i];
                var childCount = child.childCount;

                for (int j = 0; j < childCount; j++)
                {
                    DestroyImmediate(child.GetChild(0).gameObject);
                }
            }

            var gridCount = gridParent.childCount;
            for (int i = 0; i < gridCount; i++)
            {
                DestroyImmediate(gridParent.GetChild(0).gameObject);
            }
        }
    }
}

