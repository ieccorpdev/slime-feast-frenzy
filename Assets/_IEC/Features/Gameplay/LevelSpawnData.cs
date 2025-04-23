using System;
using System.Collections.Generic;
using _IEC.Features.ExportLevel.OriginalLevel;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IEC.Features.Gameplay
{
    [Serializable]
    public class LevelSpawnData : ScriptableObject
    {
        public List<ItemSpawn> listItemSpawns = new List<ItemSpawn>();

        public void CloneTo(LevelSpawnData spawnData)
        {
            spawnData.listItemSpawns = new List<ItemSpawn>(listItemSpawns);
        }
    }

    [Serializable]
    public class ItemSpawn
    {
        public string id;
        public List<ItemSpawnData> listSpawnDatas = new List<ItemSpawnData>();
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