using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _IEC.Features.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
// using OfficeOpenXml;
// using OfficeOpenXml.Drawing;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/LevelEditorConfig")]
    public class LevelEditorConfig : BaseConfig
    {
        public LevelListParameters gameLevelParameters;
        public GameplayItemPrefabConfig itemPrefabConfig;
        public List<ItemListLevel> itemListLevel;
        public List<ItemListLevel> itemListTargetLevel;
        public List<ItemData> itemDatas;
        public List<ItemData> itemTargetDatas;
        public List<ItemData> itemTargetDatasTrack;
        public List<Sprite> itemIconList;
        
#if UNITY_EDITOR 
        
        [Button]
        public void LogItemWithExp(int expValue)
        {
            var result = "";
            for (var index = 0; index < itemDatas.Count; index++)
            {
                var itemDefinitionsDict = GetItemDefinitionsDict();

                var item = itemDefinitionsDict[itemDatas[index].id];
                
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
        
        
        
        [Button]
        public void GetItemListLevel()
        {
            var str = "";
                
            var itemDefinitionsDict = GetItemDefinitionsDict();
            itemListLevel.Clear();
            itemListTargetLevel.Clear();
            for (int i = 0; i < gameLevelParameters.Levels.Length; i++)
            {
                str += $"LEVEL_{i}";
                itemListLevel.Add(new ItemListLevel { itemList = new List<ItemData>() });
                itemListTargetLevel.Add(new ItemListLevel { itemList = new List<ItemData>() });
                foreach (var itemData in gameLevelParameters.Levels[i].ItemList)
                {
                    if (!itemDefinitionsDict.ContainsKey(itemData.Id)) continue;
                    itemListLevel[i].itemList.Add(new ItemData
                    {
                        id = itemData.Id, prefab = itemDefinitionsDict[itemData.Id].GetPrefab(),
                        icon = itemDefinitionsDict[itemData.Id].GetIcon()
                    });
                }

                foreach (var recipeItem in gameLevelParameters.Levels[i].GetListRecipeItem())
                {
                    if (!itemDefinitionsDict.ContainsKey(recipeItem.Id)) continue;
                    itemListTargetLevel[i].itemList.Add(new ItemData
                    {
                        id = recipeItem.Id, prefab = itemDefinitionsDict[recipeItem.Id].GetPrefab(),
                        icon = itemDefinitionsDict[recipeItem.Id].GetIcon()
                    });
                }
            }

          
            Debug.LogError(str);
            GUIUtility.systemCopyBuffer = str;
        }

        [Button]
        public void GetUniqueItemList()
        {
            var str = "";
            HashSet<string> itemIds = new HashSet<string>();
            
            itemDatas.Clear();
            itemTargetDatas.Clear();
            
            var itemDefinitionsDict = GetItemDefinitionsDict();
            
            for (int i = 0; i < itemListLevel.Count; i++)
            {
                foreach (var itemData in itemListLevel[i].itemList)
                {
                    if (!itemDatas.Exists(x => x.id == itemData.id))
                    {
                        itemDatas.Add(new ItemData { id = itemData.id, prefab = itemData.prefab, icon = itemDefinitionsDict[itemData.id].GetIcon() });
                        itemIds.Add(itemData.prefab.name);
                    }
                }

                var listRecipeItem = gameLevelParameters.Levels[i].GetListRecipeItem();
                foreach (var recipeItem in listRecipeItem.Where(recipeItem => !itemTargetDatas.Exists(x => x.id == recipeItem.Id)))
                {
                    itemTargetDatas.Add(new ItemData { id = recipeItem.Id, prefab = itemDefinitionsDict[recipeItem.Id].GetPrefab(), icon = itemDefinitionsDict[recipeItem.Id].GetIcon()});
                }
                
            }
            foreach (var id in itemIds)
            {
                str += $"{id}\n";
            }
            Debug.LogError(str);
            GUIUtility.systemCopyBuffer = str;
        }

        [Button]
        public void AssignIconToItemDefinition()
        {
            var iconDict = new Dictionary<string, Sprite>();
            foreach (var icon in itemIconList)
            {
                iconDict[icon.name] = icon;
            }
            
            // List<string> unusedIconNames = new List<string>();
            // foreach (var item in itemTargetDatas)
            // {
            //     if (iconDict.ContainsKey(item.prefab.name)) continue;
            //     unusedIconNames.Add(item.prefab.name);
            // }
            // Debug.LogError(new JsonDotNetModule().ToJson(unusedIconNames));

            foreach (var itemDefinition in itemPrefabConfig.listItemDefinition)
            {
                var itemPrefab = itemDefinition.GetPrefab();
                if (itemPrefab == null) continue;
                var itemPrefabName = itemDefinition.GetPrefab().name;
                if (!iconDict.TryGetValue(itemPrefabName, out var sprite)) continue;
                itemDefinition.SetIcon(sprite);
                itemIconList.Remove(sprite);
                itemDefinition.SetDirty();
            }
        }

        [Button]
        public void DeActiveReceiveShadow()
        {
            var prefabList = itemDatas.Select(item => item.prefab).ToList();

            foreach (var child in prefabList.Select(prefab => prefab.GetComponentsInChildren<MeshRenderer>(true)).SelectMany(children => children))
            {
                child.receiveShadows = false;
            }
        }

        [Button]
        public void TraceItemDatasInItemsListTargetLevel()
        {
            //HashSet<int> levelAppearObjects = new HashSet<int>();
            var itemDatasId = itemDatas.Select(itemData => itemData.id).ToList();
            var itemTargetDatasId = itemTargetDatas.Select(itemData => itemData.id).ToList();
            var addedDatas = new List<string>();
            
            itemTargetDatasTrack.Clear();

            for (int i = 0; i < itemListTargetLevel.Count; i++)
            {
                foreach (var itemData in itemListTargetLevel[i].itemList)
                {
                    if (itemDatasId.Contains(itemData.id) && !itemTargetDatasId.Contains(itemData.id) && !addedDatas.Contains(itemData.id))
                    {
                        itemTargetDatasTrack.Add(itemData);
                        addedDatas.Add(itemData.id);
                    }

                }
            }

            //Debug.LogError(new JsonDotNetModule().ToJson(levelAppearObjects));
        }

        private Dictionary<string, ItemDefinition> GetItemDefinitionsDict()
        {
            var itemDefinitionsDict = new Dictionary<string, ItemDefinition>();

            foreach (var item in itemPrefabConfig.listItemDefinition.Where(item => item.GetPrefab() != null).Where(item => !itemDefinitionsDict.ContainsKey(item.GetId())))
            {
                itemDefinitionsDict.Add(item.GetId(), item);
            }
            return itemDefinitionsDict;
        }

        // [Button]
        // public void ExportCSV() => ExportItemDataToExcel($"Assets/Exports/{itemTargetDatas}", itemTargetDatas);
        //
        // public void ExportItemDataToExcel(string folderPath, List<ItemData> data)
        // {
        //     // Ensure the directory exists
        //     if (!Directory.Exists(folderPath))
        //     {
        //         Directory.CreateDirectory(folderPath);
        //     }
        //
        //     string filePath = Path.Combine(folderPath, "ItemData.xlsx");
        //     
        //     // Create and save Excel package
        //     using (ExcelPackage package = new ExcelPackage())
        //     {
        //         ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Item Data");
        //
        //         double rowHeight = 50;
        //         double iconCellWidth = worksheet.Column(3).Width = 15;
        //         
        //         // Add headers
        //         worksheet.Cells[1, 1].Value = "ID";
        //         worksheet.Cells[1, 2].Value = "Prefab";
        //         worksheet.Cells[1, 3].Value = "Icon";
        //
        //         // Loop through itemDatas and add rows
        //         for (int i = 0; i < data.Count; i++)
        //         {
        //             var item = data[i];
        //             int row = i + 2; // Starting from row 2 because row 1 is for headers
        //
        //             worksheet.Cells[row, 1].Value = item.id;
        //             worksheet.Cells[row, 2].Value = item.prefab != null ? item.prefab.name : "None";
        //
        //             worksheet.Row(row).Height = rowHeight;
        //
        //             if (item.icon != null)
        //             {
        //                 string iconFilePath = Path.Combine(folderPath, $"{item.id}_icon.png");
        //                 SaveSpriteAsPNG(item.icon, iconFilePath);
        //
        //                 // Load and add image to the worksheet
        //                 var picture = worksheet.Drawings.AddPicture($"{item.id}_icon", new FileInfo(iconFilePath));
        //                 
        //                 int pictureWidth = 50;
        //                 int pictureHeight = 50;
        //
        //                 // Calculate the cell size in pixels
        //                 int cellHeightPixels = (int)(rowHeight * 1.5); // Approx. conversion to pixels
        //                 int cellWidthPixels = (int)(iconCellWidth * 7);  // Approx. conversion to pixels
        //
        //                 // Calculate offsets to center the picture inside the cell
        //                 int offsetX = (cellWidthPixels - pictureWidth) / 2; // Horizontal centering
        //                 int offsetY = (cellHeightPixels - pictureHeight) / 2; // Vertical centering
        //                 
        //                 picture.SetPosition(row - 1, offsetY, 2, offsetX); // Position image in cell
        //                 picture.SetSize(50, 50); // Adjust size as needed
        //                 picture.EditAs = eEditAs.OneCell;
        //                 picture.LockAspectRatio = true;
        //             }
        //         }
        //         worksheet.Column(1).AutoFit();
        //         worksheet.Column(2).AutoFit();
        //
        //         // Save Excel file
        //         File.WriteAllBytes(filePath, package.GetAsByteArray());
        //     }
        //
        //     Debug.Log("Excel Export completed: " + filePath);
        // }

        private void SaveSpriteAsPNG(Sprite sprite, string filePath)
        {
            // Get the sprite's texture
            Texture2D originalTexture = sprite.texture;

            // Determine the sprite's rectangle within the texture atlas
            Rect spriteRect = sprite.textureRect;

            // Create a new Texture2D with the size of the sprite
            Texture2D newTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height);

            // Copy pixels from the original texture
            Color[] pixels = originalTexture.GetPixels(
                (int)spriteRect.x,
                (int)spriteRect.y,
                (int)spriteRect.width,
                (int)spriteRect.height);
            newTexture.SetPixels(pixels);

            // Apply changes
            newTexture.Apply();

            // Encode texture to PNG and save
            byte[] pngData = newTexture.EncodeToPNG();
            File.WriteAllBytes(filePath, pngData);

            // Clean up the temporary texture
            DestroyImmediate(newTexture);
        }
#endif
    }

    [Serializable]
    public class ItemData
    {
        public string id;
        public GameObject prefab;
        [PreviewField(75)]
        public Sprite icon;
    }

    [Serializable]
    public class ItemListLevel
    {
        public List<ItemData> itemList;
    }
}
