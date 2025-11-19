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
        public List<ItemSpawn> listItemSpawns = new List<ItemSpawn>();

        public void CloneTo(LevelSpawnData spawnData)
        {
            spawnData.listItemSpawns = new List<ItemSpawn>(listItemSpawns);
        }

    [Button]
    public void RandomizeRotations(float minAngle = -10f, float maxAngle = 10f)
    {
        foreach (var item in listItemSpawns)
        {
            foreach (var data in item.listSpawnDatas)
            {
                // lấy rotation hiện tại
                Vector3 rot = data.r.ToVector3();

                // xoay ngẫu nhiên mỗi trục
                rot.x += UnityEngine.Random.Range(minAngle, maxAngle);
                rot.y += UnityEngine.Random.Range(minAngle, maxAngle);
                rot.z += UnityEngine.Random.Range(minAngle, maxAngle);

                data.r = new Rotation(rot.x, rot.y, rot.z);
            }
        }
    }

    [Button]
    public void RandomizeSize()
    {
        foreach (var item in listItemSpawns)
        {
            foreach (var data in item.listSpawnDatas)
            {

                data.s = new Scale(1f, 1f, 1f);
            }
        }
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