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
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class ItemIdConfig
{
    public string OriginalID;
    public string NewID;
}

[ExecuteAlways]
public class BulkReplaceItemUtility : MonoBehaviour
{
#if UNITY_EDITOR
    public string urlSheetReplace = "";
    public string namePrefix = "Instance_"; // Prefix for new prefab name
    public string targetFolderPath = "Assets/GeneratedPrefabs";
    
    [Space]
    public List<ItemIdConfig> idMappings = new List<ItemIdConfig>();
    public List<ItemReplaceUtility> itemToReplace = new List<ItemReplaceUtility>();

    [Space] public LevelConfig levelConfig;
    public GameplayItemPrefabConfig itemPrefabConfig;
    
    private const string FOLDER_PATH_ITEM_IN_GAME = "_IECProject/Items";
    private const string FOLDER_PATH_ITEM_DEFINITION = "_IEC/Features/Gameplay/Config/Items";
    private const string FOLDER_PATH_ICON = "RapidIcon/Exports";
    private const string FOLDER_PATH_SIMPLIFIED_MESH = "_IECProject/SimplifiedMesh";

    [Button]
    void BulkReplace()
    {
        StartCoroutine(IEReplaceSkin(ReplaceItemPrefab));
    }

    [Button]
    void BulkCreateInstancePrefab()
    {
        StartCoroutine(IEReplaceSkin(DuplicatePrefabInstance));
    }

    [Button]
    void BulkUpdateItemIcon()
    {
        StartCoroutine(IEReplaceSkin(ReplaceIcons));
    }

    [Button]
    void BulkRemoveShadeItem()
    {
        StartCoroutine(IEReplaceSkin(RemoveShadow));
    }

    [Button]
    void BulkBakePartialHull()
    {
        StartCoroutine(IEReplaceSkin(BakePartialHull));
    }
    
    IEnumerator IEReplaceSkin(Action endAction)
    {
        string csvUrl = urlSheetReplace.Replace("/edit?gid=", "/export?format=csv&gid=");
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

    [Button]
    void LogItemByLevel()
    {
        StartCoroutine(IEConsoleLogItemFirstAppear());
    }

    IEnumerator IEConsoleLogItemFirstAppear()
    {
        string csvUrl = urlSheetReplace.Replace("/edit?gid=", "/export?format=csv&gid=");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(csvUrl))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error downloading sheet: {webRequest.error}");
                yield break;
            }

            var itemIdsDict = new Dictionary<string, int>();
            string csvText = webRequest.downloadHandler.text;
            var lines = csvText.Split('\n');
            for (int i = 1; i < lines.Length; i++)
            {
                var currentLine = lines[i].Trim();
                var column = currentLine.Split(',');

                if(itemIdsDict.ContainsKey(column[0]))
                {
                    Debug.LogError("Dupplicated key: " + column[0]);
                }
                itemIdsDict[column[0]] = -999;
            }

            var strLog = "";
            for (int levelIndex = 0; levelIndex < levelConfig.levelConfigDataEdited.Count; levelIndex++)
            {
                var levelDefinition = levelConfig.levelConfigDataEdited[levelIndex];
                var listItemData = levelDefinition.GetItemDataForInstantiate(); //get all items in the levels

                foreach (var item in listItemData)
                { 
                    var itemDefinition = itemPrefabConfig.ListItemDefinition.Find(x => x.GetId() == item.itemId);
                    if (itemDefinition == null)
                    {
                        Debug.LogError("Cant find item definition with id: " + item.itemId);
                        continue;
                    }

                    var prefabName = itemDefinition.GetPrefab().name;
                    if(prefabName == "BigDonut_01")
                    {
                        Debug.LogError("test");
                    }
                    if (itemIdsDict.ContainsKey(prefabName))
                    {
                        // Only update if the current value is 0 or less
                        if (itemIdsDict[prefabName] <= 0)
                        {
                            itemIdsDict[prefabName] = levelIndex + 1;
                        }
                    }
                }
            }

