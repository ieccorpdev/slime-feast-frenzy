using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;

namespace IEC
{
    public class ItemExportConfig
    {
        public string itemId;
        public bool isApproved;
    }

    public class LevelScreenshot : MonoBehaviour
    {
        public LevelEditorConfig levelEditorConfig;
        public List<GameObject> levelObjects;

        public List<GameObject> spawnedObjects;
        public string url;
        
        public Vector3 startPosition = Vector3.zero; // Starting position for arranging objects
        public Vector3 offset = new Vector3(2, 0, 2); // Offset between objects in the grid
        public int columns = 3; // Number of columns for the grid

        private const string FOLDER_PATH_ITEM_IN_GAME = "_IECProject/Items";

        
        [Button]
        public void GetListLevelObjects()
        {
            levelObjects.Clear();
            foreach (var itemData in levelEditorConfig.itemTargetDatas)
            {
                levelObjects.Add(itemData.prefab);
            }
        }

        [Button]
        public void SpawnObjects(bool isApproved)
        {
            StartCoroutine(IEReplaceSkin(isApproved));
        }
        
        IEnumerator IEReplaceSkin(bool isApproved)
        {
            string csvUrl = url.Replace("/edit?gid=", "/export?format=csv&gid=");
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
                ProcessCsvData(csvText, isApproved);
            }
        }
        
        void ProcessCsvData(string csvText, bool isApproved)
        {
            // var idMappings = new List<ItemExportConfig>();
            // var lines = csvText.Split("\n");
            // for (int i = 1; i < lines.Length; i++)
            // {
            //     var currentLine = lines[i].Trim();
            //     var column = currentLine.Split(',');
            //
            //     if (column.Length >= 2)
            //     {
            //         var idMapping = new ItemExportConfig();
            //         idMapping.itemId = column[0];
            //         idMapping.isApproved = column[1] == "TRUE";
            //         idMappings.Add(idMapping);
            //     }
            // }
            //
            // var listItem = new List<GameObject>();
            // foreach (var config in idMappings)
            // {
            //     if (isApproved)
            //     {
            //         if (!config.isApproved) continue;
            //     }
            //     else
            //     {
            //         if (config.isApproved) continue;
            //     }
            //     var originalPrefab = Utilities.FindAssetInFolder<GameObject>(FOLDER_PATH_ITEM_IN_GAME, config.itemId);
            //     if (originalPrefab == null)
            //     {
            //         Debug.LogError($"Missing item with name {config.itemId}");
            //         continue;
            //     }
            //     listItem.Add(originalPrefab);
            // }
            //
            // for (int i = 0; i < listItem.Count; i++)
            // {
            //     // Calculate the position for each object in the grid
            //     int row = i / columns;
            //     int column = i % columns;
            //
            //     Vector3 position = startPosition + new Vector3(column * offset.x, 0, row * offset.z);
            //
            //     // Instantiate the object at the calculated position
            //     var spawnObj = Instantiate(listItem[i], position, Quaternion.identity);
            //     
            //     // Create a list of inactive children first
            //     List<GameObject> inactiveChildren = new List<GameObject>();
            //
            //     // Collect inactive children before removal
            //     foreach (Transform child in spawnObj.transform)
            //     {
            //         if (!child.gameObject.activeInHierarchy)
            //         {
            //             inactiveChildren.Add(child.gameObject);
            //         }
            //     }
            //
            //     // Destroy collected inactive children
            //     foreach (GameObject child in inactiveChildren)
            //     {
            //         DestroyImmediate(child);
            //     }
            //     
            //     spawnedObjects.Add(spawnObj);
            // }
        }
        
        [Button]
        public void ArrangeObjects()
        {
            foreach (var obj in spawnedObjects)
            {
                DestroyImmediate(obj);
            }
            spawnedObjects.Clear();
            
            for (int i = 0; i < levelObjects.Count; i++)
            {
                // Calculate the position for each object in the grid
                int row = i / columns;
                int column = i % columns;

                Vector3 position = startPosition + new Vector3(column * offset.x, 0, row * offset.z);

                // Instantiate the object at the calculated position
                var spawnObj = Instantiate(levelObjects[i], position, Quaternion.identity);
                spawnedObjects.Add(spawnObj);
            }
        }
    }
}