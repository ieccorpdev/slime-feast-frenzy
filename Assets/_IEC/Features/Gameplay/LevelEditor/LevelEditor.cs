using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _IEC.Features.Common.Extension;
using _IEC.Features.ExportLevel.OriginalLevel;
using Sirenix.OdinInspector;
using UnityEngine;
using _IEC.Features.Gameplay;
using IEC.Linq;
using Newtonsoft.Json;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Vector3 = System.Numerics.Vector3;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IEC
{
    [ExecuteAlways]
    public class LevelEditor : SerializedMonoBehaviour
    {
#if UNITY_EDITOR
        private void OnEnable()
        {
            Selection.selectionChanged += FilterSelection;
        }

        //[ReadOnly] public string currentMapData;
        [ReadOnly] public LevelDataSO currentLevelData;

        public LevelConfig currentLevelConfig;
        //[FoldoutGroup(LevelEditorConstants.SCENE_ITEMS)]
        [Button]
        public void PlayTestLevel()
        {
            currentLevelConfig.playTestData = currentLevelData;
            EditorPrefs.SetBool("isPlayTest", true);
            EditorApplication.EnterPlaymode();
        }
        
        #region Item Selection
        
        private static System.Type filterType = typeof(SelectionBaseHolder);

        [FoldoutGroup(LevelEditorConstants.SCENE_ITEMS)]
        public bool isAllowSelectSceneItems;

        [FoldoutGroup(LevelEditorConstants.SCENE_ITEMS)]
        [ShowIf("@isAllowSelectSceneItems")]
        [ReadOnly] 
        public int totalXP;

        private void UpdateTotalXP()
        {
            totalXP = System.Linq.Enumerable.Sum(filteredObjects, obj => obj.ItemDefinition.GetXp());
        }
        
        [FoldoutGroup(LevelEditorConstants.SCENE_ITEMS)]
        [ShowIf("@isAllowSelectSceneItems")]
        [ReadOnly] public List<BaseItem> filteredObjects = new ();

        [FoldoutGroup(LevelEditorConstants.SCENE_ITEMS)] 
        [ShowIf("@isAllowSelectSceneItems")]
        public string groupItemName;

        [FoldoutGroup(LevelEditorConstants.SCENE_ITEMS)]
        [ShowIf("@isAllowSelectSceneItems")]
        [Button]
        public void GroupItems()
        {
            if (string.IsNullOrEmpty(groupItemName))
            {
                EditorUtility.DisplayDialog("Group Object name is Empty !!!", "Give a name pls", "Sorry :(");
                return;
            }
            
            if (itemHolder.GetComponentsInChildren<Transform>().Find(child => child.name == groupItemName))
            {
                EditorUtility.DisplayDialog($"Object with name {groupItemName} already exist!!!", "Give a different name pls", "Sorry :(");
                return;
            }
            
            GameObject groupHolder = new GameObject(groupItemName);
            groupHolder.transform.parent = itemHolder;

            UnityEngine.Vector3 sum = UnityEngine.Vector3.zero;
            foreach (var obj in filteredObjects)
            {
                sum += obj.transform.position;
            }
            var centerPoint = sum / filteredObjects.Count;

            groupHolder.transform.position = centerPoint;
            
            foreach (var obj in filteredObjects)
            {
                obj.ItemData.groupID = groupItemName;
                obj.ItemData.groupPosition = centerPoint;
                obj.transform.parent = groupHolder.transform;
            }
        }
        
        private void FilterSelection()
        {
            if (!isAllowSelectSceneItems)
                return;
            
            if (Selection.objects.Length == 0) return;

            filteredObjects = new ();

            foreach (Object obj in Selection.objects)
            {
                if (obj is GameObject gameObject)
                {
                    var target = gameObject.GetComponent(typeof(BaseItem)) as BaseItem;

                    if (target == null)
                        target = gameObject.GetComponentInParent(typeof(BaseItem)) as BaseItem;

                    if (target != null && target.ItemDefinition != null)
                    {
                        filteredObjects.Add(target);
                    }
                }
            }
            
            // Selection.objects = Array.Empty<Object>();
            // Selection.objects = filteredObjects.ToArray() as Object[];
            UpdateTotalXP();
        }
        #endregion

        [FoldoutGroup(LevelEditorConstants.LEVEL_AND_THEME)]
        public int level = 1;
        
        [FoldoutGroup(LevelEditorConstants.LEVEL_AND_THEME)]
        public int levelTime = 0;

        [FoldoutGroup(LevelEditorConstants.LEVEL_AND_THEME)]
        public bool createNewLevel;
        
        [FoldoutGroup(LevelEditorConstants.LEVEL_AND_THEME)]
        [ShowIf("@createNewLevel")]
        public string filePath = "Assets/EncryptedAssets";
        
        [FoldoutGroup(LevelEditorConstants.LEVEL_AND_THEME)]
        [ShowIf("@createNewLevel")]
        public string levelFolderName = "CustomLevel";
        
        [FoldoutGroup(LevelEditorConstants.LEVEL_AND_THEME)]
        [ShowIf("@createNewLevel")]
        public string levelObjectName = "LevelDataSO";
        
        [FoldoutGroup(LevelEditorConstants.LEVEL_AND_THEME)]
        [ShowIf("@createNewLevel")]
        public string levelSpawnDataFolderName = "CustomSpawnData";
        
        [FoldoutGroup(LevelEditorConstants.LEVEL_AND_THEME)]
        [ShowIf("@createNewLevel")]
        public string levelSpawnDataObjectName = "LevelSpawnData";
        
        // [FoldoutGroup(LevelEditorConstants.LEVEL_AND_THEME)]
        // [ShowIf("@createNewLevel")]
        // public string mapDataFolderName = "CustomMapData";
        //
        // [FoldoutGroup(LevelEditorConstants.LEVEL_AND_THEME)]
        // [ShowIf("@createNewLevel")]
        // public string mapDataObjectName = "MapData";

        [FoldoutGroup(LevelEditorConstants.LEVEL_AND_THEME)]
        [ShowIf("@createNewLevel")]
        [Button]
        public void CreateNewLevel()
        {
            LevelDataSO newLevelDataSO = ScriptableObject.CreateInstance<LevelDataSO>();

            LevelSpawnData newLevelSpawnDataSO = ScriptableObject.CreateInstance<LevelSpawnData>();

            if (currentLevelData != null)
            {
                currentLevelData.CloneTo(newLevelDataSO);
                currentLevelData.levelSpawnData.CloneTo(newLevelSpawnDataSO);
            }
            
            newLevelDataSO.levelSpawnData = newLevelSpawnDataSO;

            string levelDataPath = $"{filePath}/{levelFolderName}";
            if (!AssetDatabase.IsValidFolder(levelDataPath))
            {
                AssetDatabase.CreateFolder(filePath, levelFolderName);
            }
            
            AssetDatabase.CreateAsset(newLevelDataSO, $"{levelDataPath}/{levelObjectName}_{level}.asset");
            
            string levelSpawnDataPath = $"{filePath}/{levelSpawnDataFolderName}";
            if (!AssetDatabase.IsValidFolder(levelSpawnDataPath))
            {
                AssetDatabase.CreateFolder(filePath, levelSpawnDataFolderName);
            }
            
            AssetDatabase.CreateAsset(newLevelSpawnDataSO, $"{levelSpawnDataPath}/{levelSpawnDataObjectName}_{level}.asset");
            
            
            // string mapDataPath = $"{filePath}/{mapDataFolderName}";
            // if (!AssetDatabase.IsValidFolder(mapDataPath))
            // {
            //     AssetDatabase.CreateFolder(filePath, mapDataFolderName);
            // }
            //
            // string newMapDataAssetPath = $"{mapDataPath}/{levelSpawnDataObjectName}_{level}.txt";
            // if (!File.Exists(newMapDataAssetPath))
            // {
            //     File.WriteAllText(newMapDataAssetPath, newLevelDataSO.mapData != null ? newLevelDataSO.mapData.text : "");
            //     AssetDatabase.Refresh();
            // }
            //
            // TextAsset newMapDataAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(newMapDataAssetPath);
            // newLevelDataSO.mapData = newMapDataAsset;
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            levelDataSo = newLevelDataSO;
            ReloadLevel();

        }
        
        [FoldoutGroup(LevelEditorConstants.LEVEL_AND_THEME)]
        [ShowIf("@!createNewLevel")]
        public LevelDataSO levelDataSo;

        [FoldoutGroup(LevelEditorConstants.LEVEL_AND_THEME)]
        [ValueDropdown(nameof(GetAllThemes))]
        public string theme;

        // [FoldoutGroup(LevelEditorConstants.CREATIVE)]
        // public bool isLevelCreative;

        // [FoldoutGroup(LevelEditorConstants.CREATIVE)]
        // [Button]
        // [ShowIf("@isLevelCreative")]
        // void SaveOverrideCreative()
        // {
        //     if (currentLevelData == null)
        //     {
        //         Debug.LogError("There is no level currently selected");
        //         return;
        //     }
        //
        //     creativeName = currentLevelData.name;
        //     if (EditorUtility.DisplayDialog("Save Override Level", $"Override this level {creativeName}?", "Yes", "No"))
        //     {
        //         ValidatePosition();
        //         ValidateData();
        //         if (!ValidateObjectives()) return;
        //         Root root = new Root();
        //         root.InstantiateData = new List<InstantiateData>();
        //         var baseItems = itemHolder.GetComponentsInChildren<BaseItem>();
        //         foreach (var baseItem in baseItems)
        //         {
        //             //var baseItem = item.GetComponent<BaseItem>();
        //             if (baseItem == null || baseItem.ItemDefinition == null)
        //             {
        //                 continue;
        //             }
        //
        //             var id = baseItem.GetItemId();
        //             var instantiatedData = root.InstantiateData.Find(x => x.itemId == id);
        //             if (instantiatedData == null)
        //             {
        //                 instantiatedData = new InstantiateData
        //                 {
        //                     itemId = id,
        //                     itemData = new List<_IEC.Features.ExportLevel.OriginalLevel.ItemData>()
        //                 };
        //
        //                 root.InstantiateData.Add(instantiatedData);
        //             }
        //
        //             var itemData = new _IEC.Features.ExportLevel.OriginalLevel.ItemData();
        //             itemData.pos = new Position(baseItem.transform.position);
        //             itemData.rot = new Rotation(baseItem.transform.eulerAngles);
        //             itemData.scale = new Scale(baseItem.transform.lossyScale);
        //             itemData.sk = baseItem.ItemData.isStartKinematic;
        //
        //             instantiatedData.itemData.Add(itemData);
        //         }
        //
        //         if (isOverrideBombData && m_AllBombs.Count > 0)
        //             currentLevelData.levelBoomData = new LevelBoomData(isOverrideBombData, GetListBombDatas());
        //         else
        //             currentLevelData.levelBoomData = new LevelBoomData(levelBombData);
        //
        //         currentLevelData.theme = theme;
        //         currentLevelData.listObjectives = levelObjectives.ConvertAll(x => x.Clone());
        //
        //         if (mapData != null && isOverrideMapData)
        //         {
        //             currentLevelData.isOverriddenByPreset = false;
        //             currentLevelData.mapData = mapData;
        //         }
        //
        //         SaveCreativeLevelData(root, true);
        //     }
        // }

        // [FoldoutGroup(LevelEditorConstants.CREATIVE)]
        // [Button]
        // [ShowIf("@isLevelCreative")]
        // void SaveData()
        // {
        //     if (EditorUtility.DisplayDialog("Save Level", $"Save this level {creativeName}?", "Yes", "No"))
        //     {
        //         ValidatePosition();
        //         ValidateData();
        //         if (!ValidateObjectives()) return;
        //         Root root = new Root();
        //         root.InstantiateData = new List<InstantiateData>();
        //         var baseItems = itemHolder.GetComponentsInChildren<BaseItem>();
        //         foreach (var baseItem in baseItems)
        //         {
        //             //var baseItem = item.GetComponent<BaseItem>();
        //             if (baseItem == null || baseItem.ItemDefinition == null)
        //             {
        //                 continue;
        //             }
        //
        //             var id = baseItem.GetItemId();
        //             var instantiatedData = root.InstantiateData.Find(x => x.itemId == id);
        //             if (instantiatedData == null)
        //             {
        //                 instantiatedData = new InstantiateData
        //                 {
        //                     itemId = id,
        //                     itemData = new List<_IEC.Features.ExportLevel.OriginalLevel.ItemData>()
        //                 };
        //
        //                 root.InstantiateData.Add(instantiatedData);
        //             }
        //
        //             var itemData = new _IEC.Features.ExportLevel.OriginalLevel.ItemData();
        //             itemData.pos = new Position(baseItem.transform.position);
        //             itemData.rot = new Rotation(baseItem.transform.eulerAngles);
        //             itemData.scale = new Scale(baseItem.transform.lossyScale);
        //             itemData.sk = baseItem.ItemData.isStartKinematic;
        //
        //             instantiatedData.itemData.Add(itemData);
        //         }
        //
        //         if (isOverrideBombData && m_AllBombs.Count > 0)
        //             currentLevelData.levelBoomData = new LevelBoomData(isOverrideBombData, GetListBombDatas());
        //         else
        //             currentLevelData.levelBoomData = new LevelBoomData(levelBombData);
        //
        //         currentLevelData.theme = theme;
        //         currentLevelData.listObjectives = levelObjectives.ConvertAll(x => x.Clone());
        //
        //         if (mapData != null && isOverrideMapData)
        //         {
        //             currentLevelData.isOverriddenByPreset = false;
        //             currentLevelData.mapData = mapData;
        //         }
        //
        //         SaveCreativeLevelData(root, false);
        //     }
        // }

        // private void SaveCreativeLevelData(Root root, bool isOverride)
        // {
        //     var json = JsonConvert.SerializeObject(root);
        //     var pathFolder = Path.Combine(Application.dataPath, PATH_LEVEL_CREATIVE_ITEM_DATA);
        //     var pathFileItemConfig = Path.Combine(pathFolder, creativeName + ".txt");
        //     pathFileItemConfig = pathFileItemConfig.Replace('\\', '/');
        //
        //     if (!Directory.Exists(pathFolder))
        //     {
        //         Directory.CreateDirectory(pathFolder);
        //         AssetDatabase.Refresh();
        //     }
        //
        //     File.WriteAllText(pathFileItemConfig, json);
        //     AssetDatabase.SaveAssets();
        //     AssetDatabase.Refresh();
        //
        //     LevelConfigModel creativeLevelConfig = UnityEngine.Object.Instantiate(levelConfig.GetLevelDataToys(level));
        //     creativeLevelConfig.itemInstantiateItemData =
        //         Utilities.FindAssetInFolder<TextAsset>(PATH_LEVEL_CREATIVE_ITEM_DATA, creativeName);
        //     creativeLevelConfig.listObjectives.Clear();
        //     foreach (var data in root.InstantiateData)
        //     {
        //         creativeLevelConfig.listObjectives.Add(new LevelObjective(data.itemId, data.itemData.Count));
        //     }
        //
        //     creativeLevelConfig.duration = 500;
        //
        //     creativeLevelConfig.name = creativeName + ".asset";
        //
        //     string assetPath = "Assets/" + PATH_LEVEL_CREATIVE + "/" + creativeName + ".asset";
        //     assetPath = assetPath.Replace('\\', '/');
        //
        //     string fullPath = Path.Combine(Application.dataPath, PATH_LEVEL_CREATIVE);
        //     if (!Directory.Exists(fullPath))
        //     {
        //         Directory.CreateDirectory(fullPath);
        //         AssetDatabase.Refresh();
        //     }
        //
        //     if (File.Exists(assetPath))
        //     {
        //         if (!isOverride)
        //         {
        //             EditorUtility.DisplayDialog("Level exist!", $"Level exist with name {creativeName}", "Ok");
        //             return;
        //         }
        //     }
        //
        //     AssetDatabase.CreateAsset(creativeLevelConfig, assetPath);
        //     AssetDatabase.SaveAssets();
        //     AssetDatabase.Refresh();
        //
        //     Debug.Log($"Created asset at: {assetPath}");
        // }

        // [FoldoutGroup(LevelEditorConstants.CREATIVE)]
        // public string creativeName = "CreativeLevel_";

        [FoldoutGroup(LevelEditorConstants.SPAWN_ITEMS)]
        [ValueDropdown(nameof(GetAllItemIds))] [OnValueChanged(nameof(UpdateItemSpawnUtilities))]
        public string itemId;
        
        // [FoldoutGroup(LevelEditorConstants.SPAWN_ITEMS)]
        // [ValueDropdown(nameof(GetAllPillarItemIds))]
        // [ShowIf("@isSpawnPillar")]
        // public string itemPillarId;
        //
        // [FoldoutGroup(LevelEditorConstants.SPAWN_ITEMS)]
        // [ValueDropdown(nameof(GetAllPillarItemIds))]
        // [ShowIf("@isSpawnPillar")]
        // public string itemPillarTopId;

        //private bool isSpawnPillar => spawnArea is PillarSpawnArea;

        [FoldoutGroup(LevelEditorConstants.SPAWN_ITEMS)]
        public SpawnArea spawnArea;

        [FoldoutGroup(LevelEditorConstants.SPAWN_ITEMS)]
        [Button]
        public void SpawnPreviewItems()
        {
            itemSpawnUtilities.spawnArea = spawnArea;
            itemSpawnUtilities.CreatePreview();
        }

        [FoldoutGroup(LevelEditorConstants.SPAWN_ITEMS)]
        [Button]
        public void ApplyPreviewItems()
        {
            itemSpawnUtilities.Apply();
        }

        [FoldoutGroup(LevelEditorConstants.SPAWN_ITEMS)]
        [Button]
        public void ClearPreviewItems()
        {
            itemSpawnUtilities.Clear();
        }

        [FoldoutGroup(LevelEditorConstants.SPAWN_ITEMS)]
        [Button]
        public void ValidatePosition()
        {
            var baseItems = itemHolder.GetComponentsInChildren<BaseItem>();
            foreach (var item in baseItems)
            {
                var spawnSettings = item.GetComponent<SpawnSettings>();
                if (spawnSettings == null) continue;
                spawnSettings.PreserveOffset();
            }
        }

        [FoldoutGroup(LevelEditorConstants.SPAWN_ITEMS)]
        [Button]
        public void ValidateData()
        {
            var baseItems = itemHolder.GetComponentsInChildren<BaseItem>();
            Debug.LogError(baseItems.Length);
            foreach (var item in baseItems)
            {
                if (item.ItemData != null && !string.IsNullOrEmpty(item.ItemData.itemId)) continue;
                var id = item.name;
                int index = id.IndexOf("(Clone)", StringComparison.Ordinal);
                if (index >= 0)
                {
                    id = item.name.Substring(0, index);
                }

                var itemDefinition = itemConfig.listItemDefinition.Find(x => x.GetId() == id);
                if (itemDefinition == null)
                {
                    Debug.LogError("Missing item definition: " + id);
                    continue;
                }

                var itemData = new _IEC.Features.Gameplay.ItemData
                {
                    itemId = id
                };
                item.LoadData(itemData, itemDefinition);
            }
        }

        [FoldoutGroup(LevelEditorConstants.REPLACE_ITEMS)]
        [ValueDropdown(nameof(GetAllCurrentItemIds))]
        public string oldItemID;

        [FoldoutGroup(LevelEditorConstants.REPLACE_ITEMS)]
        public bool isReplaceItemWithSameXP;

        [FoldoutGroup(LevelEditorConstants.REPLACE_ITEMS)]
        [ShowIf("@!isReplaceItemWithSameXP")]
        [ValueDropdown(nameof(GetAllItemIds))]
        public string newItemID;
        
        [FoldoutGroup(LevelEditorConstants.REPLACE_ITEMS)]
        [ShowIf("@isReplaceItemWithSameXP")]
        [ValueDropdown(nameof(GetAllItemIdsWithSameXP))]
        public string newItemIDWithSameXP;

        [FoldoutGroup(LevelEditorConstants.REPLACE_ITEMS)]
        [Button]
        public void ReplaceItems()
        {
            
            var newID = isReplaceItemWithSameXP ? newItemIDWithSameXP : newItemID;
            
            var newItem = itemConfig.listItemDefinition.Find(x => x.GetId() == newID);
            if (newItem == null)
            {
                Debug.LogError($"Item with id {newID} not found");
                return;
            }

            var currentItems = itemHolder.GetComponentsInChildren<BaseItem>();
            var newItemDatas = new List<_IEC.Features.Gameplay.ItemData>();
            foreach (var item in currentItems)
            {
                if (item.GetItemId() == oldItemID)
                {
                    newItemDatas.Add(new _IEC.Features.Gameplay.ItemData()
                    {
                        itemId = newID,
                        position = item.transform.position,
                        eulerAngle = item.transform.eulerAngles,
                        isStartKinematic = item.ItemData.isStartKinematic
                    });

                    DestroyImmediate(item.gameObject);
                }
            }

            LevelHelper.SpawnAllItem(newItemDatas, itemHolder, levelConfig, itemConfig, 0);
        }

        [FoldoutGroup(LevelEditorConstants.ITEM_EXP)]
        [ValueDropdown(nameof(GetAllCurrentItemIds))]
        public List<string> listItemIds;

        [FoldoutGroup(LevelEditorConstants.ITEM_EXP)]
        public int xp;

        [FoldoutGroup(LevelEditorConstants.ITEM_EXP)]
        [Button]
        public void UpdateItemsXp()
        {
            foreach (var itemId in listItemIds)
            {
                var item = itemConfig.listItemDefinition.Find(x => x.GetId() == itemId);
                if (item == null)
                {
                    Debug.LogError($"Item with id {itemId} not found");
                    continue;
                }

                item.SetXp(xp);
                EditorUtility.SetDirty(item);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [FoldoutGroup(LevelEditorConstants.OBJECTIVES)]
        [ReadOnly]
        public List<LevelObjectiveData> levelObjectives;

        // [FoldoutGroup(LevelEditorConstants.OBJECTIVES)]
        // [OnValueChanged(nameof(UpdateItemsInScene))]
        // public List<ItemCountInScene> listItemInScenes;

        [FoldoutGroup(LevelEditorConstants.OBJECTIVES)] 
        [ValueDropdown(nameof(GetAllCurrentItemIds))]
        [OnValueChanged(nameof(UpdateValueObjectiveCount))]
        public string objectiveItemID;

        private void UpdateValueObjectiveCount()
        {
            objectiveCount = GetCountObjectiveItem();
        }
        
        private int GetCountObjectiveItem()
        {
            return
                itemHolder.GetComponentsInChildren<BaseItem>()
                    .Count(item => item.ItemDefinition != null && item.ItemDefinition.GetId() == objectiveItemID);
        }

        [FoldoutGroup(LevelEditorConstants.OBJECTIVES)]
        [PropertyRange(0, nameof(GetCountObjectiveItem))]
        public int objectiveCount;

        [FoldoutGroup(LevelEditorConstants.OBJECTIVES)]
        [Button]
        public void UpdateObjective()
        {
            var objective = levelObjectives.Find(obj => obj.id == objectiveItemID);
            if (objective == null)
            {
                objective = new LevelObjectiveData()
                {
                    id = objectiveItemID,
                };
                levelObjectives.Add(objective);
            }

            if (objectiveCount == 0)
                levelObjectives.Remove(objective);
            
            
            //objective.AddObjectiveAmount(objectiveCount);
            objective.amount = objectiveCount;

        }

        [FoldoutGroup(LevelEditorConstants.OBJECTIVES)]
        [Button]
        public void ResetObjectives()
        {
            levelObjectives.Clear();
        }
        
        // private void UpdateItemsInScene()
        // {
        //     foreach (var item in listItemInScenes)
        //     {
        //         item.itemHolder = itemHolder;
        //         item.itemConfig = itemConfig;
        //     }
        // }

        [FoldoutGroup(LevelEditorConstants.OBJECTIVES)]
        [Button]
        private bool ValidateObjectives()
        {
            var baseItems = itemHolder.GetComponentsInChildren<BaseItem>();
            foreach (var objective in levelObjectives)
            {
                int count = 0;
                foreach (var item in baseItems)
                {
                    if (item.ItemDefinition == null) continue;
                    if (item.GetItemId() == objective.id)
                    {
                        count++;
                    }
                }

                if (count < objective.amount)
                {
                    EditorUtility.DisplayDialog("Invalid Objective",
                        $"Invalid Object Count - {objective.id} | {count}/{objective.amount}", "OK");
                    return false;
                }
            }

            Debug.Log("<color=green> Valid Objectives </color>");
            return true;
        }

        [FoldoutGroup(LevelEditorConstants.MAP)]
        [OnValueChanged(nameof(ValidateMapData))]
        public bool isOverrideMapData;

        [FoldoutGroup(LevelEditorConstants.MAP)]
        [ShowIf("@isOverrideMapData")]
        // [ValueDropdown(nameof(GetAllMapData))]
        public TextAsset mapData;


        [FoldoutGroup(LevelEditorConstants.MAP)]
        [Button] [ShowIf("@isOverrideMapData")]
        public void SaveCurrentMapEditor()
        {
            var groundJsonData = mapEditor.ProcessTilemap();
            var groundRoot = new GroundRoot
            {
                GroundJson = groundJsonData
            };
            var path = EditorUtility.SaveFilePanelInProject("Save Map Data", $"MapData_{level}", "txt", "Save Map Data",
                PATH_MAP);

            if (string.IsNullOrEmpty(path))
                return;


            File.WriteAllText(path, groundRoot.JsonSerialize());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            var mapAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (mapAsset != null)
            {
                mapData = mapAsset;
            }
        }

        [FoldoutGroup(LevelEditorConstants.MAP)]
        [Button] [ShowIf("@isOverrideMapData")]
        public void ClearMap()
        {
            mapEditor.ClearMap();
        }

        [FoldoutGroup(LevelEditorConstants.MAP)]
        [Button] [ShowIf("@isOverrideMapData")]
        public void ToggleMapOnOff()
        {
            mapEditor.gameObject.SetActive(!mapEditor.gameObject.activeSelf);
        }

        [FoldoutGroup(LevelEditorConstants.MAP)]
        [OnValueChanged(nameof(ValidateMapData))]
        public bool isOverrideMapPreset;

        [FoldoutGroup(LevelEditorConstants.MAP)]
        [ShowIf("@isOverrideMapPreset")] [ValueDropdown(nameof(GetAllPreset))]
        public GroundShapePresetParameters presetMapData;

        [FoldoutGroup(LevelEditorConstants.MAP)]
        [OnValueChanged(nameof(ValidateMapData))]
        public bool isOverrideMapPrefab;

        [FoldoutGroup(LevelEditorConstants.MAP)]
        [ShowIf("@isOverrideMapPrefab")]
        public PrefabMapData prefabMapData = new PrefabMapData();

        [FoldoutGroup(LevelEditorConstants.MAP)]
        [Button] [ShowIf("@isOverrideMapData")]
        public void LoadMapEditorFromMapData()
        {
            if (mapData == null)
            {
                Debug.LogError("Map data is null");
                return;
            }

            mapEditor.ClearMap();
            var groundRoot = JsonConvert.DeserializeObject<GroundRoot>(mapData.text);
            if (groundRoot == null)
            {
                Debug.LogError("Ground root is null");
                return;
            }

            mapEditor.LoadMap(groundRoot.GroundJson);
        }

        [FoldoutGroup(LevelEditorConstants.MAP)]
        [Button] [ShowIf("@isOverrideMapPrefab")]
        public void UpdatePrefabMapData()
        {
            if (prefabMapData.mapPrefab == null)
            {
                Debug.LogError("Map prefab is null");
                return;
            }

            var currentMap = groundHolder.transform.GetChild(0);
            if (currentMap == null)
            {
                Debug.LogError("Map is not spawned yet");
                return;
            }

            var currentMapName = currentMap.name;
            int index = currentMapName.IndexOf("(Clone)", StringComparison.Ordinal);
            if (index >= 0)
            {
                currentMapName = currentMapName.Substring(0, index);
            }

            if (currentMapName != prefabMapData.mapPrefab.name)
            {
                Debug.LogError("Map prefab is not the same as the current map");
                return;
            }

            prefabMapData.pos = new Position(currentMap.transform.position);
            prefabMapData.rot = new Rotation(currentMap.transform.eulerAngles);
            prefabMapData.scale = new Scale(currentMap.transform.lossyScale);
        }

        private void ValidateMapData()
        {
            if (isOverrideMapPrefab)
            {
                isOverrideMapData = false;
                isOverrideMapPreset = false;
                return;
            }

            if (isOverrideMapData)
            {
                isOverrideMapPreset = false;
                isOverrideMapPrefab = false;
                return;
            }

            if (isOverrideMapPreset)
            {
                isOverrideMapData = false;
                isOverrideMapPrefab = false;
                return;
            }
        }

        [FoldoutGroup(LevelEditorConstants.BOMB)]
        public LevelBoomData levelBombData;

        [FoldoutGroup(LevelEditorConstants.BOMB)]
        public bool showBombButton;

        [FoldoutGroup(LevelEditorConstants.BOMB)]
        [Button] [ShowIf("@showBombButton")]
        public void SetBombsForLevels(int minLevel, int maxRange, int bombNumber = 1, float minScale = 1,
            float maxScale = 1.5f,
            bool isSkipLevelWithBomb = true, bool isKeepSeed = false)
        {
            for (int i = minLevel; i <= maxRange; i++)
            {
                var levelData = levelConfig.GetLevelDataToys(i);
                if (levelData == null)
                    continue;

                if (levelData.name.ToLower().StartsWith("coins"))
                    continue;

                if (isSkipLevelWithBomb & levelData.levelBoomData.number != 0)
                    continue;

                var seedScale = isKeepSeed ? levelData.levelBoomData.seedScale : Random.Range(0, 100);
                var seedPosition = isKeepSeed ? levelData.levelBoomData.seedPosition : Random.Range(0, 100);
                levelData.levelBoomData = new LevelBoomData(bombNumber, minScale, maxScale, seedScale, seedPosition);
                EditorUtility.SetDirty(levelData);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [FoldoutGroup(LevelEditorConstants.BOMB)]
        public bool isOverrideBombData;

        [FoldoutGroup(LevelEditorConstants.BOMB)]
        [ShowIf("@isOverrideBombData")]
        public List<BombItem> m_AllBombs = new List<BombItem>();

        [Header("Config")]
        [FoldoutGroup(LevelEditorConstants.REFERENCES_AND_CONFIG)]
        public LevelConfig levelConfig;
        [FoldoutGroup(LevelEditorConstants.REFERENCES_AND_CONFIG)]
        public ThemeConfig themeConfig;
        [FoldoutGroup(LevelEditorConstants.REFERENCES_AND_CONFIG)]
        public PrefabConfig prefabConfig;
        [FoldoutGroup(LevelEditorConstants.REFERENCES_AND_CONFIG)]
        public GameplayItemPrefabConfig itemConfig;


        [Header("Level holder")]
        [FoldoutGroup(LevelEditorConstants.REFERENCES_AND_CONFIG)]
        public ItemSpawnUtilities itemSpawnUtilities;
        [FoldoutGroup(LevelEditorConstants.REFERENCES_AND_CONFIG)]
        public Transform itemHolder;
        [FoldoutGroup(LevelEditorConstants.REFERENCES_AND_CONFIG)]
        public Transform groundHolder;
        [FoldoutGroup(LevelEditorConstants.REFERENCES_AND_CONFIG)]
        //public GroundExtend prefabGroundExtend;
        public GroundExtend groundExtendObj;
        [FoldoutGroup(LevelEditorConstants.REFERENCES_AND_CONFIG)]
        public MapEditor mapEditor;

        private List<BaseItem> m_AllItems = new List<BaseItem>();
        private LevelData m_CurrentLevelData;

        public static readonly string[] LEVEL_EDITOR_SCENES =
        {
            "Level Editor",
        };

        private const string PATH_LEVEL_CREATIVE = "_IECProject/Creative Levels/Level Config";
        private const string PATH_LEVEL_CREATIVE_ITEM_DATA = "_IECProject/Creative Levels/Item Config";
        private const string PATH_LEVEL_CONFIG = "Assets/_IEC/Features/Gameplay/Config/Levels";
        private const string PATH_LEVEL_FS = "Assets/_IECProject/FSLevels";
        private const string PATH_MAP = "Assets/GeneratedLevels/NewlyCreated/";

        [ResponsiveButtonGroup] [Button]
        public void LoadLevelFromProject()
        {
            // currentLevelData = isLevelCreative ? levelConfig.GetLevelDataCreative(level) : levelConfig.GetLevelDataToys(level);
            if (levelDataSo != null)
            {
                currentLevelData = levelDataSo;
            }
            else
            {
                currentLevelData = levelConfig.GetLevelDataToysSO(level);
            }

            if (currentLevelData == null)
            {
                Debug.LogError("Level Data is null");
                return;
            }

            Reset();
            mapData = currentLevelData.mapData;
            //currentMapData = currentLevelData.mapData != null ? currentLevelData.mapData.name : "";
            levelObjectives = currentLevelData.listObjectives.ConvertAll(x => x.Clone());
            theme = currentLevelData.theme;
            levelTime = currentLevelData.time;
            levelBombData = currentLevelData.levelBoomData;
            isOverrideBombData = currentLevelData.levelBoomData.isOverrideBombData;
            List<_IEC.Features.Gameplay.ItemData> testItm = new List<_IEC.Features.Gameplay.ItemData>();
            foreach (var im in currentLevelData.levelSpawnData.listItemSpawns)
            {
                foreach (var imm in im.listSpawnDatas)
                {
                    testItm.Add(imm.ToItemData(im.id));
                }

            }
            m_AllItems = LevelHelper.SpawnAllItem( testItm, itemHolder, levelConfig, itemConfig, 0);
            groundExtendObj.ResizeLevelGround(new EventLevelLoad(m_AllItems, theme));
            LevelHelper.SpawnMap(currentLevelData, groundHolder, themeConfig);

            m_AllBombs = LevelHelper.SpawnAllBomb(currentLevelData, m_AllItems, itemHolder, prefabConfig);
            m_CurrentLevelData = LevelHelper.CreateLevelData(currentLevelData, level);
            //LoadMapEditorFromMapData();
        }

        [ResponsiveButtonGroup] [Button]
        void ReloadLevel()
        {
            if (levelDataSo != null)
            {
                currentLevelData = levelDataSo;
            }
            else
            {
                currentLevelData = levelConfig.GetLevelDataToysSO(level);
            }
            var levelData = ScriptableObject.CreateInstance<LevelDataSO>();
            if (currentLevelData == null)
            {
                Debug.LogError("Level Data is null");
                return;
            }

            Reset();
            currentLevelData.CloneTo(levelData);
            //currentMapData = currentLevelData.mapData != null ? currentLevelData.mapData.name : "";
            levelTime = currentLevelData.time;
            if (isOverrideMapData)
                levelData.mapData = mapData;
            levelData.listObjectives = levelObjectives.ConvertAll(x => x.Clone());
            levelData.theme = theme;
            if (isOverrideBombData)
                levelData.levelBoomData = new LevelBoomData(isOverrideBombData, GetListBombDatas());
            else
                levelData.levelBoomData = new LevelBoomData(levelBombData);
            List<_IEC.Features.Gameplay.ItemData> testItm = new List<_IEC.Features.Gameplay.ItemData>();
            foreach (var im in currentLevelData.levelSpawnData.listItemSpawns)
            {
                foreach (var imm in im.listSpawnDatas)
                {
                    testItm.Add(imm.ToItemData(im.id));
                }
            }
            m_AllItems = LevelHelper.SpawnAllItem( testItm, itemHolder, levelConfig, itemConfig, 0);
            //groundExtend groundExtend = Instantiate(prefabGroundExtend, groundHolder);
            //groundExtend.ResizeLevelGroundDirectly(new EventLevelLoad(m_AllItems, theme));
            LevelHelper.SpawnMap(currentLevelData, groundHolder, themeConfig);

            // this.RaiseEvent(EventList.GamePlay.ON_LEVEL_LOADED, new EventLevelLoad(m_AllItems, theme));
            if (isOverrideMapPrefab)
            {
                LevelHelper.SpawnMap(prefabMapData, groundHolder);
            }
            else if (isOverrideMapData)
            {
                LevelHelper.SpawnMap(levelData.GetMapData(mapData), groundHolder, themeConfig);
            }

            m_AllBombs = LevelHelper.SpawnAllBomb(levelData, m_AllItems, itemHolder, prefabConfig);
            m_CurrentLevelData = LevelHelper.CreateLevelData(levelData, level);
        }

        [Button]
        public void LoadItemsFromText(TextAsset itemText)
        {
            if (itemText == null)
            {
                EditorUtility.DisplayDialog("Error", "Item text is null", "OK");
                return;
            }

            foreach (var item in m_AllItems)
            {
                DestroyImmediate(item.gameObject);
            }

            var itemData = LevelDataAdapter.GetAllItemData(itemText.text);
            m_AllItems = LevelHelper.SpawnAllItem(itemData, itemHolder, levelConfig, itemConfig, 0);
        }

        [ResponsiveButtonGroup] [Button]
        [ShowIf("@itemHolder.transform.childCount > 0" /*&& !isLevelCreative"*/)]
        void SaveOverrideLevel()
        {
            if (currentLevelData == null)
            {
                Debug.LogError("There is no level currently selected");
                return;
            }

            if (EditorUtility.DisplayDialog("Save Override Level", "Override this level?", "Yes", "No"))
            {
                ValidatePosition();
                ValidateData();
                if (currentLevelData.difficulty != "breaktime" && !ValidateObjectives()) return;
                var baseItems = itemHolder.GetComponentsInChildren<BaseItem>();
                var listItemSpawns = new List<ItemSpawn>();
                foreach (var baseItem in baseItems)
                {
                    //var baseItem = item.GetComponent<BaseItem>();
                    if (baseItem == null || baseItem.ItemDefinition == null)
                    {
                        continue;
                    }

                    var id = baseItem.GetItemId();
                    // var spawnData = listItemSpawns.Find(x => x.id == id);
                    var spawnData = new ItemSpawn
                    {
                        id = id,
                        listSpawnDatas = new List<ItemSpawnData>()
                    };
                    listItemSpawns.Add(spawnData);
                    // var instantiatedData = root.InstantiateData.Find(x => x.itemId == id);
                    // if (instantiatedData == null)
                    // {
                    //     instantiatedData = new InstantiateData
                    //     {
                    //         itemId = id,
                    //         itemData = new List<_IEC.Features.ExportLevel.OriginalLevel.ItemData>()
                    //     };
                    //
                    //     root.InstantiateData.Add(instantiatedData);
                    // }
                    
                    var itemSpawnData = new ItemSpawnData
                    {
                        groupID =  baseItem.ItemData.groupID,
                        groupPosition =  baseItem.transform.parent.transform.position,
                        p = new Position(baseItem.transform.position),
                        r = new Rotation(baseItem.transform.eulerAngles),
                        s = new Scale(baseItem.transform.lossyScale),
                        kinematic = baseItem.ItemData.isStartKinematic
                    };
                    
                    spawnData.listSpawnDatas.Add(itemSpawnData);
                }

                if (isOverrideBombData && m_AllBombs.Count > 0)
                    currentLevelData.levelBoomData = new LevelBoomData(isOverrideBombData, GetListBombDatas());
                else
                    currentLevelData.levelBoomData = new LevelBoomData(levelBombData);

                currentLevelData.theme = theme;
                currentLevelData.listObjectives = levelObjectives.ConvertAll(x => x.Clone());
                currentLevelData.time = levelTime;

                if (mapData != null && isOverrideMapData)
                {
                    currentLevelData.isOverriddenByPreset = false;
                    currentLevelData.mapData = mapData;
                }

                SaveLevelData(listItemSpawns);
            }
        }

        private void SaveLevelData(List<ItemSpawn> listItemSpawnDatas)
        {
            if (currentLevelData.levelSpawnData == null)
                currentLevelData.levelSpawnData = ScriptableObject.CreateInstance<LevelSpawnData>();
            
            currentLevelData.levelSpawnData.listItemSpawns = listItemSpawnDatas.ToList();

            EditorUtility.SetDirty(currentLevelData);
            EditorUtility.SetDirty(currentLevelData.levelSpawnData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // [ResponsiveButtonGroup] [Button]
        // [ShowIf("@itemHolder.transform.childCount > 0 && !isLevelCreative")]
        // void SaveLevel()
        // {
        //     if (currentLevelData == null)
        //     {
        //         Debug.LogError("There is no level currently selected");
        //         return;
        //     }
        //
        //     if (EditorUtility.DisplayDialog("Save Level", "Choose the location to save this level's data", "Yes", "No"))
        //     {
        //         OnSaveLevel();
        //     }
        // }

        [ResponsiveButtonGroup] [Button]
        private void Reset()
        {
            // currentLevelData = null;
            // currentMapData = null;
            // levelDataSo = null;
            m_AllItems.Clear();
            m_AllBombs.Clear();

            while (itemHolder.transform.childCount > 0)
            {
                DestroyImmediate(itemHolder.transform.GetChild(0).gameObject);
            }

            while (groundHolder.transform.childCount > 0)
            {
                DestroyImmediate(groundHolder.transform.GetChild(0).gameObject);
            }
        }

        [ResponsiveButtonGroup]
        [Button]
public void LogInfoLevel()
{
    string dialog = "DataInfoLevel\n";
    dialog += "------------------------------------------------------------\n";
    dialog += string.Format("{0,-40} {1,-10} {2,-10} {3,-10} {4,-20}", "Objective", "Amount", "Exp", "Total Exp", "Size") + "\n";
    dialog += "------------------------------------------------------------\n";
    
    int countExp = 0;
    if (currentLevelData == null)
    {
        Debug.LogError("Not found Data Level");
        return;
    }

    Dictionary<string, int> objectiveId = new Dictionary<string, int>();
    foreach (var levelObjectiveData in currentLevelData.listObjectives)
    {
        dialog += string.Format("{0,-40} {1,-10} {2,-10} {3,-10} {4,-20}", 
            "Objective: " + levelObjectiveData.id, 
            levelObjectiveData.amount.ToString(),  // Làm tròn số Amount
            itemConfig.GetItemExp(levelObjectiveData.id).ToString(),  // Làm tròn số Exp
            (levelObjectiveData.amount * itemConfig.GetItemExp(levelObjectiveData.id)).ToString("F2"),  // Tổng Exp
            " ") + "\n";
        
        objectiveId.Add(levelObjectiveData.id, levelObjectiveData.amount);
    }

    foreach (var listItemSpawn in currentLevelData.levelSpawnData.listItemSpawns)
    {
        if (objectiveId.ContainsKey(listItemSpawn.id))
        {
            objectiveId.TryGetValue(listItemSpawn.id, out int targetAm);
            if (listItemSpawn.listSpawnDatas.Count > targetAm)
            {
                dialog += string.Format("{0,-40} {1,-10} {2,-10} {3,-10} {4,-20}", 
                    "Objective Extra: " + listItemSpawn.id, 
                    (listItemSpawn.listSpawnDatas.Count - targetAm).ToString(),  // Số mục thừa
                    itemConfig.GetItemExp(listItemSpawn.id).ToString(),  // Làm tròn Exp
                    (itemConfig.GetItemExp(listItemSpawn.id) * (listItemSpawn.listSpawnDatas.Count - targetAm)).ToString("F2"),
                    GetStringSizeObject(itemConfig.GetItemPrefab(listItemSpawn.id)) + "\n");
            }
            else if (listItemSpawn.listSpawnDatas.Count < targetAm)
            {
                dialog += string.Format("{0,-40} {1,-10} {2,-10} {3,-10} {4,-20}", 
                    "Objective Missing: " + listItemSpawn.id, 
                    (targetAm - listItemSpawn.listSpawnDatas.Count).ToString(),  // Số mục thiếu
                    itemConfig.GetItemExp(listItemSpawn.id).ToString(),  // Làm tròn Exp
                    (itemConfig.GetItemExp(listItemSpawn.id) * (targetAm - listItemSpawn.listSpawnDatas.Count)).ToString("F2"),
                    GetStringSizeObject(itemConfig.GetItemPrefab(listItemSpawn.id)) + "\n");
            }

            // Skip objective
            countExp += itemConfig.GetItemExp(listItemSpawn.id) * listItemSpawn.listSpawnDatas.Count;
            continue;
        }

        if (listItemSpawn.id.Contains("food_"))
        {
            dialog += string.Format("{0,-40} {1,-10} {2,-10} {3,-10} {4,-20}", 
                "Normal: " + listItemSpawn.id, 
                listItemSpawn.listSpawnDatas.Count.ToString(), 
                itemConfig.GetItemExp(listItemSpawn.id).ToString(), 
                (listItemSpawn.listSpawnDatas.Count * itemConfig.GetItemExp(listItemSpawn.id)).ToString("F2"),
                GetStringSizeObject(itemConfig.GetItemPrefab(listItemSpawn.id)) + "\n");
            countExp += itemConfig.GetItemExp(listItemSpawn.id) * listItemSpawn.listSpawnDatas.Count;
        }
        else
        {
            dialog += string.Format("{0,-40} {1,-10} {2,-10} {3,-10} {4,-20}", 
                "Special: " + listItemSpawn.id, 
                listItemSpawn.listSpawnDatas.Count.ToString(),
                itemConfig.GetItemExp(listItemSpawn.id).ToString(), 
                (listItemSpawn.listSpawnDatas.Count * itemConfig.GetItemExp(listItemSpawn.id)).ToString("F2"),
                GetStringSizeObject(itemConfig.GetItemPrefab(listItemSpawn.id)) + "\n");
        }
    }

    dialog += "------------------------------------------------------------\n";
    dialog += "Total Exp: " + countExp.ToString("F2") + "\n";  // Làm tròn tổng Exp
    dialog += "------------------------------------------------------------\n";
    dialog += string.Format("{0,-40} {1,-10} {2,-10} {3,-10}", 
        "Bomb: " , 
        currentLevelData.levelBoomData.number, 
        0, 
        0) + "\n";

    // Hiển thị cửa sổ custom dialog với nội dung đã được định dạng
    CustomDialogWindow.ShowWindow(dialog);
}


        [Button]
        private void DecreaseLevelGoalsRandomly(int minLevel, int maxLevel, int minDecrease, int maxDecrease,
            int multiplier = 1,
            float decreaseRate = 0.5f)
        {
            for (int i = minLevel; i <= maxLevel; i++)
            {
                var levelData = levelConfig.GetLevelDataToysSO(i);
                if (levelData == null)
                {
                    Debug.LogError($"Level {i} is null");
                    continue;
                }

                foreach (var levelObjective in levelData.listObjectives)
                {
                    var isDecrease = UnityEngine.Random.value < decreaseRate;
                    if (!isDecrease) continue;

                    var random = UnityEngine.Random.Range(minDecrease, maxDecrease) * multiplier;
                    if (levelObjective.amount > random)
                    {
                        levelObjective.amount -= random;
                    }
                }

                EditorUtility.SetDirty(levelData);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public string GetStringSizeObject(GameObject obj,float scx =1f,float scy=1f,float scz=1f )
        {
            //Apply with localscale
            var itemHeight = 1f;
            var renderer  = obj.GetComponentInChildren<MeshRenderer>();;
            if (renderer != null)
            {
                itemHeight = renderer.bounds.size.y;
                return "x:"+(renderer.bounds.size.x*scx).ToString("F2") + "  ,y:" + (renderer.bounds.size.y*scy).ToString("F2") +"  ,z:" + (renderer.bounds.size.z*scz).ToString("F2");
            }

            return " ";
        }
        private static IEnumerable GetAllMapData()
        {
            var root = "Assets/GeneratedLevels/NewlyCreated/";

            return AssetDatabase.GetAllAssetPaths()
                .Where(x => x.StartsWith(root))
                .Select(x => x.Substring(root.Length))
                .Select(x => new ValueDropdownItem(x, UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(root + x)));
        }

        private static IEnumerable GetAllPreset()
        {
            return AssetDatabase.FindAssets("t:GroundShapePresetParameters")
                .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
                .Select(x =>
                    new ValueDropdownItem(x,
                        UnityEditor.AssetDatabase.LoadAssetAtPath<GroundShapePresetParameters>(x)));
        }

        private IEnumerable GetAllThemes()
        {
            return themeConfig.groundThemeConfigModels.Select(x => x.themeId);
        }

        private IEnumerable GetAllItemIds()
        {
            return itemConfig.listItemDefinition.Select(x => x.GetId());
        }
        
        private IEnumerable GetAllPillarItemIds()
        {
            return itemConfig.listItemDefinition
                .Where(x => x.GetPhysicFallingItemType() == _IEC.Features.Gameplay.PhysicFallingItemType.Column)
                .Select(x => x.GetId());
        }
        
        private IEnumerable GetAllCurrentItemIds()
        {
            var currentItems = itemHolder.GetComponentsInChildren<BaseItem>().ToList();
            return currentItems.Where(x => x.ItemDefinition != null).Select(x => x.GetItemId());
        }
        
        private IEnumerable GetAllItemIdsWithSameXP()
        {
            var oldItem = itemConfig.listItemDefinition.Find(item => item.GetId() == oldItemID);
            return itemConfig.listItemDefinition.Where(x => x.GetXp() == oldItem.GetXp()).Select(x => x.GetId());
        }

        private void UpdateItemSpawnUtilities()
        {
            itemSpawnUtilities.itemId = itemId;
        }

        private List<BombData> GetListBombDatas()
        {
            if (m_AllBombs.Count == 0)
            {
                return null;
            }

            var bombDatas = new List<BombData>();
            foreach (var bomb in m_AllBombs)
            {
                bombDatas.Add(new BombData
                {
                    pos = new Position(bomb.transform.position),
                    scale = new Scale(bomb.transform.lossyScale),
                });
            }

            return bombDatas;
        }

        // private void OnSaveLevel()
        // {
        //     ValidatePosition();
        //     ValidateData();
        //     Root root = new Root();
        //     root.InstantiateData = new List<InstantiateData>();
        //     var baseItems = itemHolder.GetComponentsInChildren<BaseItem>();
        //     foreach (var baseItem in baseItems)
        //     {
        //         //var baseItem = item.GetComponent<BaseItem>();
        //         if (baseItem == null)
        //         {
        //             continue;
        //         }
        //
        //         var id = baseItem.GetItemId();
        //         var instantiatedData = root.InstantiateData.Find(x => x.itemId == id);
        //         if (instantiatedData == null)
        //         {
        //             instantiatedData = new InstantiateData
        //             {
        //                 itemId = id,
        //                 itemData = new List<_IEC.Features.ExportLevel.OriginalLevel.ItemData>()
        //             };
        //
        //             root.InstantiateData.Add(instantiatedData);
        //         }
        //
        //         var itemData = new _IEC.Features.ExportLevel.OriginalLevel.ItemData();
        //         itemData.pos = new Position(baseItem.transform.position);
        //         itemData.rot = new Rotation(baseItem.transform.eulerAngles);
        //         itemData.scale = new Scale(baseItem.transform.lossyScale);
        //         itemData.sk = true;
        //
        //         instantiatedData.itemData.Add(itemData);
        //     }
        //
        //     if (isOverrideBombData && m_AllBombs.Count > 0)
        //     {
        //         currentLevelData.levelBoomData = new LevelBoomData(isOverrideBombData, GetListBombDatas());
        //     }
        //     else
        //         currentLevelData.levelBoomData = new LevelBoomData(levelBombData);
        //
        //     currentLevelData.theme = theme;
        //     currentLevelData.listObjectives = levelObjectives.ConvertAll(x => x.Clone());
        //
        //     if (mapData != null && isOverrideMapData)
        //     {
        //         currentLevelData.isOverriddenByPreset = false;
        //         currentLevelData.mapData = mapData;
        //     }
        //
        //     var json = JsonConvert.SerializeObject(root);
        //     TextAsset itemTextAsset = currentLevelData.itemInstantiateItemData;
        //     var itemJsonPath = EditorUtility.SaveFilePanelInProject("Save item data", "Items_" + level, "txt",
        //         "Please enter a file name to save the item data", PATH_LEVEL_FS);
        //
        //     File.WriteAllText(itemJsonPath, json);
        //     AssetDatabase.SaveAssets();
        //     AssetDatabase.Refresh();
        //
        //     var loadedItemJson = AssetDatabase.LoadAssetAtPath<TextAsset>(itemJsonPath);
        //     if (loadedItemJson != null)
        //     {
        //         currentLevelData.itemInstantiateItemData = loadedItemJson;
        //     }
        //
        //     var levelConfigPath = EditorUtility.SaveFilePanelInProject("Save map", "Level", "asset",
        //         "Please enter a file name to save the texture to", PATH_LEVEL_CONFIG);
        //
        //     if (File.Exists(levelConfigPath))
        //     {
        //         var loadedLevelData = AssetDatabase.LoadAssetAtPath<LevelConfigModel>(levelConfigPath);
        //         if (loadedLevelData != null)
        //         {
        //             currentLevelData = loadedLevelData;
        //         }
        //
        //         EditorUtility.SetDirty(loadedLevelData);
        //     }
        //     else
        //     {
        //         AssetDatabase.CreateAsset(currentLevelData, levelConfigPath);
        //         currentLevelData = AssetDatabase.LoadAssetAtPath<LevelConfigModel>(levelConfigPath);
        //     }
        //
        //     EditorUtility.SetDirty(currentLevelData);
        //     EditorUtility.SetDirty(currentLevelData.levelSpawnData);
        //     AssetDatabase.Refresh();
        //     AssetDatabase.SaveAssets();
        // }
        

        [Serializable]
        public class ItemCountInScene
        {
            [HorizontalGroup]
            [ValueDropdown(nameof(GetAllItemIds))]
            [OnValueChanged(nameof(UpdateItemCount))]
            public string itemId;
            [HorizontalGroup]
            [ReadOnly] public int count;

            [HideInInspector]
            public Transform itemHolder;
            [HideInInspector]
            public GameplayItemPrefabConfig itemConfig;

            public ItemCountInScene()
            {
                UpdateItemCount();
            }

            public ItemCountInScene(string itemId, int count)
            {
                this.itemId = itemId;
                this.count = count;

                UpdateItemCount();
            }

            private void UpdateItemCount()
            {
                if (itemHolder == null) return;
                count = itemHolder.GetComponentsInChildren<BaseItem>()
                    .Count(x => x.ItemDefinition != null && x.GetItemId() == itemId);
            }

            [Button]
            public void UpdateCount()
            {
                UpdateItemCount();
            }

            private IEnumerable GetAllItemIds()
            {
                if (itemConfig == null) return null;
                return itemConfig.listItemDefinition.Select(x => x.GetId());
            }


        }
#endif
    }
}
public class CustomDialogWindow : EditorWindow
{
    private string dialogContent = "";

    // Hàm này được gọi để hiển thị cửa sổ và truyền nội dung dialog vào
    public static void ShowWindow(string dialog)
    {
        CustomDialogWindow window = GetWindow<CustomDialogWindow>("Custom Dialog");
        window.minSize = new Vector2(400, 300);  // Quy ước kích thước tối thiểu
        window.maxSize = new Vector2(600, 500);  // Quy ước kích thước tối đa
        window.dialogContent = dialog;  // Truyền nội dung vào cửa sổ
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Dialog Content", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Hiển thị nội dung của dialog trong TextArea
        EditorGUILayout.TextArea(dialogContent, GUILayout.Height(300));

        GUILayout.Space(10);
        if (GUILayout.Button("OK"))
        {
            Close(); // Đóng cửa sổ khi nhấn OK
        }
    }
}