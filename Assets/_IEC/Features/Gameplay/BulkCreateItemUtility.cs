using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using _IEC.Features.Gameplay;
using IEC;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif



public class BulkCreateItemUtility : MonoBehaviour
{
#if UNITY_EDITOR
    public string urlSheetCreate = "";
    public bool isAddItemToConfigAfterCreate;
    // public string namePrefix = "Instance_"; // Prefix for new prefab name
    // public string targetFolderPath = "Assets/GeneratedPrefabs";
    
    [Space]
    [ReadOnly] public List<ItemMapping> listItemMappings = new List<ItemMapping>();
    [ReadOnly] public List<ItemDefinition> listItemDefinitions = new List<ItemDefinition>();
    // public List<ItemReplaceUtility> itemToReplace = new List<ItemReplaceUtility>();

    [Header("Config")]
    public GameplayItemPrefabConfig itemPrefabConfig;
    public Object itemPrefabExportFolder;
    [ReadOnly] public string itemPrefabFolderExportPath;
    public Object itemIconFolder;
    [ReadOnly] public string itemIconFolderPath;
    public Object itemDefinitionFolder;
    [ReadOnly] public string itemDefinitionFolderPath;

    private const string FOLDER_PATH_ITEM_IN_GAME = "_IECProject/Items";
    private const string FOLDER_PATH_ITEM_DEFINITION = "_IEC/Features/Gameplay/Config/Items";
    private const string FOLDER_PATH_ICON = "RapidIcon/Exports";
    private const string FOLDER_PATH_SIMPLIFIED_MESH = "_IECProject/SimplifiedMesh";

    private void OnValidate()
    {
        itemPrefabFolderExportPath = AssetDatabase.GetAssetPath(itemPrefabExportFolder);
        itemIconFolderPath = AssetDatabase.GetAssetPath(itemIconFolder);
        itemDefinitionFolderPath = AssetDatabase.GetAssetPath(itemDefinitionFolder);
    }

    [Button]
    void BulkCreate()
    {
        StartCoroutine(IEFetchData(CreateItemPrefab));
    }

    // [Button]
    // void BulkCreateInstancePrefab()
    // {
    //     StartCoroutine(IEFetchData(DuplicatePrefabInstance));
    // }

    [Button]
    void BulkUpdateItemIcon()
    {
        StartCoroutine(IEFetchData(ReplaceIcons));
    }

    [Button]
    void BulkRemoveShadeItem()
    {
        StartCoroutine(IEFetchData(RemoveShadow));
    }

    [Button]
    void BulkBakePartialHull()
    {
        StartCoroutine(IEFetchData(BakePartialHull));
    }
    
