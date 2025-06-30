using System;
using System.Collections.Generic;
using _IEC.Features.ExportLevel.OriginalLevel;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class LevelSpawnData : ScriptableObject
    {
        public List<ItemSpawn> listItemSpawns = new List<ItemSpawn>();
    }

    [Serializable]
    public class ItemSpawn
    {
        public string id;
        public List<ItemSpawnData> listSpawnDatas;
    }

    [Serializable]
    public class ItemSpawnData
    {
        public string groupID;
        public Vector3 groupPosition;
        public Position p;
        public Rotation r;
        public Scale s;
        public bool kinematic;
        public List<Vector3> pathPoints;

        public ItemData ToItemData(string id)
        {
            ItemData itemData = new ItemData();
            itemData.groupID = groupID;
            itemData.groupPosition = groupPosition;
            itemData.itemId = id;
            itemData.position = p.ToVector3();
            itemData.eulerAngle = r.ToVector3();
            itemData.localScale = s.ToVector3();
            itemData.isStartKinematic = kinematic;
            if (pathPoints != null)
            {
                itemData.pathPoints = new List<Vector3>(pathPoints);
            }
            return itemData;
        }
    }
}