using System;
using _IEC.Features.ExportLevel.OriginalLevel;
using _IEC.Features.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IEC
{
    public abstract class SpawnArea
    {
        public virtual SpawnAreaShapes SpawnShape => SpawnAreaShapes.None;
        public SpawnTypes spawnType;
        public Vector3 spawnPos = Vector3.zero;
        public float spawnRot = 0f;
        public bool startAsKinematic = true;

        public abstract GameObject CreatePreview(GameplayItemPrefabConfig itemPrefabConfig, string folderPathItemInGame,
            string itemId);

        protected virtual void CreateItems(GameplayItemPrefabConfig itemPrefabConfig, GameObject prefab, Vector3 pos,
            GameObject previewGameObject, string itemId, int verticalCount)
        {
            var newItem = Object.Instantiate(prefab, previewGameObject.transform);

            var itemDefinition = itemPrefabConfig.ListItemDefinition.Find(x => x.GetId() == itemId);
            var itemComponent = newItem.GetComponent<BaseItem>();
            if (itemComponent == null)
            {
                itemComponent = newItem.AddComponent<BaseItem>();
            }

            var itemData = new _IEC.Features.Gameplay.ItemData();
            itemData.itemId = itemId;
            itemData.isStartKinematic = startAsKinematic;

            itemComponent.LoadData(itemData, itemDefinition);
            newItem.transform.position = pos;
            newItem.transform.eulerAngles = new Vector3(0, spawnRot, 0);
                        
            CreateStackedItems(itemPrefabConfig, prefab, itemComponent, itemId, verticalCount - 1);
        }
        
        protected virtual void CreateStackedItems(GameplayItemPrefabConfig itemPrefabConfig, GameObject prefab, BaseItem firstItem, 
            string itemId, int verticalCount)
        {
            if (verticalCount <= 0)
                return;
            
            for (int i = 0; i < verticalCount; i++)
            {
                var newItem = Object.Instantiate(prefab, firstItem.transform.parent);
                
                var itemDefinition = itemPrefabConfig.ListItemDefinition.Find(x => x.GetId() == itemId);
                var itemComponent = newItem.GetComponent<BaseItem>();
                if (itemComponent == null)
                {
                    itemComponent = newItem.AddComponent<BaseItem>();
                }
                
                var itemData = new _IEC.Features.Gameplay.ItemData();
                itemData.itemId = itemId;
                itemComponent.LoadData(itemData, itemDefinition);
                newItem.transform.position = firstItem.transform.position;
                newItem.transform.eulerAngles = new Vector3(0, spawnRot, 0);

                var renderer = itemComponent.MyRenderer;
                if (renderer != null)
                {
                    var itemHeight = renderer.bounds.size.y * newItem.transform.localScale.y * (i + 1);
                    var pivotOffset = (renderer.bounds.center.y + renderer.transform.localPosition.y);
                    newItem.transform.position += Vector3.up * (itemHeight);
                    // - pivotOffset);
                }
            }
        }
    }

    [Serializable]
    public class RectangeSpawnArea : SpawnArea
    {
        public override SpawnAreaShapes SpawnShape => SpawnAreaShapes.Rectangle;

        public int width = 10;
        public int length = 10;
        public float spacing = 1.5f;
        public int verticalCount = 1;

        public override GameObject CreatePreview(GameplayItemPrefabConfig itemPrefabConfig, string folderPathItemInGame,
            string itemId)
        {
            GameObject previewGameObject;
            var prefab = Utilities.FindAssetInFolder<GameObject>(folderPathItemInGame, itemId);
            if (prefab == null)
            {
                Debug.LogError($"Missing itemId {itemId}");
                return null;
            }

            previewGameObject = new GameObject("Preview");
            previewGameObject.transform.position = spawnPos;

            var sizeWidth = width * spacing;
            var sizeLength = length * spacing;
            var bottomLeftPos = previewGameObject.transform.position + Vector3.left * (sizeWidth / 2f) +
                                Vector3.back * (sizeLength / 2f);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    if (spawnType == SpawnTypes.Border)
                    {
                        if (x != 0 && x != width - 1 && y != 0 && y != length - 1)
                        {
                            continue;
                        }
                    }

                    var pos = bottomLeftPos + Vector3.right * (spacing * x) + Vector3.forward * (spacing * y);
                    CreateItems(itemPrefabConfig, prefab, pos, previewGameObject, itemId, verticalCount);
                }
            }

            return previewGameObject;
        }
    }

    [Serializable]
    public class CircleSpawnArea : SpawnArea
    {
        public override SpawnAreaShapes SpawnShape => SpawnAreaShapes.Circle;

        public int radius = 3;
        public float spacing = 1.5f;
        public int verticalCount = 1;

        public override GameObject CreatePreview(GameplayItemPrefabConfig itemPrefabConfig, string folderPathItemInGame,
            string itemId)
        {
            GameObject previewGameObject;
            var prefab = Utilities.FindAssetInFolder<GameObject>(folderPathItemInGame, itemId);
            if (prefab == null)
            {
                Debug.LogError($"Missing itemId {itemId}");
                return null;
            }

            previewGameObject = new GameObject("Preview");
            previewGameObject.transform.position = spawnPos;

            if (spawnType == SpawnTypes.Border)
            {
                float circumference = 2 * Mathf.PI * radius * spacing;
                int itemCount = Mathf.FloorToInt(circumference / spacing);

                for (int i = 0; i < itemCount; i++)
                {
                    float angle = i * (360f / itemCount);
                    var pos = spawnPos + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius * spacing, 0,
                        Mathf.Sin(angle * Mathf.Deg2Rad) * radius * spacing);
                    CreateItems(itemPrefabConfig, prefab, pos, previewGameObject, itemId, verticalCount);
                }
            }
            else
            {
                for (int r = 0; r <= radius; r++)
                {
                    int itemCount = Mathf.FloorToInt(2 * Mathf.PI * r * spacing / spacing);
                    for (int i = 0; i < itemCount; i++)
                    {
                        float angle = i * (360f / itemCount);
                        var pos = spawnPos + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * r * spacing, 0,
                            Mathf.Sin(angle * Mathf.Deg2Rad) * r * spacing);
                        
                        CreateItems(itemPrefabConfig, prefab, pos, previewGameObject, itemId, verticalCount);
                    }
                }
            }

            return previewGameObject;
        }
    }

    [Serializable]
    public class TriangleSpawnArea : SpawnArea
    {
        public override SpawnAreaShapes SpawnShape => SpawnAreaShapes.Triangle;
        public int sideLength = 5;
        public float spacing = 1.5f;
        public int verticalCount = 1;

        public override GameObject CreatePreview(GameplayItemPrefabConfig itemPrefabConfig, string folderPathItemInGame,
            string itemId)
        {
            GameObject previewGameObject;
            var prefab = Utilities.FindAssetInFolder<GameObject>(folderPathItemInGame, itemId);
            if (prefab == null)
            {
                Debug.LogError($"Missing itemId {itemId}");
                return null;
            }

            previewGameObject = new GameObject("Preview");
            previewGameObject.transform.position = spawnPos;

            if (spawnType == SpawnTypes.Border)
            {
                Vector3[] vertices = new Vector3[]
                {
                    spawnPos + new Vector3(-sideLength / 2f * spacing, 0, -Mathf.Sqrt(3) / 6f * sideLength * spacing),
                    spawnPos + new Vector3(sideLength / 2f * spacing, 0, -Mathf.Sqrt(3) / 6f * sideLength * spacing),
                    spawnPos + new Vector3(0, 0, Mathf.Sqrt(3) / 3f * sideLength * spacing)
                };

                for (int i = 0; i < 3; i++)
                {
                    Vector3 start = vertices[i];
                    Vector3 end = vertices[(i + 1) % 3];
                    int itemCount = sideLength - 1;

                    for (int j = 0; j <= itemCount; j++)
                    {
                        Vector3 pos = Vector3.Lerp(start, end, (float)j / itemCount);
                        CreateItems(itemPrefabConfig, prefab, pos, previewGameObject, itemId, verticalCount);
                    }
                }
            }
            else
            {
                for (int row = 0; row <= sideLength - 1; row++)
                {
                    for (int col = 0; col <= row; col++)
                    {
                        float x = (col - row / 2f) * spacing;
                        float z = (Mathf.Sqrt(3) / 2f * -row) * spacing;
                        Vector3 pos = spawnPos + new Vector3(x, 0, z + Mathf.Sqrt(3) / 6f * sideLength * spacing);
                        CreateItems(itemPrefabConfig, prefab, pos, previewGameObject, itemId, verticalCount);
                    }
                }
            }

            return previewGameObject;
        }
    }

    [Serializable]
    public class HexagonSpawnArea : SpawnArea
    {
        public override SpawnAreaShapes SpawnShape => SpawnAreaShapes.Hexagon;

        public int radius = 3;
        public float spacing = 1.5f;
        public int verticalCount = 1;

        public override GameObject CreatePreview(GameplayItemPrefabConfig itemPrefabConfig, string folderPathItemInGame, string itemId)
        {
            var prefab = Utilities.FindAssetInFolder<GameObject>(folderPathItemInGame, itemId);
            if (prefab == null)
            {
                Debug.LogError($"Missing itemId {itemId}");
                return null;
            }

            var previewGameObject = new GameObject("Preview");
            previewGameObject.transform.position = spawnPos;

            if (spawnType == SpawnTypes.Border)
            {
                for (int q = -radius; q <= radius; q++)
                {
                    int r1 = Mathf.Max(-radius, -q - radius);
                    int r2 = Mathf.Min(radius, -q + radius);
                    for (int r = r1; r <= r2; r++)
                    {
                        if (Mathf.Abs(q) == radius || Mathf.Abs(r) == radius || Mathf.Abs(q + r) == radius)
                        {
                            Vector3 pos = HexToCartesian(q, r, spacing) + spawnPos;
                            CreateItems(itemPrefabConfig, prefab, pos, previewGameObject, itemId, verticalCount);
                        }
                    }
                }
            }
            else
            {
                for (int q = -radius; q <= radius; q++)
                {
                    int r1 = Mathf.Max(-radius, -q - radius);
                    int r2 = Mathf.Min(radius, -q + radius);
                    for (int r = r1; r <= r2; r++)
                    {
                        Vector3 pos = HexToCartesian(q, r, spacing) + spawnPos;
                        CreateItems(itemPrefabConfig, prefab, pos, previewGameObject, itemId, verticalCount);
                    }
                }
            }

            return previewGameObject;
        }

        private Vector3 HexToCartesian(int q, int r, float spacing)
        {
            float x = spacing * (3.0f / 2.0f * q);
            float z = spacing * (Mathf.Sqrt(3.0f) * (r + q / 2.0f));
            return new Vector3(x, 0, z);
        }
    }

    public enum SpawnAreaShapes
    {
        None,
        Rectangle,
        Circle,
        Triangle,
        Hexagon
    }

    public enum SpawnTypes
    {
        Full,
        Border
    }
}