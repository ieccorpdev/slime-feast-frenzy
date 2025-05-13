using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IEC;
using IEC.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using _IEC.Features.ExportLevel.OriginalLevel;
using UnityEngine.Networking;

#if UNITY_EDITOR
using _IECModules;
using UnityEditor;
#endif

namespace _IEC.Features.Gameplay
{
    [Serializable]
    public class LevelFTUEAB
    {
        public int level;
        public LevelConfigModel levelConfigModel;
    }
    
    [CreateAssetMenu(menuName = "Gameplay/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public List<LevelConfigModel> levelConfigData;
        [ListDrawerSettings(ShowIndexLabels = true)]
        public List<LevelConfigModel> levelConfigDataEdited;
        [ListDrawerSettings(ShowIndexLabels = true)]
        // public List<LevelConfigModel> levelConfigDataWithToys;
        public List<LevelConfigModel> levelConfigDataSHP;
        public List<LevelConfigModel> levelConfigCreative;

        [Header("New Datas")] 
        public LevelDataSO playTestData;
        // public List<LevelDataSO> listLevelDatas;
        // public List<LevelDataSO> listLevelDatasSHP;

        public LevelDataContainerSO levelData;
        public LevelDataContainerSO levelDataSHP;
        // public List<LevelDataSO> listLevelDataToys;
        
        
        public List<LevelFTUEAB> levelFTUEAB;
#if UNITY_EDITOR
        [SerializeField] private DefaultAsset folder;
#endif
        [ReadOnly, ShowInInspector] string _folderPath;
        
        public int maxLevel;
        public int startLoopLevel;

        [Header("Editor only")] 
        public GameplayItemPrefabConfig editorItemPrefabConfig;

        [Button]
        public void UpdateItemToNormal(string id)
        {
#if UNITY_EDITOR
            foreach (var levelData in levelDataSHP.listAllLevelData)
            {
                var obj = levelData.listObjectives.FirstOrDefault(item => item.id.Equals(id));
                while (obj != null)
                {
                    levelData.listObjectives.Remove(obj);
                    obj = levelData.listObjectives.FirstOrDefault(item => item.id.Equals(id));
                    Debug.LogError(($"Remove objective: {id} at {levelData.name}"));
                    
                    EditorUtility.SetDirty(levelData);
                    AssetDatabase.SaveAssets();        
                    AssetDatabase.Refresh();
                }
            }
#endif
        }

        [Button]
        public void CheckValidAllLevel()
        {
            List<string> listDebugLevel = new();
            foreach (var level in levelDataSHP.listAllLevelData)
            {
                if(level.difficulty.Equals("breaktime"))
                    continue;
                
                string temp = "";
                foreach (var obj in level.listObjectives)
                {
                    var objItem = level.levelSpawnData.listItemSpawns.Find(item => item.id.Equals(obj.id));

                    if (objItem == null || obj.amount > objItem.listSpawnDatas.Count)
                    {
                        temp += obj.id + "_";
                    }
                }

                if (!String.IsNullOrEmpty(temp))
                {
                    temp = level.name + ": " + temp;
                    listDebugLevel.Add(temp);
                    //Debug.LogError(temp);
                }
            }

            if (listDebugLevel.Count > 0)
            {
                foreach (var debug in listDebugLevel)
                {
                    Debug.LogError(debug);
                }
            }
            else
            {
                Debug.LogError("All levels are valid");
            }
        }
        
        [Button]
        public void ExportAllLevelsToCSV(LevelDataContainerSO allLevels)
        {
            string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            string path = Path.Combine(desktopPath, $"{allLevels.name}.csv");
            StringBuilder csv = new StringBuilder();
            
            csv.AppendLine("Level,Objective,Amount,Exp,Time");

            for (int levelIndex = 0; levelIndex < allLevels.listAllLevelData.Count; levelIndex++)
            {
                LevelDataSO levelData = allLevels.listAllLevelData[levelIndex];

                if (levelData == null)
                {
                    Debug.LogWarning($"Level {levelIndex} is null, skipping.");
                    continue;
                }

                // Add spacing between levels
                // if (levelIndex > 0)
                // {
                //     csv.AppendLine(); // Empty line
                // }
                //
                // csv.AppendLine("Level,Objective,Amount,Exp,Time");

                Dictionary<string, int> objectiveId = new Dictionary<string, int>();

                foreach (var levelObjectiveData in levelData.listObjectives)
                {
                    var id = levelObjectiveData.id;
                    var amount = levelObjectiveData.amount;
                    var exp = editorItemPrefabConfig.GetItemExp(id);
                    int time = levelData.time;

                    csv.AppendLine($"{levelIndex},Objective: {id},{amount},{exp},{time}");
                    objectiveId.Add(id, amount);
                }

                foreach (var listItemSpawn in levelData.levelSpawnData.listItemSpawns)
                {
                    var id = listItemSpawn.id;
                    var count = listItemSpawn.listSpawnDatas.Count;
                    var exp = editorItemPrefabConfig.GetItemExp(id);
                    int time = levelData.time;

                    if (objectiveId.TryGetValue(id, out int targetAm))
                    {
                        if (count > targetAm)
                        {
                            csv.AppendLine($"{levelIndex},Objective Extra: {id},{count - targetAm},{exp},{time}");
                        }
                        else if (count < targetAm)
                        {
                            csv.AppendLine($"{levelIndex},Objective Missing: {id},{targetAm - count},{exp},{time}");
                        }

                        continue;
                    }

                    string type = id.Contains("food_") ? "Normal" : "Special";
                    csv.AppendLine($"{levelIndex},{type}: {id},{count},{exp},{time}");
                }

                // Optional: include bomb count per level
                csv.AppendLine($"{levelIndex},Bomb,{levelData.levelBoomData.number},0,{levelData.time}");
            }

            File.WriteAllText(path, csv.ToString());
            Debug.Log("All levels exported to CSV at: " + path);
        }
        
        private GameConfig Config => Pool.Get<GameConfig>();

        private readonly List<int> excludedLevels = new List<int>
        {
            125, 189, 235,
            127, 141, 146, 207, 210, 221, 237, 245, 260, 295, 361, 363, 370, 376, 383, 385, 389, 391, 411, 413, 434, 451, 460, 468, 471, 473, 475, 481, 485, 491, 495, 508, 526, 529, 544, 553, 562, 570, 581,
        };

#if  UNITY_EDITOR
        void OnValidate()
        {
            if (folder != null)
            {
                _folderPath = AssetDatabase.GetAssetPath(folder);
                if (!AssetDatabase.IsValidFolder(_folderPath))
                {
                    Debug.LogError("The assigned asset is not a folder!");
                    folder = null;
                    _folderPath = null;
                }
                else
                {
                    Debug.Log($"Folder path: {_folderPath}");
                   
                }
            }
        }
#endif


#if UNITY_EDITOR
        [Button]
        void RemakeLevelSpawnDats()
        {
            // foreach (var levelDataSO in listLevelDatas)
            // {
            //     levelDataSO.RemakeLevelSpawnData();
            // }
            
            foreach (var levelDataSO in levelDataSHP.listAllLevelData)
            {
                levelDataSO.RemakeLevelSpawnData();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [Button]
        void ReplaceAllIdsForFS(GameplayItemPrefabConfig itemConfig)
        {
            var itemMapping = new Dictionary<string, string>();
            for (int i = 0; i < itemConfig.listItemDefinition.Count; i++)
            {
                var itemDefinition = itemConfig.listItemDefinition[i];
                itemMapping.TryAdd(itemDefinition.GetId(), $"food_{i + 1}");
            }
            
            foreach (var levelConfigModel in levelConfigDataEdited)
            {
                Root root = JsonConvert.DeserializeObject<Root>(levelConfigModel.itemInstantiateItemData.text);
                if (root == null)
                {
                    Debug.LogError("Cannot deserialize root" + levelConfigModel.name);
                    continue;
                }
                
                foreach (var levelObjective in levelConfigModel.levelObjectives)
                {
                    if (!itemMapping.ContainsKey(levelObjective.itemId)) continue;
                    levelObjective.itemId = itemMapping[levelObjective.itemId];
                }
                
                foreach (var instantiateData in root.InstantiateData)
                {
                    if (!itemMapping.ContainsKey(instantiateData.itemId)) continue;
                    instantiateData.itemId = itemMapping[instantiateData.itemId];
                }
                
                var convertedText = JsonConvert.SerializeObject(root);
                
                EditorUtility.SetDirty(levelConfigModel);
                var path = AssetDatabase.GetAssetPath(levelConfigModel.itemInstantiateItemData);
                File.WriteAllText(path, convertedText);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [Button]
        void ReplaceAllIdsForSHP(GameplayItemPrefabConfig itemConfig)
        {
            var itemMapping = new Dictionary<string, string>();
            for (int i = 0; i < itemConfig.listItemDefinitionShp.Count; i++)
            {
                var itemDefinition = itemConfig.listItemDefinitionShp[i];
                itemMapping.TryAdd(itemDefinition.GetId(), $"food_{i + 1}");
            }
            
            foreach (var levelConfigModel in levelConfigDataSHP)
            {
                if (levelConfigModel == null) continue;
                if (levelConfigModel.itemInstantiateItemData == null) continue;
                Root root = JsonConvert.DeserializeObject<Root>(levelConfigModel.itemInstantiateItemData.text);
                if (root == null)
                {
                    Debug.LogError("Cannot deserialize root" + levelConfigModel.name);
                    continue;
                }
                
                foreach (var levelObjective in levelConfigModel.levelObjectives)
                {
                    if (!itemMapping.ContainsKey(levelObjective.itemId)) continue;
                    levelObjective.itemId = itemMapping[levelObjective.itemId];
                }
                
                foreach (var instantiateData in root.InstantiateData)
                {
                    if (!itemMapping.ContainsKey(instantiateData.itemId)) continue;
                    instantiateData.itemId = itemMapping[instantiateData.itemId];
                }
                
                var convertedText = JsonConvert.SerializeObject(root);
                
                EditorUtility.SetDirty(levelConfigModel);
                var path = AssetDatabase.GetAssetPath(levelConfigModel.itemInstantiateItemData);
                File.WriteAllText(path, convertedText);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [Button]
        void ReplaceAllIdsOfLevel(GameplayItemPrefabConfig itemConfig, LevelConfigModel levelConfigModel)
        {
            var itemMapping = new Dictionary<string, string>();
            for (int i = 0; i < itemConfig.listItemDefinition.Count; i++)
            {
                var itemDefinition = itemConfig.listItemDefinition[i];
                itemMapping.TryAdd(itemDefinition.GetId(), $"food_{i + 1}");
            }
            
            Root root = JsonConvert.DeserializeObject<Root>(levelConfigModel.itemInstantiateItemData.text);
            if (root == null)
            {
                Debug.LogError("Cannot deserialize root" + levelConfigModel.name);
                return;
            }
            
            foreach (var instantiateData in root.InstantiateData)
            {
                instantiateData.itemId = itemMapping[instantiateData.itemId];
            }
            
            var convertedText = JsonConvert.SerializeObject(root);
            
            EditorUtility.SetDirty(levelConfigModel);
            var path = AssetDatabase.GetAssetPath(levelConfigModel.itemInstantiateItemData);
            File.WriteAllText(path, convertedText);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Button]
        void ReplaceAllIdsForObjectives(TextAsset itemIdsCSV)
        {
            var itemMapping = new Dictionary<string, string>();
            var result = GoogleSheetToCSV.ParseCSV(itemIdsCSV.text, true);
            foreach (var line in result)
            {
                if (line.Count < 2) break;
                var id = line[0];
                var newId = line[1];
                itemMapping.TryAdd(id, newId);
            }
            
            foreach (var levelConfig in levelConfigDataEdited)
            {
                foreach (var levelObjective in levelConfig.levelObjectives)
                {
                    levelObjective.itemId = itemMapping[levelObjective.itemId];
                }
                EditorUtility.SetDirty(levelConfig);
            }
            
            foreach (var levelDataSO in levelDataSHP.listAllLevelData)
            {
                foreach (var levelObjective in levelDataSO.listObjectives)
                {
                    levelObjective.id = itemMapping[levelObjective.id];
                }
                EditorUtility.SetDirty(levelDataSO);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [Button]
        void CreateLevelSpawnDatas()
        {
            for (int i = 0; i < levelConfigDataEdited.Count; i++)
            {
                var levelConfig = levelConfigDataEdited[i];
                levelConfig.ConvertToLevelSpawnData(i + 1, "FS");
            }
            
            for (int i = 0; i < levelConfigDataSHP.Count; i++)
            {
                var levelConfig = levelConfigDataSHP[i];
                if (levelConfig == null) continue;
                levelConfig.ConvertToLevelSpawnData(i + 1, "SHP");
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [Button]
        void CreateLevelDataSO()
        {
            levelData.listAllLevelData = new List<LevelDataSO>();
            levelDataSHP.listAllLevelData = new List<LevelDataSO>();
            
            for (int i = 0; i < levelConfigDataEdited.Count; i++)
            {
                var levelConfig = levelConfigDataEdited[i];
                var levelDataSo = levelConfig.CreateLevelDataSO(i + 1, "FS");
                levelData.listAllLevelData.Add(levelDataSo);
            }
            
            for (int i = 0; i < levelConfigDataSHP.Count; i++)
            {
                var levelConfig = levelConfigDataSHP[i];
                if (levelConfig == null) continue;
                var levelDataSo = levelConfig.CreateLevelDataSO(i + 1, "SHP");
                levelDataSHP.listAllLevelData.Add(levelDataSo);
            }
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Button]
        void RemapAllLevelCoin()
        {
            List<string> idCoin = new List<string>() { "food_224", "food_225", "food_226", "food_227", "food_228" };
            foreach (var level in levelData.listAllLevelData)
            {
                var isCoin = level.difficulty == "breaktime";
                if(!isCoin) continue;

                foreach (var objective in level.listObjectives)
                {
                    objective.id = "coin";
                }
                
                EditorUtility.SetDirty(level);
            }
            
            AssetDatabase.SaveAssets();
        }

        [Button]
        void ReplaceCoinAnimal()
        {
            foreach (var level in levelData.listAllLevelData)
            {
                var isCoin = level.difficulty == "breaktime";
                if(!isCoin) continue;

                foreach (var itemSpawn in level.levelSpawnData.listItemSpawns)
                {
                    if (itemSpawn.id == "food_183")
                    {
                        itemSpawn.id = "coin_cat";
                    }
                    if (itemSpawn.id == "food_184")
                    {
                        itemSpawn.id = "coin_pig";
                    }
                    if (itemSpawn.id == "food_185")
                    {
                        itemSpawn.id = "coin_chinese";
                    }
                    if (itemSpawn.id == "food_186")
                    {
                        itemSpawn.id = "coin_big";
                    }
                    if (itemSpawn.id == "food_187")
                    {
                        itemSpawn.id = "coin_small";
                    }
                }
                
                EditorUtility.SetDirty(level.levelSpawnData);
            }
            
            AssetDatabase.SaveAssets();
        }
        
        [Button]
        void SetDirtyAndSaveAllLevels()
        {
            foreach (var levelConfigModel in levelConfigDataEdited)
            {
                EditorUtility.SetDirty(levelConfigModel);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Button]
        void ShuffleObjectives()
        {
            foreach (var levelDataSo in levelData.listAllLevelData)
            {
                levelDataSo.listObjectives.Shuffle();
                EditorUtility.SetDirty(levelDataSo);
            }
            
            foreach (var levelDataSo in levelDataSHP.listAllLevelData)
            {
                levelDataSo.listObjectives.Shuffle();
                EditorUtility.SetDirty(levelDataSo);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [Button]
        void AddLevelFTUEAB()
        {
            if (!AssetDatabase.IsValidFolder(_folderPath))
            {
                return;
            }

            levelFTUEAB = new List<LevelFTUEAB>();
            var allLevelConfigModels = Utilities.FindAllAssetsInFolder<LevelConfigModel>(_folderPath);
            foreach (var levelModel in allLevelConfigModels)
            {
                levelFTUEAB.Add(new LevelFTUEAB()
                {
                    level = int.Parse(levelModel.name.Split(' ')[1]),
                    levelConfigModel = levelModel
                });
            }
        }
#endif
        
        // public GameObject GetItemPrefab(string itemId)
        // {
        //     var itemPrefabConfig = Config.itemPrefabConfig.listItemDefinition.Find(x => x.GetId() == itemId);
        //     if (itemPrefabConfig == null) return null;
        //     return itemPrefabConfig.GetPrefab();
        // }

        public ItemDefinition GetItemDefinition(string itemId, int version)
        {
            var itemPrefabConfig = Config.itemPrefabConfig.GetListItemDefinition().Find(x => x.GetId() == itemId);
            return itemPrefabConfig;
        }

        public ItemDefinition GetItemDefinition(string itemId, GameplayItemPrefabConfig itemPrefabConfig, int version)
        {
            var itemDefinition = itemPrefabConfig.GetListItemDefinition().Find(x => x.GetId() == itemId);
            return itemDefinition;
        }
        
        public GameObject GetMapTilePrefab(string tileId, string skinID)
        {
            return null;
        }

        public LevelConfigModel GetLevelData(int level)
        {
            level = GetLevel(level);
            var levelIndex = level - 1;
            return levelConfigDataEdited[levelIndex];
        }

        public LevelConfigModel GetLevelDataCreative(int level)
        {
            level = Mathf.Clamp(level,1, levelConfigCreative.Count);
            var levelIndex = level - 1;
            return levelConfigCreative[levelIndex];
        }

        public LevelConfigModel GetLevelDataSHP(int level)
        {
            level = GetLevel(level);
            var levelIndex = level - 1;
            return levelConfigDataSHP[levelIndex];
        }
        
        public LevelConfigModel GetLevelDataToys(int level)
        {
            level = GetLevel(level);
            var levelIndex = level - 1;
            return levelConfigDataEdited[levelIndex];
        }
        
        public LevelConfigModel GetFTUELevelData(int level)
        {
            var levelIndex = level - 1;
            
            var levelFTUE = levelFTUEAB.Find(x => x.level == level);
            if (levelFTUE == null)
            {
                return levelConfigDataEdited[levelIndex];
            }
            
            return levelFTUE.levelConfigModel;
        }
        
        public LevelDataSO GetLevelDataSO(int level)
        {
            level = GetLevel(level);
            var levelIndex = level - 1;
            return levelData.listAllLevelData[levelIndex];
        }
        
        public LevelDataSO GetLevelDataSOSHP(int level)
        {
            level = GetLevel(level);
            var levelIndex = level - 1;
            return levelDataSHP.listAllLevelData[levelIndex];
        }
        
        public LevelDataSO GetLevelDataToysSO(int level)
        {
            level = GetLevel(level);
            var levelIndex = level - 1;
            return levelData.listAllLevelData[levelIndex];
        }

        public LevelDataSO GetLevelDataCreativeSO(int level)
        {
            level = Mathf.Clamp(level,1, levelData.listAllLevelData.Count);
            var levelIndex = level - 1;
            return levelData.listAllLevelData[levelIndex];
        }

        private int GetLevel(int level)
        {
            var max = levelData.listAllLevelData.Count;
            if (level <= max)
            {
                return level; // Nếu level nằm trong khoảng từ 1 đến maxLevel, trả về level hiện tại
            }
            else
            {
                // Nếu level vượt quá maxLevel, bắt đầu lại từ startLoopLevel
                //return 0;
                return startLoopLevel + ((level - max - 1) % (max - startLoopLevel + 1));
            }
        }
        
        
        
#if UNITY_EDITOR

        [Button]
        void RemapThemeId()
        {
            var breakfastMap = new List<string> { "FastFood" };
            var asianMap = new List<string> {"Cakes" , "ColdBeverage", "SchoolSupplies", "TastyPastry", "Toys"};
            foreach (var levelConfigModel in levelConfigDataEdited)
            {
                if (breakfastMap.Contains(levelConfigModel.groundSkin))
                {
                    levelConfigModel.groundSkin = "Breakfast";
                }
                else if (asianMap.Contains(levelConfigModel.groundSkin))
                {
                    levelConfigModel.groundSkin = "Asian";
                }

                EditorUtility.SetDirty(levelConfigModel);
            }
            
            foreach (var levelConfigModel in levelConfigDataSHP)
            {
                if (breakfastMap.Contains(levelConfigModel.groundSkin))
                {
                    levelConfigModel.groundSkin = "Breakfast";
                }
                else if (asianMap.Contains(levelConfigModel.groundSkin))
                {
                    levelConfigModel.groundSkin = "Asian";
                }

                EditorUtility.SetDirty(levelConfigModel);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [Button]
        void LogLevelWithArch()
        {
            List<string> listLevelConfig = new List<string>();
            List<int> listLevel = new List<int>();
            for (int i = 0; i < levelConfigDataEdited.Count; i++)
            {
                var currentLevelConfig = levelConfigDataEdited[i];
                var listItemData = currentLevelConfig.GetItemDataForInstantiate();

                bool isLevelToRemove = false;
                foreach (var item in listItemData)
                {
                    var itemDefinition = editorItemPrefabConfig.listItemDefinition.Find(x => x.GetId() == item.itemId);
                    if (itemDefinition == null)
                    {
                        Debug.LogError("Cant find item definition with id: " + item.itemId);
                        continue;
                    }

                    if (itemDefinition.GetId() == "Special_Arch")
                    {
                        isLevelToRemove = true;
                        break;
                    }
                }

                if (isLevelToRemove)
                {
                    listLevelConfig.Add(currentLevelConfig.name);
                    listLevel.Add(i + 1);
                }
            }

            var strLevelConfig = "";
            for (int i = 0; i < listLevelConfig.Count; i++)
            {
                strLevelConfig+= $"{listLevelConfig[i]}\n";
            }

            var strLevel = "";
            for (int i = 0; i < listLevel.Count; i++)
            {
                strLevel+= $"{listLevel[i]}\n";
            }
            
            Debug.LogError(strLevelConfig);
            Debug.LogError(strLevel);
        }
        
        [Button]
        void LogLevelWithCoin()
        {
            List<string> listLevelConfig = new List<string>();
            List<int> listLevel = new List<int>();
            for (int i = 0; i < levelConfigDataEdited.Count; i++)
            {
                var currentLevelConfig = levelConfigDataEdited[i];
                var listItemData = currentLevelConfig.GetItemDataForInstantiate();

                bool isLevelCoin = false;
                foreach (var item in listItemData)
                {
                    var itemDefinition = editorItemPrefabConfig.listItemDefinition.Find(x => x.GetId() == item.itemId);
                    if (itemDefinition == null)
                    {
                        Debug.LogError("Cant find item definition with id: " + item.itemId);
                        continue;
                    }

                    if (itemDefinition.GetId().ToLower().Contains("coin"))
                    {
                        isLevelCoin = true;
                        break;
                    }
                }

                if (isLevelCoin)
                {
                    listLevelConfig.Add(currentLevelConfig.name);
                    listLevel.Add(i + 1);
                }
            }

            var strLevelConfig = "";
            for (int i = 0; i < listLevelConfig.Count; i++)
            {
                strLevelConfig+= $"{listLevelConfig[i]}\n";
            }

            var strLevel = "";
            for (int i = 0; i < listLevel.Count; i++)
            {
                strLevel+= $"{listLevel[i]}\n";
            }
            
            Debug.LogError(strLevelConfig);
            Debug.LogError(strLevel);
        }
        
        
        [Button]
        public void GetLevelConfigDataEdited()
        {
            levelConfigDataEdited.Clear();
            for (int i = 0; i < levelConfigData.Count; i++)
            {

                if (excludedLevels.Contains(i + 1)) continue;
                levelConfigDataEdited.Add(levelConfigData[i]);
            }
        }

        // [Button]
        // public void AddNonFoodLevels()
        // {
        //     string resultLevel = "";
        //     for (int i = 0; i < levelConfigDataSHP.Count; i++)
        //     {
        //         var currentLevelConfig = levelConfigDataSHP[i];
        //         var listItemData = currentLevelConfig.GetItemDataForInstantiate();
        //
        //         bool isLevelToy = false;
        //         foreach (var item in listItemData)
        //         {
        //             var itemDefinition = editorItemPrefabConfig.listItemDefinition.Find(x => x.GetId() == item.itemId);
        //             if (itemDefinition == null)
        //             {
        //                 Debug.LogError("Cant find item definition with id: " + item.itemId);
        //                 continue;
        //             }
        //
        //             if (itemDefinition.GetFallingItemSetType() == FallingItemSetType.TOYS || itemDefinition.GetFallingItemSetType() == FallingItemSetType.SCHOOL_SUPPLIES)
        //             {
        //                 isLevelToy = true;
        //                 break;
        //             }
        //         }
        //
        //         if (isLevelToy)
        //         {
        //             Debug.LogError($"Add level toy {currentLevelConfig} at index to level {i+1}");
        //             resultLevel += $"{i + 1}\n";
        //
        //             if (i <= levelConfigDataEdited.Count)
        //             {
        //                 levelConfigDataEdited.Insert(i, currentLevelConfig);
        //             }
        //             else
        //             {
        //                 levelConfigDataEdited.Add(currentLevelConfig);
        //             }
        //         }
        //     }
        //     
        //     Debug.LogError(resultLevel);
        //     EditorUtility.SetDirty(this);
        //     AssetDatabase.SaveAssets();
        // }
        
#region BakeFromOriginalLevel

        [Header("for Bake from original level")] 
        [SerializeField] private LevelListParameters levelListParameters;
        [SerializeField] private string textAssetFolder;
        [SerializeField] private string outputFolder;
        [SerializeField] private List<GroundShapePresetParameters> listPresetGrid;

        [Button()]
        public void ResetLevelObjective()
        {
            foreach (var levelConfigModel in levelConfigData)
            {
                var levelpramester = levelListParameters.Levels.First(x => x.name == levelConfigModel.name);
                if(levelpramester == null) continue;
                foreach (var levelObjective in levelConfigModel.levelObjectives)
                {
                    levelObjective.number = levelpramester.GetListRecipeItem().First(x => x.Id == levelObjective.itemId)
                        .Count;
                }
            }
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }

        [Button]
        void UpdateTheme(int levelStart, int levelEnd, string levelTheme)
        {
            for (int i = levelStart; i <= levelEnd; i++)
            {
                levelConfigDataEdited[i - 1].groundSkin = levelTheme;
                UnityEditor.EditorUtility.SetDirty(levelConfigDataEdited[i - 1]);
                UnityEditor.AssetDatabase.SaveAssets();
            }
        }
        
        [Button()]
        public void Bake()
        {
            var assetFiles = GetAllTextAssetFiles(textAssetFolder);
            
            foreach (var levelDefinition in levelListParameters.Levels)
            {
                var newModel = CreateInstance<LevelConfigModel>();
                newModel.itemInstantiateItemData = assetFiles.Find(x=> IsMatchWithItemInstantiateDataName(levelDefinition.name,x.name));
                newModel.mapData = assetFiles.Find(x => IsMatchWithMapName(levelDefinition.name,x.name));
                if (newModel.mapData == null)
                {
                    newModel.presetMapData =
                        listPresetGrid.Find(x => IsMatchWithMapPreset(levelDefinition.GridUsed, x.Id));
                }
                newModel.duration = levelDefinition.GetDurationInSeconds();
                newModel.durationSave = levelDefinition.DurationInSecondsSave;
                
                newModel.levelObjectives = new List<LevelObjective>();
                foreach (var recipe in levelDefinition.GetListRecipeItem())
                {
                    var levelObjective = new LevelObjective();
                    levelObjective.itemId = recipe.Id;
                    levelObjective.number = recipe.Count;
                    newModel.levelObjectives.Add(levelObjective);
                }

                newModel.type = levelDefinition.Type;
                newModel.groundSkin = levelDefinition.GroundSkin;
                newModel.bombWeight = levelDefinition.BombWeight;
                newModel.radioActiveWeight = levelDefinition.RadioactiveWeight;
                newModel.negativeTimerWeight = levelDefinition.NegativeTimerWeight;
                string path = System.IO.Path.Combine(outputFolder,levelDefinition.name + ".asset");
                path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);
                UnityEditor.AssetDatabase.CreateAsset(newModel,path);
                levelConfigData.Add(newModel);

            }
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    
        [Button]
        public void BakeLevelDefinitionToLevelModel()
        {
            foreach (var levelConfigModel in levelConfigData)
            {
                var levelDefinition = levelListParameters.Levels.Find(x => x.name == levelConfigModel.name);
                if (levelDefinition == null) 
                {
                    Debug.LogError($"Missing definition {levelConfigModel.name}");
                    
                    //Try again
                    var nameFormatted = levelConfigModel.name.Substring(0, levelConfigModel.name.IndexOf(' '));
                    Debug.LogError(nameFormatted);
                    levelDefinition = levelListParameters.Levels.Find(x => x.name == nameFormatted);
                    if (levelDefinition == null)
                    {
                        continue;
                    }
                }
                levelConfigModel.levelDefinition = levelDefinition;
                UnityEditor.EditorUtility.SetDirty(levelConfigModel);
            }
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }

        [Button]
        public void GetUniqueItemId()
        {
            var str = "";
            HashSet<string> uniqueIds = new HashSet<string>();
            
            foreach (var levelConfigModel in levelConfigDataEdited)
            {
                var definition = levelConfigModel.levelDefinition;
                if (definition == null)
                {
                    Debug.LogError($"Missing level definition {levelConfigModel.name}");
                    continue;
                }

                foreach (var itemId in definition.ItemList)
                {
                    uniqueIds.Add(itemId.Id);
                }
            }

            foreach (var id in uniqueIds)
            {
                str += $"{id}\n";
            }

            GUIUtility.systemCopyBuffer = str;
            Debug.LogError(str);
        }

        [Button]
        public void GetItemIdByLevel()
        {
            var str = "";

            for (var index = 0; index < levelConfigDataEdited.Count; index++)
            {
                var levelConfigModel = levelConfigDataEdited[index];
                
                str += "\n";
                str += $"Level {index + 1} | {levelConfigModel.name} \n";

                var definition = levelConfigModel.levelDefinition;
                if (definition == null)
                {
                    Debug.LogError($"Missing level definition {levelConfigModel.name}");
                    continue;
                }

                foreach (var itemId in definition.ItemList)
                {
                    str += $"{itemId.Id} \n";
                }

                str += $"\nLevel Objectives \n";
                foreach (var objective in levelConfigModel.levelObjectives)
                {
                    str += $"{objective.itemId} \n";
                }
            }

            GUIUtility.systemCopyBuffer = str;
            Debug.LogError(str);
        }
        
        [Button()]
        public void CreateBombLevelData()
        {
            for (int i = 0; i < levelConfigData.Count; i++)
            {
                var levelConfig = levelConfigData[i];
                levelConfig.levelBoomData = new LevelBoomData();
                levelConfig.levelBoomData.scaleMin = 1;
                levelConfig.levelBoomData.scaleMax = 1.5f;
                levelConfig.levelBoomData.seedPosition = i;
                levelConfig.levelBoomData.seedScale = i + 1;
                levelConfig.levelBoomData.number = 0;
                var level = i + 1;
                var numberLogic = level % 50;
                if (numberLogic == 27)
                {
                    levelConfig.levelBoomData.number = 2;
                }else if (numberLogic == 28)
                {
                    levelConfig.levelBoomData.number = 2;
                }else if (numberLogic == 31)
                {
                    levelConfig.levelBoomData.number = 3;
                }else if (numberLogic == 32)
                {
                    levelConfig.levelBoomData.number = 3;
                }else if (numberLogic == 35)
                {
                    levelConfig.levelBoomData.number = 4;
                }else if (numberLogic == 36)
                {
                    levelConfig.levelBoomData.number = 4;
                }else if (numberLogic == 39)
                {
                    levelConfig.levelBoomData.number = 5;
                }else if (numberLogic == 40)
                {
                    levelConfig.levelBoomData.number = 5;
                }else if (numberLogic == 43)
                {
                    levelConfig.levelBoomData.number = 5;
                }else if (numberLogic == 44)
                {
                    levelConfig.levelBoomData.number = 5;
                }else if (numberLogic == 47)
                {
                    levelConfig.levelBoomData.number = 5;
                }else if (numberLogic == 48)
                {
                    levelConfig.levelBoomData.number = 5;
                }
                UnityEditor.EditorUtility.SetDirty(levelConfig);
                UnityEditor.AssetDatabase.SaveAssets();
            }
        }

        [Button]
        void BindDataLevelSHP()
        {
            levelConfigDataSHP.Clear();
            foreach (var level in levelConfigDataEdited)
            {
                var counterpartLevels = FindMatchingScriptableObject(level, "Assets/_IECProject/SHP Levels/");
                if (counterpartLevels == null)
                {
                    Debug.LogError("Cannot find counterpart level" + level.name);
                    continue;
                }
                
                levelConfigDataSHP.Add(counterpartLevels);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }
        
        public T FindMatchingScriptableObject<T>(T sourceObject, string searchPath = "Assets") where T : ScriptableObject
        {
#if UNITY_EDITOR
            if (sourceObject == null)
                return null;

            string sourcePath = AssetDatabase.GetAssetPath(sourceObject);
            string sourceFileName = Path.GetFileNameWithoutExtension(sourcePath);
        
            // Find all assets of the same type
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            
                // Skip if it's the source file
                if (assetPath == sourcePath)
                    continue;
                
                // Check if the file name matches
                if (Path.GetFileNameWithoutExtension(assetPath) == sourceFileName)
                {
                    // Verify the path contains the search directory
                    if (assetPath.StartsWith(searchPath, System.StringComparison.OrdinalIgnoreCase))
                    {
                        T foundObject = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                        if (foundObject != null)
                            return foundObject;
                    }
                }
            }
        
            return null;
#endif
            return null;
        }

        [Button]
        void UpdateOriginalSHPMapData()
        {
            var assetFiles = GetAllTextAssetFiles(textAssetFolder);
            foreach (var level in levelConfigDataSHP)
            {
                var textAsset = assetFiles.Find(x=> x.name == level.itemInstantiateItemData.name);
                if (textAsset == null)
                {
                    Debug.LogError("Missing text asset " + textAsset);
                    continue;
                }

                level.itemInstantiateItemData = textAsset;
#if UNITY_EDITOR
                EditorUtility.SetDirty(level);
                AssetDatabase.SaveAssets();
#endif
            }
        }

        [Button]
        void LogLevelDifficulties()
        {
            var csvString = "";
            csvString += "Index,Level Name,Difficulty\n";
            for (int i = 0; i < levelConfigDataEdited.Count; i++)
            {
                var levelConfig = levelConfigDataEdited[i];
                var levelDefinition = levelConfig.levelDefinition;
                if (levelDefinition == null)
                {
                    Debug.LogError($"Missing level definition {levelConfig.name}");
                    continue;
                }

                var difficulty = levelDefinition.Type;
                csvString += $"{i + 1},{levelConfig.name},{difficulty}\n";
            }

#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(csvString))
            {
                string path = "Assets/LevelDifficulties.csv";
                File.WriteAllText(path, csvString);
                AssetDatabase.Refresh();
            }
#endif
        }

        // [Button]
        // void LogLevelToyDifficulties()
        // {
        //     var csvString = "";
        //     for (int i = 0; i < levelConfigDataEdited.Count; i++)
        //     {
        //         var levelConfig = levelConfigDataEdited[i];
        //         var levelDefinition = levelConfig.levelDefinition;
        //         if (levelDefinition == null)
        //         {
        //             Debug.LogError($"Missing level definition {levelConfig.name}");
        //             continue;
        //         }
        //
        //         var listItemData = levelConfig.GetItemDataForInstantiate();
        //
        //         bool isLevelToy = false;
        //         foreach (var item in listItemData)
        //         {
        //             var itemDefinition = editorItemPrefabConfig.listItemDefinition.Find(x => x.GetId() == item.itemId);
        //             if (itemDefinition == null)
        //             {
        //                 Debug.LogError("Cant find item definition with id: " + item.itemId);
        //                 continue;
        //             }
        //
        //             if (itemDefinition.GetFallingItemSetType() == FallingItemSetType.TOYS ||
        //                 itemDefinition.GetFallingItemSetType() == FallingItemSetType.SCHOOL_SUPPLIES)
        //             {
        //                 isLevelToy = true;
        //                 break;
        //             }
        //         }
        //
        //         if (isLevelToy)
        //         {
        //             var difficulty = levelDefinition.Type;
        //             csvString += $"{difficulty},{i+1}\n";
        //         }
        //     }
        //     Debug.LogError(csvString);
        // }
        
        [Button]
        void BulkValidateLevels()
        {
            bool isHaveErrorLevel = false;
            for (var index = 0; index < levelData.listAllLevelData.Count; index++)
            {
                var level = index < 50 ? levelDataSHP.listAllLevelData[index] : levelData.listAllLevelData[index];
                var listObjectives = level.listObjectives;
                if (level.difficulty == "breaktime") continue;
                var listItemSpawn = level.levelSpawnData.listItemSpawns;
                foreach (var objective in listObjectives)
                {
                    var objectiveId = objective.id;

                    var count = 0;
                    foreach (var item in listItemSpawn)
                    {
                        if (item.id == objectiveId)
                        {
                            count += item.listSpawnDatas.Count;
                        }
                    }
                    // var allItemsOfType = listItemSpawn.FindAll(x => x.id == objectiveId);
                    if (count <= 0)
                    {
                        Debug.LogError($"Missing objective {objectiveId} at level {index + 1}");
                        isHaveErrorLevel = true;
                        continue;
                    }

                    if (count < objective.amount)
                    {
                        Debug.LogError($"Invalid count objective {objectiveId} | {count} / {objective.amount} at level {index + 1}");
                        isHaveErrorLevel = true;
                    }
                    else if (count > objective.amount)
                    {
                        Debug.LogError($"Redundant objective {objectiveId} | {count} / {objective.amount} at level {index + 1}");
                    }
                    else
                    {
                        Debug.Log($"Objective valid {objectiveId} | {count} / {objective.amount} at level {index + 1}");
                    }
                }
            }

            if (!isHaveErrorLevel)
            {
                Debug.LogError("<color=green>All levels are valid</color>");
            }            
        }
        
        private bool IsMatchWithMapName(string levelDefinitionName, string textAssetFileName)
        {
            return levelDefinitionName + " Level Ground" == textAssetFileName;
        }

        private bool IsMatchWithMapPreset(string mapPresetName, string mapPresetId)
        {
            return mapPresetName.Contains(mapPresetId);
        }
        private bool IsMatchWithItemInstantiateDataName(string levelDefinitionName, string textAssetFileName)
        {
            return levelDefinitionName + " Level Json" == textAssetFileName;
        }
        private List<TextAsset> GetAllTextAssetFiles(string rootFolder)
        {
            List<TextAsset> textAssets = new List<TextAsset>();

            // Ensure the folder path is valid
            if (!UnityEditor.AssetDatabase.IsValidFolder(rootFolder))
            {
                Debug.LogError($"Invalid folder path: {rootFolder}");
                return textAssets;
            }

            // Find all assets of type TextAsset in the specified folder
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset", new[] { rootFolder });

            foreach (string guid in guids)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                TextAsset textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                if (textAsset != null)
                {
                    textAssets.Add(textAsset);
                }
            }

            return textAssets;
        }
        
        [Button]
        void LogLevelNotStartWithMapData()
        {
            string str = "";
            for (var index = 0; index < levelConfigDataEdited.Count; index++)
            {
                var level = levelConfigDataEdited[index];
                var levelMapData = level.mapData;
                if (levelMapData == null)
                {
                    str += $"{level.name},{index + 1}\n";
                    continue;
                }

                if (!levelMapData.name.StartsWith("MapData") || level.isOverriddenByPreset)
                {
                    str += $"{level.name},{index + 1}\n";
                    continue;
                }
            }

            GUIUtility.systemCopyBuffer = str;
            Debug.LogError(str);
        }

        [Button]
        public void SetAllLevelSpawnKinematic()
        {
            foreach (var levelData in levelDataSHP.listAllLevelData)
            {
                levelData.levelSpawnData.SetAllSpawnDataKinematic();
                EditorUtility.SetDirty(levelData.levelSpawnData);
                AssetDatabase.SaveAssets();        
                AssetDatabase.Refresh();
            }
        }

        #endregion
#endif
    }
    [System.Serializable]
    public class LevelConfigDataModel
    {
        public int index;
        public LevelObject levelObject;
    }
    
}