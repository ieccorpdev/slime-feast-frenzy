using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IEC.Features.Gameplay
{
    public class LevelDataSO : SerializedScriptableObject
    {
        public LevelSpawnData levelSpawnData;
        public TextAsset mapData;
        public List<LevelObjectiveData> listObjectives;
        public int time;
        public string theme; // ground skin
        public string difficulty; // type
        public Vector3 mapCenter;
        public LevelBoomData levelBoomData;
        public bool isOverriddenByPreset;
        public bool isOverridenByMapPrefab;

        public MapData GetMapData(TextAsset file)
        {
            if (file == null)
            {
                return GetMapData();
            }

            return ExportLevel.OriginalLevel.LevelDataAdapter.GetMapData(theme, file.text);
        }

        public MapData GetMapData()
        {
            if (mapData != null)
                return ExportLevel.OriginalLevel.LevelDataAdapter.GetMapData(theme, mapData.text);

            return null;
        }


        public void CloneTo(LevelDataSO levelData)
        {
            levelData.levelSpawnData = levelSpawnData;
            levelData.mapData = mapData;
            levelData.listObjectives = listObjectives.Select(x => x.Clone()).ToList();
            levelData.time = time;
            levelData.theme = theme;
            levelData.difficulty = difficulty;
            levelData.mapCenter = mapCenter;
            levelData.levelBoomData = levelBoomData;
            levelData.isOverriddenByPreset = isOverriddenByPreset;
            levelData.isOverridenByMapPrefab = isOverridenByMapPrefab;
        }

        public void RemakeLevelSpawnData()
        {
            if (levelSpawnData == null) return;
            var newList = new List<ItemSpawn>();

            foreach (var itemSpawn in levelSpawnData.listItemSpawns)
            {
                if (!newList.Exists(x => x.id == itemSpawn.id))
                {
                    newList.Add(new ItemSpawn
                    {
                        id = itemSpawn.id,
                        listSpawnDatas = new List<ItemSpawnData>()
                    });
                }
                
                newList.Find(x => x.id == itemSpawn.id).listSpawnDatas.AddRange(itemSpawn.listSpawnDatas);
            }
            
            levelSpawnData.listItemSpawns = newList;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.EditorUtility.SetDirty(levelSpawnData);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

        }
    }

    [Serializable]
    public class LevelObjectiveData
    {
        [FormerlySerializedAs("itemId")]
        [ValueDropdown(nameof(GetAllItemIds))]
        public string id;
        public int amount;

        public void AddObjectiveAmount(int addAmount)
        {
            this.amount += addAmount;
        }

        public LevelObjectiveData()
        {
        }

        public LevelObjectiveData(string id, int num)
        {
            this.id = id;
            amount = num;
        }

        public LevelObjectiveData Clone()
        {
            return new LevelObjectiveData()
            {
                id = id,
                amount = amount
            };
        }

        public LevelObjective ToLevelObjective()
        {
            return new LevelObjective(id, amount);
        }

        private static IEnumerable GetAllItemIds()
        {
#if UNITY_EDITOR
            var itemConfigPath = UnityEditor.AssetDatabase.FindAssets("t:GameplayItemPrefabConfig");
            if (itemConfigPath.Length == 0)
            {
                Debug.LogError("GameplayItemPrefabConfig not found");
                return null;
            }

            var itemConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<GameplayItemPrefabConfig>
                (UnityEditor.AssetDatabase.GUIDToAssetPath(itemConfigPath[0]));
            return itemConfig.listItemDefinition.Select(x => x.GetId());
#endif
            return null;
        }
    }
}