            strLog = string.Join("\n", itemIdsDict.Values);
            Debug.LogError(strLog);
            GUIUtility.systemCopyBuffer = strLog;
        }
    }

    void ProcessCsvData(string csvText)
    {
        idMappings.Clear();

        var lines = csvText.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            var currentLine = lines[i].Trim();
            var column = currentLine.Split(',');

            if (column.Length >= 2)
            {
                var idMapping = new ItemIdConfig();
                idMapping.OriginalID = column[0];
                idMapping.NewID = column[1];
                idMappings.Add(idMapping);
            }
        }
    }

    void ReplaceItemPrefab()
    {
        foreach (var mapping in idMappings)
        {
            if(mapping.NewID.IsNullOrWhitespace()) continue;

            var originalPrefab = Utilities.FindAssetInFolder<GameObject>(FOLDER_PATH_ITEM_IN_GAME, mapping.OriginalID);
            if (originalPrefab == null)
            {
                Debug.LogError($"Missing item with name {mapping.OriginalID}");
                continue;
            }
            
            string prefabPath = AssetDatabase.GetAssetPath(originalPrefab);
            GameObject tempInstance = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;

            if (tempInstance == null)
            {
                Debug.LogError($"Missing item with name {mapping.OriginalID}");
                continue;
            }
            
            var reskinUtility = tempInstance.GetComponent<ItemReplaceUtility>();
            if (reskinUtility == null)
            {
                Debug.LogError($"Missing reskin utility in item {originalPrefab.name}");
                continue;
            }

            reskinUtility.ReplaceSkin(mapping.NewID);
            PrefabUtility.SaveAsPrefabAsset(tempInstance, prefabPath);
            DestroyImmediate(tempInstance);
        }
    }

    void DuplicatePrefabInstance()
    {
        foreach (var mapping in idMappings)
        {
            if(mapping.NewID.IsNullOrWhitespace()) continue;

            var originalPrefab = Utilities.FindAssetInFolder<GameObject>(FOLDER_PATH_ITEM_IN_GAME, mapping.OriginalID);
            if (originalPrefab == null)
            {
                Debug.LogError($"Missing item with name {mapping.OriginalID}");
                continue;
            }
            
            GameObject tempInstance = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;

            if (tempInstance == null)
            {
                Debug.LogError($"Missing item with name {mapping.OriginalID}");
                continue;
            }
            
            // Generate a unique name with the prefix
            string uniqueName = GetUniqueFileName(namePrefix + tempInstance.name);
            tempInstance.name = uniqueName;
            
            // Create the full path for the new prefab
            string prefabPath = Path.Combine(targetFolderPath, uniqueName + ".prefab");
        
            // Convert the path to use forward slashes for Unity
            prefabPath = prefabPath.Replace("\\", "/");

            // Create the new prefab asset
#if UNITY_EDITOR
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(tempInstance, prefabPath);
            Debug.Log($"New prefab created at: {prefabPath}");
        
            // Optionally destroy the scene instance if you don't need it
            DestroyImmediate(tempInstance);
#else
        Debug.LogError("This function can only be used in the Unity Editor!");
        return null;
#endif
        }
    }
    
    private string GetUniqueFileName(string baseName)
    {
        string uniqueName = baseName;
        int counter = 1;

        // Keep incrementing counter until we find a unique name
        while (File.Exists(Path.Combine(targetFolderPath, uniqueName + ".prefab")))
        {
            uniqueName = $"{baseName}_{counter}";
            counter++;
        }

        return uniqueName;
    }

    void ReplaceIcons()
    {
        foreach (var mapping in idMappings)
        {
            if(mapping.NewID.IsNullOrWhitespace()) continue;

            List<ItemDefinition> itemDefinitionAsset = new List<ItemDefinition>();
            
            // Find all ScriptableObject asset guids in the project
            var guids = AssetDatabase.FindAssets($"t:{typeof(ItemDefinition).Name}");

            // Load each ScriptableObject asset and check the prefab field
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var scriptableObject = AssetDatabase.LoadAssetAtPath<ItemDefinition>(path);
                if (scriptableObject == null)
                {
                    Debug.LogError($"Missing at {path}");
                    continue;
                }

                if (scriptableObject.GetPrefab() == null)
                {
                    Debug.LogError($"No prefab at {path}");
                    continue;
                }
                
                if (scriptableObject.GetPrefab()?.name == mapping.OriginalID)
                {
                    itemDefinitionAsset.Add(scriptableObject);
                }
            }
            
            if (itemDefinitionAsset.Count <= 0)
            {
                Debug.LogError($"Cannot find definition for {mapping.OriginalID}");
            }
            
            var newIcon = Utilities.FindAssetInFolder<Sprite>(FOLDER_PATH_ICON, mapping.OriginalID);

            if (newIcon == null)
            {
                Debug.LogError($"Cannot find icon {mapping.OriginalID}");
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
        foreach (var mapping in idMappings)
        {
            var originalPrefab = Utilities.FindAssetInFolder<GameObject>(FOLDER_PATH_ITEM_IN_GAME, mapping.OriginalID);
            if (originalPrefab == null)
            {
                Debug.LogError($"Missing item with name {mapping.OriginalID}");
                continue;
            }
            
            string prefabPath = AssetDatabase.GetAssetPath(originalPrefab);
            GameObject tempInstance = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;

            if (tempInstance == null)
            {
                Debug.LogError($"Missing item with name {mapping.OriginalID}");
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
        foreach (var mapping in idMappings)
        {
            if(mapping.NewID.IsNullOrWhitespace()) continue;
            
            var originalPrefab = Utilities.FindAssetInFolder<GameObject>(FOLDER_PATH_ITEM_IN_GAME, mapping.OriginalID);
            if (originalPrefab == null)
            {
                Debug.LogError($"Missing item with name {mapping.OriginalID}");
                continue;
            }
            
            string prefabPath = AssetDatabase.GetAssetPath(originalPrefab);
            GameObject tempInstance = PrefabUtility.InstantiatePrefab(originalPrefab) as GameObject;
            
            if (tempInstance == null)
            {
                Debug.LogError($"Missing item with name {mapping.OriginalID}");
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
    
                //var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
                //meshSimplifier.Initialize(sourceMesh);
                var quality = 256f / currentPolygon;
    
                //meshSimplifier.SimplifyMesh(quality);
                //Mesh decimatedMesh = meshSimplifier.ToMesh();
    
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
                //AssetDatabase.CreateAsset(decimatedMesh, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (meshCollider is MeshCollider col)
                {
                    //col.sharedMesh = decimatedMesh;
                }
            }
            
            PrefabUtility.SaveAsPrefabAsset(tempInstance, prefabPath);
            DestroyImmediate(tempInstance);
        }
    }
#endif
}