    IEnumerator IEFetchData(Action endAction)
    {
        string csvUrl = urlSheetCreate.Replace("/edit?gid=", "/export?format=csv&gid=");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(csvUrl))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error downloading sheet: {webRequest.error}");
                yield break;
            }

            string csvText = webRequest.downloadHandler.text;
            ProcessCsvData(csvText);
            endAction?.Invoke();
        }
    }

    void ProcessCsvData(string csvText)
    {
        listItemMappings.Clear();

        var lines = csvText.Split("\n");
        for (int i = 1; i < lines.Length; i++)
        {
            var currentLine = lines[i].Trim();
            var column = currentLine.Split(',');

            if (column.Length >= 2)
            {
                var idMapping = new ItemMapping();
                idMapping.id = column[0];
                idMapping.prefabName = column[1];
                idMapping.specialItemId = (_IEC.Features.Gameplay.SpecialItemID) 
                    Enum.Parse(typeof(_IEC.Features.Gameplay.SpecialItemID), column[2]);
                idMapping.level = int.Parse(column[3]);
                idMapping.overrideLevel = int.Parse(column[4]);
                idMapping.xp = int.Parse(column[5]);
                idMapping.startAsKinematic = bool.Parse(column[6]);
                idMapping.canBeGoal = bool.Parse(column[7]);
                idMapping.ignoreInCalculations = bool.Parse(column[8]);
                idMapping.fallingItemSetType = (FallingItemSetType) Enum.Parse(typeof(FallingItemSetType), column[9]);
                idMapping.physicFallingItemType = (_IEC.Features.Gameplay.PhysicFallingItemType) 
                    Enum.Parse(typeof(_IEC.Features.Gameplay.PhysicFallingItemType), column[10]);
                listItemMappings.Add(idMapping);
            }
        }
    }

    void CreateItemPrefab()
    {
        listItemDefinitions.Clear();
        
        foreach (var mapping in listItemMappings)
        {
            if(mapping.prefabName.IsNullOrWhitespace()) continue;

            var prefabPath = Path.Combine(itemPrefabFolderExportPath, mapping.prefabName + ".prefab");
            var tempInstance = new GameObject();

            var reskinUtility = tempInstance.AddComponent<ItemReplaceUtility>();

            reskinUtility.ReplaceSkin(mapping.prefabName);
            PrefabUtility.SaveAsPrefabAsset(tempInstance, prefabPath);
            
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"Missing item with name {mapping.id}");
                continue;
            }
            
            string path = Path.Combine(itemDefinitionFolderPath, mapping.id + ".asset");
            ItemDefinition itemDefinition = Utilities.FindAssetInFolder<ItemDefinition>(FOLDER_PATH_ITEM_DEFINITION, mapping.id);
            if (itemDefinition == null)
            {
                itemDefinition = ScriptableObject.CreateInstance<ItemDefinition>();
            }
            
            itemDefinition.SetPrefab(prefab);
            itemDefinition.SetId(mapping.id);
            itemDefinition.SetIcon(Utilities.FindAssetInFolder<Sprite>(itemIconFolderPath, mapping.prefabName));
            itemDefinition.SetSpecialId(mapping.specialItemId);
            itemDefinition.SetLevel(mapping.level);
            itemDefinition.SetOverrideLevel(mapping.overrideLevel);
            itemDefinition.SetXp(mapping.xp);
            itemDefinition.SetStartAsKinematic(mapping.startAsKinematic);
            itemDefinition.SetCanBeGoal(mapping.canBeGoal);
            itemDefinition.SetIgnoreInCalculations(mapping.ignoreInCalculations);
            itemDefinition.SetFallingItemSetType(mapping.fallingItemSetType);
            itemDefinition.SetPhysicFallingItemType(mapping.physicFallingItemType);
            
            if (!File.Exists(path))
            {
                AssetDatabase.CreateAsset(itemDefinition, path);
            }
            
            itemDefinition.SetDirty();
            var loadedItemDefinition = AssetDatabase.LoadAssetAtPath<ItemDefinition>(path);
            listItemDefinitions.Add(loadedItemDefinition);
            DestroyImmediate(tempInstance);

            
            AssetDatabase.SaveAssets();
        }
        
        if (isAddItemToConfigAfterCreate)
        {
            foreach (var itemDefinition in listItemDefinitions)
            {
                if (itemPrefabConfig.ListItemDefinition.Exists(x => x.GetId() == itemDefinition.GetId())) continue;
                itemPrefabConfig.ListItemDefinition.Add(itemDefinition);
            }
            EditorUtility.SetDirty(itemPrefabConfig);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

//     void DuplicatePrefabInstance()
//     {
//         foreach (var mapping in listItemMappings)
//         {
//             if(mapping.prefabName.IsNullOrWhitespace()) continue;
//
//             var originalPrefab = Utilities.FindAssetInFolder<GameObject>(FOLDER_PATH_ITEM_IN_GAME, mapping.OriginalID);
//             if (originalPrefab == null)
//             {
//                 Debug.LogError($"Missing item with name {mapping.OriginalID}");
//                 continue;
//             }
//             
//             GameObject tempInstance = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;
//
//             if (tempInstance == null)
//             {
//                 Debug.LogError($"Missing item with name {mapping.OriginalID}");
//                 continue;
//             }
//             
//             // Generate a unique name with the prefix
//             string uniqueName = GetUniqueFileName(namePrefix + tempInstance.name);
//             tempInstance.name = uniqueName;
//             
//             // Create the full path for the new prefab
//             string prefabPath = Path.Combine(targetFolderPath, uniqueName + ".prefab");
//         
//             // Convert the path to use forward slashes for Unity
//             prefabPath = prefabPath.Replace("\\", "/");
//
//             // Create the new prefab asset
// #if UNITY_EDITOR
//             GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(tempInstance, prefabPath);
//             Debug.Log($"New prefab created at: {prefabPath}");
//         
//             // Optionally destroy the scene instance if you don't need it
//             DestroyImmediate(tempInstance);
// #else
//         Debug.LogError("This function can only be used in the Unity Editor!");
//         return null;
// #endif
//         }
//     }
    
    // private string GetUniqueFileName(string baseName)
    // {
    //     string uniqueName = baseName;
    //     int counter = 1;
    //
    //     // Keep incrementing counter until we find a unique name
    //     while (File.Exists(Path.Combine(targetFolderPath, uniqueName + ".prefab")))
    //     {
    //         uniqueName = $"{baseName}_{counter}";
    //         counter++;
    //     }
    //
    //     return uniqueName;
    // }

    void ReplaceIcons()
    {
        foreach (var mapping in listItemMappings)
        {
            if(mapping.prefabName.IsNullOrWhitespace()) continue;

            List<ItemDefinition> itemDefinitionAsset = new List<ItemDefinition>();
            
            // Find all ScriptableObject asset guids in the project
            var guids = AssetDatabase.FindAssets($"t:{typeof(ItemDefinition).Name}");

            // Load each ScriptableObject asset and check the prefab field
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var itemDefinition = AssetDatabase.LoadAssetAtPath<ItemDefinition>(path);
                if (itemDefinition == null)
                {
                    Debug.LogError($"Missing at {path}");
                    continue;
                }

                if (itemDefinition.GetPrefab() == null)
                {
                    Debug.LogError($"No prefab at {path}");
                    continue;
                }
                
                if (itemDefinition.GetPrefab()?.name == mapping.prefabName)
                {
                    itemDefinitionAsset.Add(itemDefinition);
                }
            }
            
            if (itemDefinitionAsset.Count <= 0)
            {
                Debug.LogError($"Cannot find definition for {mapping.prefabName}");
            }
            
            var newIcon = Utilities.FindAssetInFolder<Sprite>(FOLDER_PATH_ICON, mapping.prefabName);

            if (newIcon == null)
            {
                Debug.LogError($"Cannot find icon {mapping.prefabName}");
                continue;
            }

            foreach (var definition in itemDefinitionAsset)
            {
                definition.SetIcon(newIcon);
                definition.SetDirty();
            }
           
            AssetDatabase.SaveAssets();
        }
    }

    void RemoveShadow()
    {
        foreach (var mapping in listItemMappings)
        {
            var originalPrefab = Utilities.FindAssetInFolder<GameObject>(FOLDER_PATH_ITEM_IN_GAME, mapping.prefabName);
            if (originalPrefab == null)
            {
                Debug.LogError($"Missing item with name {mapping.prefabName}");
                continue;
            }
            
            string prefabPath = AssetDatabase.GetAssetPath(originalPrefab);
            GameObject tempInstance = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;

            if (tempInstance == null)
            {
                Debug.LogError($"Missing item with name {mapping.prefabName}");
                continue;
            }

            var meshRenderers = tempInstance.GetComponentsInChildren<MeshRenderer>();
            for (var index = 0; index < meshRenderers.Length; index++)
            {
                var mr = meshRenderers[index];
                mr.receiveShadows = false;
                mr.lightProbeUsage = LightProbeUsage.Off;
                mr.reflectionProbeUsage = ReflectionProbeUsage.Off;
            }

            PrefabUtility.SaveAsPrefabAsset(tempInstance, prefabPath);
            DestroyImmediate(tempInstance);
        }
    }
    
    void BakePartialHull()
    {
        foreach (var mapping in listItemMappings)
        {
            if(mapping.prefabName.IsNullOrWhitespace()) continue;
            
            var originalPrefab = Utilities.FindAssetInFolder<GameObject>(FOLDER_PATH_ITEM_IN_GAME, mapping.prefabName);
            if (originalPrefab == null)
            {
                Debug.LogError($"Missing item with name {mapping.prefabName}");
                continue;
            }
            
            string prefabPath = AssetDatabase.GetAssetPath(originalPrefab);
            GameObject tempInstance = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;
            
            if (tempInstance == null)
            {
                Debug.LogError($"Missing item with name {mapping.prefabName}");
                continue;
            }

            foreach (Transform childObject in tempInstance.transform)
            {
                var meshFilter = childObject.GetComponent<MeshFilter>();
                if (meshFilter == null || !meshFilter.gameObject.activeInHierarchy) continue;

                var meshCollider = meshFilter.gameObject.GetComponent<Collider>();
                if (meshCollider == null || !meshCollider.enabled) continue;

                var sourceMesh = meshFilter.sharedMesh;
                if (sourceMesh == null) continue;

                var currentPolygon = meshFilter.sharedMesh.triangles.Length / 3;
                if(currentPolygon < 256) continue;
    
                var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
                meshSimplifier.Initialize(sourceMesh);
                var quality = 256f / currentPolygon;
    
                meshSimplifier.SimplifyMesh(quality);
                Mesh decimatedMesh = meshSimplifier.ToMesh();
    
                // Convert the full path to a path relative to Assets folder
                string assetPath = "Assets/" + FOLDER_PATH_SIMPLIFIED_MESH + meshFilter.name + ".asset";
                assetPath = assetPath.Replace("\\", "/");

                // Ensure the directory exists
                string directoryPath = Path.GetDirectoryName(assetPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Delete existing asset if it exists
                if (AssetDatabase.LoadAssetAtPath<Mesh>(assetPath) != null)
                {
                    AssetDatabase.DeleteAsset(assetPath);
                    AssetDatabase.Refresh();
                }
    
                // Create and save the new asset
                AssetDatabase.CreateAsset(decimatedMesh, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (meshCollider is MeshCollider col)
                {
                    col.sharedMesh = decimatedMesh;
                }
            }
            
            PrefabUtility.SaveAsPrefabAsset(tempInstance, prefabPath);
            DestroyImmediate(tempInstance);
        }
    }
#endif
    
    [Serializable]
    public class ItemMapping
    {
        public string id;
        public string prefabName;
        public _IEC.Features.Gameplay.SpecialItemID specialItemId;
        public int level;
        public int overrideLevel;
        public int xp;
        public bool startAsKinematic;
        public bool canBeGoal;
        public bool ignoreInCalculations;
        public FallingItemSetType fallingItemSetType;
        public _IEC.Features.Gameplay.PhysicFallingItemType physicFallingItemType;
    }
}
