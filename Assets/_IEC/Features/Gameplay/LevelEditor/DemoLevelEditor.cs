using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using _IEC.Features.ExportLevel.OriginalLevel;
using _IEC.Features.Gameplay;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
using UnityEditor;
using UnityEngine.Serialization;


namespace IEC
{
    public class DemoLevelEditor : MonoBehaviour
    {
#if UNITY_EDITOR
        public Transform mapParent;

        [InfoBox("Which items to ignore, if the item name contains one of the following strings, it will be ignored")]
        public List<string> listIgnoreItemNames;

        [Header("Create Items")]
        public string urlSheetCreate;
        public bool isAddItemToConfigAfterCreate;
        public List<ItemMapping> listItemMappings = new List<ItemMapping>();
        public List<ItemDefinition> listItemDefinitions = new List<ItemDefinition>();

        [Header("Config")]
        public GameplayItemPrefabConfig itemPrefabConfig;
        [FormerlySerializedAs("itemPrefabFolder")]
        public Object itemPrefabExportFolder;
        [FormerlySerializedAs("itemPrefabFolderPath")]
        [ReadOnly] public string itemPrefabExportFolderPath;
        public Object itemIconFolder;
        [ReadOnly] public string itemIconFolderPath;
        public Object itemDefinitionFolder;
        [ReadOnly] public string itemDefinitionFolderPath;
        public Object itemMapFolder;
        [ReadOnly] public string itemMapFolderPath;

        private const string FOLDER_PATH_ITEM_IN_GAME = "_IECProject/Items";
        private const string FOLDER_PATH_ITEM_DEFINITION = "_IEC/Features/Gameplay/Config/Items";
        private const string FOLDER_PATH_SIMPLIFIED_MESH = "_IECProject/SimplifiedMesh";

        private void OnValidate()
        {
            itemPrefabExportFolderPath = AssetDatabase.GetAssetPath(itemPrefabExportFolder);
            itemIconFolderPath = AssetDatabase.GetAssetPath(itemIconFolder);
            itemDefinitionFolderPath = AssetDatabase.GetAssetPath(itemDefinitionFolder);
            itemMapFolderPath = AssetDatabase.GetAssetPath(itemMapFolder);
        }

        [Button]
        public void CreateItemsFromSheet()
        {
            StartCoroutine(IEFetchData(CreateNewItemsFromSheet));
        }

