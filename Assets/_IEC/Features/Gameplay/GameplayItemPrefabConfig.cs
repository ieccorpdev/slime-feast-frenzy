using System.Collections.Generic;
using System.IO;
using System.Linq;
using IEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _IEC.Features.Gameplay
{
    [CreateAssetMenu(menuName = "Gameplay/ItemPrefabConfig")]
    public class GameplayItemPrefabConfig : ScriptableObject
    {
        [Searchable]
        public List<ItemDefinition> listItemDefinition;
        [Space(10)]
        public List<ItemDefinition> listItemDefinitionShp;
        #region Bake
        
#if  UNITY_EDITOR
        [SerializeField] private DefaultAsset folder;
#endif
        [ReadOnly, ShowInInspector] string _folderPath;

        [SerializeField] private string outputFolder;
        [SerializeField] private string prefabFolder;
        
        // public List<ItemDefinition> ListItemDefinition => Pool.Get<PlayerSecret>().level <= 50 && Pool.Get<PlayerSecret>().level % 20 != 0
        //     ? listItemDefinitionShp : listItemDefinition; 
        public List<ItemDefinition> ListItemDefinition => listItemDefinitionShp; 
        
        public List<ItemDefinition> GetListItemDefinition()
        {
            //return ListItemDefinition;
            return listItemDefinitionShp;
        }
        
        
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
        void RemoveInactiveObject()
        {
            foreach (var item in listItemDefinition)
            {
                var prefab = item.GetPrefab();
                if (prefab == null)
                {
                    Debug.LogError("Missing prefab " + item.GetId());
                    continue;
                }

                // Create a temporary copy of the prefab to modify
                GameObject tempPrefab = Instantiate(prefab);
            
                // Find inactive children in the temporary prefab
                var inactiveChildren = tempPrefab.GetComponentsInChildren<Transform>(true)
                    .Where(t => !t.gameObject.activeSelf)
                    .Select(t => t.gameObject)
                    .ToList();

                bool prefabModified = false;
                foreach (var inactiveChild in inactiveChildren)
                {
                    // Destroy the inactive child in the temporary prefab
                    DestroyImmediate(inactiveChild);
                    prefabModified = true;
                }

                if (prefabModified)
                {
                    // Save the modified prefab
                    PrefabUtility.SaveAsPrefabAsset(tempPrefab, AssetDatabase.GetAssetPath(prefab));
                    Debug.Log($"Processed prefab: {prefab.name} | Removed inactive GameObjects");
                }

                // Destroy the temporary prefab instance
                DestroyImmediate(tempPrefab);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [Button]
        public void LogItemWithExp(int expValue)
        {
            var result = "";
            for (var index = 0; index < listItemDefinition.Count; index++)
            {
                var item = listItemDefinition[index];
                if (item.GetXp() != expValue) continue;
                if (index == 0)
                {
                     result += $"{item.GetId()}";
                }
                else
                {
                    result += $"{item.GetId()}\n";
                }
            }

            Debug.LogError(result);
            GUIUtility.systemCopyBuffer = result;
        }

        // [Button]
        // void BindNewSkinToys()
        // {
        //     foreach (var itemDefinition in listItemDefinition)
        //     {
        //         if (itemDefinition.GetFallingItemSetType() == FallingItemSetType.SCHOOL_SUPPLIES ||
        //             itemDefinition.GetFallingItemSetType() == FallingItemSetType.TOYS)
        //         {
        //             var prefab = GetPrefabById(itemDefinition.GetPrefab().name);
        //             if (prefab == null)
        //             {
        //                 Debug.LogError($"Missing prefab with id {itemDefinition.GetPrefab().name}");
        //                 continue;
        //             }
        //             itemDefinition.SetPrefab(prefab);
        //             EditorUtility.SetDirty(itemDefinition);
        //         }
        //     }
        //     
        //     AssetDatabase.SaveAssets();
        // }

        [Button]
        public void BakeIcon()
        {
            string folderPath = "Assets/Texture2D";
            string[] guids = AssetDatabase.FindAssets("", new[] { folderPath });
            Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                if (texture != null)
                {
                    Debug.Log("Found Texture2D (Sprite Atlas) Asset: " + assetPath);
                    // Load all sub-assets (sprites) associated with this Texture2D
                    Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);

                    foreach (var obj in sprites)
                    {
                        if (obj is Sprite sprite)
                        {
                            spriteDict.TryAdd(sprite.name, sprite);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to load Texture2D at path: " + assetPath);
                }
            }
            
            foreach (var item in listItemDefinition)
            {
                if (item.GetPrefab() == null || item.GetIcon() != null) continue;
                var spriteName = item.GetPrefab().name;
                if (!spriteDict.TryGetValue(spriteName, out var value)) continue;
                item.SetIcon(value);
                item.SetDirty();
            }
        }

        // [Button]
        // void RemoveNonFoodItem()
        // {
        //     for (int i = 0; i < listItemDefinition.Count; i++)
        //     {
        //         var currentItem = listItemDefinition[i];
        //         if (currentItem.GetFallingItemSetType() == FallingItemSetType.SCHOOL_SUPPLIES ||
        //             currentItem.GetFallingItemSetType() == FallingItemSetType.TOYS)
        //         {
        //             Debug.LogError($"Remove {currentItem.GetId()}");
        //             listItemDefinition.RemoveAt(i);
        //             i--;
        //         }
        //     }
        //     
        //     EditorUtility.SetDirty(this);
        //     AssetDatabase.SaveAssets();
        // }
        
        public GameObject GetPrefabById(string id)
        {
            string folderPath = prefabFolder;
    
            bool folderExists = AssetDatabase.IsValidFolder(folderPath);
            Debug.Log($"Folder exists: {folderExists}");
            Debug.Log($"Absolute path: {Path.GetFullPath(folderPath)}");
    
            string[] allAssets = AssetDatabase.FindAssets("", new[] {folderPath});
            Debug.Log($"Total assets found in folder: {allAssets.Length}");
    
            string[] assetGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] {folderPath});
            Debug.Log($"Prefabs found: {assetGUIDs.Length}");
            foreach (string guid in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (prefab.name.Equals(id))
                {
                    return prefab;
                }
            }
            return null;
        }

        [Button]
        void ConvertToNewIds()
        {
            for (int i = 0; i < listItemDefinition.Count; i++)
            {
                var item = listItemDefinition[i];
                item.SetId($"food_{i + 1}");
                EditorUtility.SetDirty(item);
            }
            
            for (int i = 0; i < listItemDefinitionShp.Count; i++)
            {
                var item = listItemDefinitionShp[i];
                item.SetId($"food_{i + 1}");
                EditorUtility.SetDirty(item);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [Button]
        void LogItemIds()
        {
            var csvString = "";
            csvString += "Old_ID,New_ID\n";
            for (int i = 0; i < listItemDefinition.Count; i++)
            {
                var item = listItemDefinition[i];
                csvString += $"{item.GetId()},food_{i + 1},\n";
            }

#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(csvString))
            {
                string path = "Assets/ItemIds.csv";
                File.WriteAllText(path, csvString);
                AssetDatabase.Refresh();
            }
#endif
        }

        [Button]
        void RenameAll()
        {
            foreach (var itemDef in listItemDefinitionShp)
            {
                if (itemDef == null) continue;
                
                var itemDefPath = AssetDatabase.GetAssetPath(itemDef);
                AssetDatabase.RenameAsset(itemDefPath, itemDef.GetId()+"_SHP");
                EditorUtility.SetDirty(itemDef);

                //if (itemDef.GetPrefab() != null)
                //{
                //    var prefabPath = AssetDatabase.GetAssetPath(itemDef.GetPrefab());
                //    AssetDatabase.RenameAsset(prefabPath, itemDef.GetId());
                //    EditorUtility.SetDirty(itemDef.GetPrefab());
                //}
                //if (itemDef.GetIcon() != null)
                //{
                //    var iconPath = AssetDatabase.GetAssetPath(itemDef.GetIcon());
                //    AssetDatabase.RenameAsset(iconPath, itemDef.GetId());
                //    EditorUtility.SetDirty(itemDef.GetIcon());   
                //}
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        [Button]
        void CustomEnableGPUInstancing()
        {
            foreach (var itemDef in listItemDefinitionShp)
            {
                if (itemDef == null) continue;
                
                var itemDefPath = AssetDatabase.GetAssetPath(itemDef);


                if (itemDef.GetPrefab() != null)
                {
                    var mats = itemDef.GetPrefab().GetComponentsInChildren<MeshRenderer>();
                    foreach (var mat in mats)
                    {
                        mat.sharedMaterial.enableInstancing = true;
                        EditorUtility.SetDirty(mat.sharedMaterial);
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Button]
        void MarkAllModalReadWirteAble()
        {
            Debug.LogError($"Start MarkAllModalReadWirteAble");
            foreach (var itemDef in listItemDefinitionShp)
            {
                if (itemDef == null) continue;

                var itemDefPath = AssetDatabase.GetAssetPath(itemDef);
                
                if (itemDef.GetPrefab() != null)
                {
                    //Make Mesh Convex
                    var convexs = itemDef.GetPrefab().GetComponentsInChildren<MeshCollider>();
                    foreach (var convex in convexs)
                    {
                        if (!convex.convex)
                        {
                            Debug.LogError("CvOn:" +itemDef.GetPrefab().name);
                            convex.convex = true;
                        }

                    }

                    var mats = itemDef.GetPrefab().GetComponentsInChildren<MeshFilter>();
                    foreach (var mf in mats)
                    {
                        if (mf.sharedMesh == null)
                            continue;

                        string path = AssetDatabase.GetAssetPath(mf.sharedMesh);
                        if (string.IsNullOrEmpty(path))
                            continue; // Not an asset, likely a procedural mesh

                        // Load the mesh importer
                        ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
                        if (importer != null && !importer.isReadable)
                        {
                            importer.isReadable = true;
                            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                            Debug.LogError($"Enabled Read/Write on mesh at: {path}");
                        }
                    }
                }
            }
            Debug.LogError($"End MarkAllModalReadWirteAble");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [Button]
        void DeleteAllDeactiveObject()
        {
            Debug.LogError($"Start MarkAllModalReadWirteAble");
            foreach (var itemDef in listItemDefinitionShp)
            {
                if (itemDef == null) continue;
                var itemDefPath = AssetDatabase.GetAssetPath(itemDef.GetPrefab());
                 if (string.IsNullOrEmpty(itemDefPath))
                {
                    Debug.LogError("Missing path for " + itemDef.GetId());
                    continue;
                }
                GameObject prefab = PrefabUtility.LoadPrefabContents(itemDefPath);
                if (prefab != null)
                {
                    var allChild = GetAllChildren(prefab.transform);
                    for (int i = allChild.Count-1; i >=0; i--)
                    {
                        if (!allChild[i].gameObject.activeSelf)
                        {
                            DestroyImmediate(allChild[i].gameObject,true);
                        }
                    }
                    PrefabUtility.SaveAsPrefabAsset(prefab, itemDefPath);
                    PrefabUtility.UnloadPrefabContents(prefab);
                }
            }
            Debug.LogError($"End MarkAllModalReadWirteAble");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        [Button]
        void ReModalItemForFloating(int min,int max)
        {
            Debug.LogError($"Start ReModalItemForFloating");
            for (int i = min; i < max; i++)
            {
                if(i >= listItemDefinitionShp.Count) break;
                var itemDef = listItemDefinitionShp[i];
                var itemDefPath = AssetDatabase.GetAssetPath(itemDef.GetPrefab());
                if (string.IsNullOrEmpty(itemDefPath))
                {
                    Debug.LogError("Missing path for " + itemDef.GetId());
                    continue;
                }
                GameObject prefab = PrefabUtility.LoadPrefabContents(itemDefPath);
                if (prefab != null)
                {
                    //Make Mesh Convex
                    var colliders = prefab.GetComponentsInChildren<Collider>();
                    var meshs = prefab.GetComponentsInChildren<MeshRenderer>();
                    if (colliders.Length > 1)
                    {
                        Debug.LogWarning(prefab.name + " Item have more one than collider");
                    } 
                    if (meshs.Length > 1)
                    {
                        Debug.LogWarning(prefab.name + " Item have more one than meshRenderer");
                    }

                    foreach (var ms in meshs)
                    {
                        ms.shadowCastingMode = ShadowCastingMode.Off;
                        ms.receiveShadows = false;
                    }
                    CheckOverlap(colliders, meshs, prefab.name);
                    //if (colliders[0].gameObject == meshs[0].gameObject)
                    //{
                    //    //Collider and mesh SameObject
                    //    GameObject child = new GameObject("collider");
                    //    child.transform.SetParent(prefab.transform);
                    //    CopyTransform(meshs[0].gameObject.transform, child.transform);
                    //    ColliderUtils.CopyCollider(colliders[0], child);
                    //    Object.DestroyImmediate(colliders[0], true);
                    //}
                    //if (meshs.Length ==1)
                    //{
                    //    //Only one mehsRender, can fake spring
                    //    if (meshs[0].gameObject.GetComponent<SpringOscillation>() == null)
                    //    {
                    //        meshs[0].gameObject.AddComponent<SpringOscillation>();
                    //    }
                    //}

                    if (prefab.GetComponentInChildren<SpringOscillation>() == null)
                    {
                        Debug.LogError("Mising SpringOsci in " + prefab.name);
                    }
                    PrefabUtility.SaveAsPrefabAsset(prefab, itemDefPath);
                    PrefabUtility.UnloadPrefabContents(prefab);
                }
            }
            Debug.LogError($"End ReModalItemForFloating");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        void CheckOverlap(Collider[] colliders, MeshRenderer[] meshs,string nameObj)
        {
            // Lấy danh sách GameObject từ mỗi mảng
            var colliderObjects = colliders.Select(c => c.gameObject);
            var meshObjects     = meshs.Select(m => m.gameObject);

            // Tìm giao (intersect)
            var duplicates = colliderObjects.Intersect(meshObjects);

            if (duplicates.Any())
            {
                    Debug.LogError($"Found overlapping GameObject: "+ nameObj);
            }
        }
        void CopyTransform(Transform source, Transform target)
        {
            target.position = source.position;
            target.rotation = source.rotation;
            target.localScale = source.localScale;
        }
#endif
#endregion
        public GameObject GetItemPrefab(string id)
        {
            var itemDef = listItemDefinition.Find(x => x.GetId() == id);
            return itemDef.GetPrefab();
        }
        public int GetItemExp(string id)
        {
            var itemDef = listItemDefinition.Find(x => x.GetId() == id);
            if (itemDef == null)
                return 0;
            return itemDef.GetXp();
        }
        List<Transform> GetAllChildren(Transform parent)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in parent)
            {
                children.Add(child);
                children.AddRange(GetAllChildren(child)); // Đệ quy lấy tiếp các con của child
            }
            return children;
        }
    }
    
}