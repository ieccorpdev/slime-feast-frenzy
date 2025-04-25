using System.Collections.Generic;
using _IEC.Features.ExportLevel.OriginalLevel;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    [System.Serializable]
    public class LevelConfigModel : ScriptableObject
    {
        public LevelSpawnData levelSpawnData;
        public TextAsset itemInstantiateItemData;
        public TextAsset mapData;
        public PrefabMapData prefabMapData;
        public GroundShapePresetParameters presetMapData;
        public List<LevelObjective> levelObjectives;
        public int duration;
        public int durationSave;
        public string groundSkin;
        public string type;
        public int bombWeight;
        public int radioActiveWeight;
        public int negativeTimerWeight;
        public Vector3 mapCenter;
        public LevelDefinition levelDefinition;
        public bool isOverriddenByPreset;
        public bool isOverridenByMapPrefab;
        public List<ItemData> GetItemDataForInstantiate()
        {
            return ExportLevel.OriginalLevel.LevelDataAdapter.GetAllItemData(itemInstantiateItemData.text);
        }

        public MapData GetMapData(TextAsset file)
        {
            if (file == null)
            {
                return GetMapData();
            }
            
            return ExportLevel.OriginalLevel.LevelDataAdapter.GetMapData(groundSkin, file.text);
        }
        
        public MapData GetMapData(GroundShapePresetParameters groundPreset)
        {
            if (groundPreset == null)
            {
                return GetMapData();
            }
            
            return ExportLevel.OriginalLevel.LevelDataAdapter.GetMapData(groundSkin, groundPreset);
        }
        
        public MapData GetMapData()
        {
            if (isOverriddenByPreset && presetMapData != null)
            {
                return ExportLevel.OriginalLevel.LevelDataAdapter.GetMapData(groundSkin, presetMapData);
            }
            
            if(mapData!= null)
                return ExportLevel.OriginalLevel.LevelDataAdapter.GetMapData(groundSkin, mapData.text);
            if (presetMapData != null)
            {
                return ExportLevel.OriginalLevel.LevelDataAdapter.GetMapData(groundSkin, presetMapData);
            }
            return null;
        }

        public void CloneTo(LevelConfigModel clone)
        {
            clone.itemInstantiateItemData = itemInstantiateItemData;
            clone.mapData = mapData;
            clone.presetMapData = presetMapData;
            clone.levelObjectives = new List<LevelObjective>();
            foreach (var levelObjective in levelObjectives)
            {
                clone.levelObjectives.Add(levelObjective.Clone());
            }
            clone.duration = duration;
            clone.durationSave = durationSave;
            clone.groundSkin = groundSkin;
            clone.type = type;
            clone.bombWeight = bombWeight;
            clone.radioActiveWeight = radioActiveWeight;
            clone.negativeTimerWeight = negativeTimerWeight;
            clone.mapCenter = mapCenter;
            clone.levelDefinition = levelDefinition;
            clone.isOverriddenByPreset = isOverriddenByPreset;
            
        }

        public void ConvertToLevelSpawnData(int levelIndex, string folderName)
        {
            var data = ScriptableObject.CreateInstance<LevelSpawnData>();
            data.listItemSpawns = new List<ItemSpawn>();
            
            var itemList = GetItemDataForInstantiate();
            foreach (var item in itemList)
            {
                var itemSpawn = new ItemSpawn();
                itemSpawn.id = item.itemId;
                itemSpawn.listSpawnDatas = new List<ItemSpawnData>();
                var itemSpawnData = new ItemSpawnData();
                itemSpawnData.p = new Position(item.position);
                itemSpawnData.r = new Rotation(item.eulerAngle);
                itemSpawnData.s = new Scale(item.localScale);
                itemSpawnData.kinematic = item.isStartKinematic;
                itemSpawn.listSpawnDatas.Add(itemSpawnData);
                data.listItemSpawns.Add(itemSpawn);    
            }
            
            levelSpawnData = data;
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(data, $"Assets/_IECProject/FSLevels/_New_{folderName}/LevelSpawnDatas/LevelSpawnData_{levelIndex}.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public LevelDataSO CreateLevelDataSO(int levelIndex, string folderName)
        {
            var levelDataSO = ScriptableObject.CreateInstance<LevelDataSO>();
            
            levelDataSO.levelSpawnData = levelSpawnData;
            levelDataSO.mapData = mapData;
            levelDataSO.listObjectives = new List<LevelObjectiveData>();
            foreach (var levelObjective in levelObjectives)
            {
                levelDataSO.listObjectives.Add(levelObjective.ToLevelObjectiveData());
            }
            levelDataSO.time = duration;
            levelDataSO.theme = groundSkin;
            levelDataSO.difficulty = type;
            levelDataSO.mapCenter = mapCenter;
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(levelDataSO, $"Assets/_IECProject/FSLevels/_New_{folderName}/LevelDataSOs/LevelDataSO_{levelIndex}.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif

            return levelDataSO;
        }
        
    }
}