        [Button]
        public void CreateItemsFromCurrentScene()
        {
            if (mapParent == null)
            {
                EditorUtility.DisplayDialog("Error", "Map parent is null", "OK");
                return;
            }

            listItemDefinitions.Clear();
            for (int i = 0; i < mapParent.childCount; i++)
            {
                var go = mapParent.GetChild(i).gameObject;
                var prefab = GetOriginalPrefab(go);

                if (prefab == null)
                {
                    Debug.LogError($"Missing prefab for {go.name}");
                    continue;
                }

                var prefabName = go.name.Split(' ')[0].Replace("(Clone)", "");
                if (listIgnoreItemNames.Exists(x => prefabName.ToLower().Contains(x.ToLower()))) continue;

                var itemDefinition = GetItemDefinition(prefabName);
                if (listItemDefinitions.Exists(x => x.GetId() == prefabName)) continue;

                if (itemDefinition == null)
                {
                    itemDefinition = ScriptableObject.CreateInstance<ItemDefinition>();
                }

                var prefabPath = Path.Combine(itemPrefabExportFolderPath, prefabName + ".prefab");
                var tempInstance = new GameObject();

                var reskinUtility = tempInstance.AddComponent<ItemReplaceUtility>();

                reskinUtility.ReplaceSkin(prefabName, true);
                BakePartialHull(tempInstance);
                PrefabUtility.SaveAsPrefabAsset(tempInstance, prefabPath);

                var loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (loadedPrefab == null)
                {
                    Debug.LogError($"Missing item with name {prefabName}");
                    continue;
                }

                string itemDefinitionPath = Path.Combine(itemDefinitionFolderPath, prefabName + ".asset");

                itemDefinition.SetPrefab(loadedPrefab);
                itemDefinition.SetId(prefabName);
                itemDefinition.SetIcon(Utilities.FindAssetInFolder<Sprite>(itemIconFolderPath, prefabName));
                itemDefinition.SetStartAsKinematic(true);
                itemDefinition.SetCanBeGoal(true);

                if (!File.Exists(itemDefinitionPath))
                {
                    AssetDatabase.CreateAsset(itemDefinition, itemDefinitionPath);
                }

                itemDefinition.SetDirty();
                var loadedItemDefinition = AssetDatabase.LoadAssetAtPath<ItemDefinition>(itemDefinitionPath);
                listItemDefinitions.Add(loadedItemDefinition);
                DestroyImmediate(tempInstance);
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
        }

        private void BakePartialHull(GameObject tempInstance)
        {
            foreach (Transform childObject in tempInstance.transform)
            {
                var meshFilter = childObject.GetComponent<MeshFilter>();
                if (meshFilter == null || !meshFilter.gameObject.activeInHierarchy) continue;

                var meshCollider = meshFilter.gameObject.GetComponent<Collider>();
                if (meshCollider == null || !meshCollider.enabled) continue;

                var sourceMesh = meshFilter.sharedMesh;
                if (sourceMesh == null) continue;

                var currentPolygon = meshFilter.sharedMesh.triangles.Length / 3;
                if (currentPolygon < 256) continue;

               // var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
                //meshSimplifier.Initialize(sourceMesh);
                var quality = 256f / currentPolygon;

                //meshSimplifier.SimplifyMesh(quality);
               // Mesh decimatedMesh = meshSimplifier.ToMesh();

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

        private void ProcessCsvData(string csvText)
        {
            listItemMappings.Clear();

            var lines = csvText.Split('\n');
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
                    idMapping.fallingItemSetType =
                        (FallingItemSetType)Enum.Parse(typeof(FallingItemSetType), column[9]);
                    idMapping.physicFallingItemType = (_IEC.Features.Gameplay.PhysicFallingItemType)
                        Enum.Parse(typeof(_IEC.Features.Gameplay.PhysicFallingItemType), column[10]);
                    listItemMappings.Add(idMapping);
                }
            }
        }

        private void CreateNewItemsFromSheet()
        {
            listItemDefinitions.Clear();

            foreach (var mapping in listItemMappings)
            {
                if (mapping.prefabName.IsNullOrWhitespace()) continue;

                if (GetItemDefinition(mapping.prefabName) != null)
                {
                    Debug.LogWarning($"Item with name {mapping.prefabName} already exists");
                    continue;
                }

                var prefabPath = Path.Combine(itemPrefabExportFolderPath, mapping.prefabName + ".prefab");
                var tempInstance = new GameObject();

                var reskinUtility = tempInstance.AddComponent<ItemReplaceUtility>();

                reskinUtility.ReplaceSkin(mapping.prefabName, true);
                PrefabUtility.SaveAsPrefabAsset(tempInstance, prefabPath);

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab == null)
                {
                    Debug.LogError($"Missing item with name {mapping.id}");
                    continue;
                }

                string path = Path.Combine(itemDefinitionFolderPath, mapping.id + ".asset");
                ItemDefinition itemDefinition =
                    Utilities.FindAssetInFolder<ItemDefinition>(FOLDER_PATH_ITEM_DEFINITION, mapping.id);
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

        [Button]
        public void SaveMapItems()
        {
            if (!mapParent)
            {
                EditorUtility.DisplayDialog("Error", "Map parent is null", "OK");
                return;
            }

            var duplicatedMapParent = DuplicateGameObject();
            if (!duplicatedMapParent)
            {
                Debug.LogError("Failed to duplicate map parent.");
                return;
            }

            Root root = new Root();
            root.InstantiateData = new List<InstantiateData>();

            var childCount = duplicatedMapParent.transform.childCount;
            var listDestroyTransforms = new List<Transform>();
            for (int i = 0; i < childCount; i++)
            {
                var go = duplicatedMapParent.transform.GetChild(i).gameObject; // To get prefab
                var prefabName = go.name.Split(' ')[0].Replace("(Clone)", "");

                if (listIgnoreItemNames.Exists(x => prefabName.ToLower().Contains(x.ToLower()))) continue;

                var itemDefinition = GetItemDefinition(prefabName);
                if (itemDefinition == null)
                {
                    Debug.LogError($"Missing item definition for {prefabName}");
                    continue;
                }

                var loadedPrefab = itemDefinition.GetPrefab();
                if (loadedPrefab == null)
                {
                    Debug.LogError($"Missing item with name {prefabName}");
                    continue;
                }

                var tempInstance = Instantiate(loadedPrefab, go.transform.position, go.transform.rotation,
                    duplicatedMapParent.transform);
                tempInstance.transform.SetAsLastSibling();
                listDestroyTransforms.Add(go.transform);

                var itemComponent = tempInstance.GetComponent<BaseItem>();
                if (itemComponent == null)
                {
                    itemComponent = tempInstance.AddComponent<BaseItem>();
                }

                var id = itemDefinition.GetId();
                var instantiatedData = root.InstantiateData.Find(x => x.itemId == id);
                if (instantiatedData == null)
                {
                    instantiatedData = new InstantiateData
                    {
                        itemId = id,
                        itemData = new List<_IEC.Features.ExportLevel.OriginalLevel.ItemData>()
                    };

                    root.InstantiateData.Add(instantiatedData);
                }

                var itemData = new _IEC.Features.ExportLevel.OriginalLevel.ItemData();
                itemData.pos = new Position(itemComponent.transform.position);
                itemData.rot = new Rotation(itemComponent.transform.eulerAngles);
                itemData.scale = new Scale(go.transform.localScale);
                itemData.sk = true;

                instantiatedData.itemData.Add(itemData);
            }

            foreach (var tf in listDestroyTransforms)
            {
                DestroyImmediate(tf.gameObject);
            }

            string path = EditorUtility.SaveFilePanelInProject("Save map", "Item Map", "txt",
                "Please enter a file name to save the texture to", itemMapFolderPath);

            if (path.Length != 0)
            {
                var json = JsonConvert.SerializeObject(root);
                File.WriteAllText(path, json);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            DestroyImmediate(duplicatedMapParent);
        }

        private GameObject DuplicateGameObject()
        {
            if (mapParent == null)
            {
                Debug.LogError("No object specified to duplicate.");
                return null;
            }

            GameObject duplicatedObject = Instantiate(mapParent.gameObject, mapParent.transform.position,
                mapParent.transform.rotation);
            duplicatedObject.name = mapParent.name + "_Copy";
            // DuplicateChildren(mapParent.transform, duplicatedObject.transform);
            return duplicatedObject;
        }

        private void DuplicateChildren(Transform original, Transform duplicate)
        {
            foreach (Transform child in original)
            {
                GameObject childCopy = Instantiate(child.gameObject, child.position, child.rotation, duplicate);
                childCopy.name = child.gameObject.name;
                DuplicateChildren(child, childCopy.transform);
            }
        }

        private ItemDefinition GetItemDefinition(string prefabName)
        {
            return itemPrefabConfig.ListItemDefinition.Find(x => x.GetPrefab() && x.GetPrefab()?.name == prefabName);
        }

        private GameObject GetOriginalPrefab(GameObject obj)
        {
            return PrefabUtility.GetCorrespondingObjectFromSource(obj);
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
}