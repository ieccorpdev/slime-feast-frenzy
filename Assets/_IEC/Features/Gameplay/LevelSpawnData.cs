using System;
using System.Collections.Generic;
using _IEC.Features.ExportLevel.OriginalLevel;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IEC.Features.Gameplay
{
    public class LevelSpawnData : SerializedScriptableObject
    {
        public List<ItemSpawn> listItemSpawns;

        [Button]
        public void SetAllSpawnDataKinematic()
        {
            foreach (var item in listItemSpawns)
            {
                foreach (var data in item.listSpawnDatas)
                {
                    data.kinematic = true;
                }
            }
        }

        public void CloneTo(LevelSpawnData spawnData)
        {
            spawnData.listItemSpawns = new(listItemSpawns);
        }
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
            return itemData;
        }
    }
}