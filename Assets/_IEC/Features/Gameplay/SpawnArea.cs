using System;
using System.Collections.Generic;
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
        public Vector3 spawnPos;
        public float spawnRot = 0f;
        public bool startAsKinematic = true;
        public bool isFixedSpawnAmount = false;
        public int fixedSpawnAmount;

        public abstract GameObject CreatePreview(GameplayItemPrefabConfig itemPrefabConfig, string folderPathItemInGame,
            string itemId);

        protected virtual void CreateItems(GameplayItemPrefabConfig itemPrefabConfig, GameObject prefab, Vector3 pos,
            GameObject previewGameObject, string itemId, int verticalCount, float rotOffset = 0)
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
            newItem.transform.eulerAngles = new Vector3(0, spawnRot + rotOffset, 0);
                        
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
            //var prefab = Utilities.FindAssetInFolder<GameObject>(folderPathItemInGame, itemId);
            var prefab = itemPrefabConfig.ListItemDefinition.Find(x => x.GetId() == itemId).GetPrefab();
            if (prefab == null)
            {
                Debug.LogError($"Missing itemId {itemId}");
                return null;
            }

            previewGameObject = new GameObject("Preview");
            previewGameObject.transform.position = spawnPos;
            
            if (isFixedSpawnAmount)
            {
                Bounds bounds = GroundExtend.GetRendererBounds(prefab);
                float maxSize = Mathf.Max(bounds.size.x, bounds.size.z);
                spacing = maxSize * 1.1f; // Prevent overlap
                
                (width, length) = FindBestAreaFit(fixedSpawnAmount, spawnType);
            }

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
        
        private (int width, int length) FindBestAreaFit(int targetCount, SpawnTypes spawnType)
        {
            int bestW = 1, bestL = targetCount;
            int minDiff = targetCount;

            if (spawnType == SpawnTypes.Border)
            {
                // Border spawn type: Adjust width and length to fit the perimeter with fixed amount of objects

                // For border spawn, we need at least 4 objects to form a rectangle
                if (targetCount < 4)
                {
                    // If targetCount is less than 4, we place objects along one row or column
                    return (1, targetCount);
                }

                // Try to find width and length to fit the border (i.e., perimeter placement)
                for (int w = 1; w <= Mathf.Sqrt(targetCount); w++)
                {
                    if (targetCount % w == 0)
                    {
                        int l = targetCount / w;

                        // We need to account for perimeter positions:
                        // Total positions on the border for a w x l rectangle is: 2 * (w + l) - 4 (to avoid double-counting corners)
                        int perimeterCount = 2 * (w + l) - 4;

                        if (perimeterCount >= targetCount)
                        {
                            int diff = Mathf.Abs(w - l);
                            if (diff < minDiff)
                            {
                                minDiff = diff;
                                bestW = w;
                                bestL = l;
                            }
                        }
                    }
                }
            }
            else
            {
                // Full spawn type: Adjust width and length to fill the entire area
                // For full spawn, try to find width and length to cover the area evenly
                for (int w = 1; w <= Mathf.Sqrt(targetCount); w++)
                {
                    if (targetCount % w == 0)
                    {
                        int l = targetCount / w;

                        // We just want the best approximation for width and length that fits the target count
                        int diff = Mathf.Abs(w - l);
                        if (diff < minDiff)
                        {
                            minDiff = diff;
                            bestW = w;
                            bestL = l;
                        }
                    }
                }
            }

            return (bestW, bestL);
        }
    }

    [Serializable]
    public class CircleSpawnArea : SpawnArea
    {
        public override SpawnAreaShapes SpawnShape => SpawnAreaShapes.Circle;

        public float radius = 3;
        public float spacing = 1.5f;
        public int verticalCount = 1;

        public override GameObject CreatePreview(GameplayItemPrefabConfig itemPrefabConfig, string folderPathItemInGame,
            string itemId)
        {
            GameObject previewGameObject;
            //var prefab = Utilities.FindAssetInFolder<GameObject>(folderPathItemInGame, itemId);
            var prefab = itemPrefabConfig.ListItemDefinition.Find(x => x.GetId() == itemId).GetPrefab();
            if (prefab == null)
            {
                Debug.LogError($"Missing itemId {itemId}");
                return null;
            }

            previewGameObject = new GameObject("Preview");
            previewGameObject.transform.position = spawnPos;

            // Bounds bounds = GroundExtend.GetRendererBounds(prefab);
            // float diameter = Mathf.Max(bounds.size.x, bounds.size.z);
            // spacing = diameter * 1.1f;
            //
            // if (spawnType == SpawnTypes.Border)
            // {
            //     float circumference = fixedSpawnAmount * spacing;
            //     radius = Mathf.CeilToInt(circumference / (2 * Mathf.PI * spacing));
            // }
            // else
            // {
            //     // Full spawn: estimate how many rings we need
            //     int rings = 0;
            //     int count = 0;
            //     while (count < fixedSpawnAmount)
            //     {
            //         rings++;
            //         count += Mathf.Max(1, Mathf.FloorToInt(2 * Mathf.PI * rings)); // at least 1 per ring
            //     }
            //     radius = rings;
            // }

            if (isFixedSpawnAmount)
            {
                PrepareFixedCircleSpawn(prefab);
            }

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

        private void PrepareFixedCircleSpawn(GameObject prefab)
        {
            Bounds bounds = GroundExtend.GetRendererBounds(prefab);
            float minSpacing = Mathf.Max(bounds.size.x, bounds.size.z) * 1.1f;

            spacing = minSpacing;

            if (spawnType == SpawnTypes.Border)
            {
                radius = fixedSpawnAmount / (2f * Mathf.PI);
                float testCircumference = 2f * Mathf.PI * radius;
                if (Mathf.FloorToInt(testCircumference) < fixedSpawnAmount)
                    radius += 0.01f;
            }
            else // Full spawn type
            {
                float bestRadius = 1f;
                int bestCount = 0;
                float maxRadius = 50f;
                float step = 0.1f;

                for (float r = 0.5f; r <= maxRadius; r += step)
                {
                    int totalCount = 1; // center point

                    for (int ring = 1; ring <= Mathf.FloorToInt(r); ring++)
                    {
                        int ringCount = Mathf.FloorToInt(2 * Mathf.PI * ring);
                        totalCount += ringCount;
                    }

                    if (Mathf.Abs(totalCount - fixedSpawnAmount) < Mathf.Abs(bestCount - fixedSpawnAmount))
                    {
                        bestRadius = r;
                        bestCount = totalCount;

                        // early exit if exact match
                        if (totalCount == fixedSpawnAmount)
                            break;
                    }
                }

                radius = Mathf.CeilToInt(bestRadius);
            }
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
            //var prefab = Utilities.FindAssetInFolder<GameObject>(folderPathItemInGame, itemId);
            var prefab = itemPrefabConfig.ListItemDefinition.Find(x => x.GetId() == itemId).GetPrefab();
            if (prefab == null)
            {
                Debug.LogError($"Missing itemId {itemId}");
                return null;
            }

            previewGameObject = new GameObject("Preview");
            previewGameObject.transform.position = spawnPos;
            
            if (isFixedSpawnAmount)
            {
                PrepareFixedTriangleSpawn(prefab);
            }

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
        
        private void PrepareFixedTriangleSpawn(GameObject prefab)
        {
            // Calculate safe minimum spacing based on prefab size
            Bounds bounds = GroundExtend.GetRendererBounds(prefab);
            float minSpacing = Mathf.Max(bounds.size.x, bounds.size.z) * 1.1f;
            spacing = minSpacing;

            int targetCount = fixedSpawnAmount;
            int bestSideLength = 1;

            if (spawnType == SpawnTypes.Full)
            {
                // For full spawn: total count = (n * (n + 1)) / 2
                while (true)
                {
                    int count = (bestSideLength * (bestSideLength + 1)) / 2;
                    if (count >= targetCount)
                        break;

                    bestSideLength++;
                }
            }
            else if (spawnType == SpawnTypes.Border)
            {
                // For border: 3 sides, each has (n - 1) segments, with corners shared
                // So total = 3 * (n - 1)
                while (true)
                {
                    int count = 3 * (bestSideLength - 1);
                    if (count >= targetCount)
                        break;

                    bestSideLength++;
                }
            }

            sideLength = bestSideLength;
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
            //var prefab = Utilities.FindAssetInFolder<GameObject>(folderPathItemInGame, itemId);
            var prefab = itemPrefabConfig.ListItemDefinition.Find(x => x.GetId() == itemId).GetPrefab();
            if (prefab == null)
            {
                Debug.LogError($"Missing itemId {itemId}");
                return null;
            }

            var previewGameObject = new GameObject("Preview");
            previewGameObject.transform.position = spawnPos;
            
            if (isFixedSpawnAmount)
            {
                PrepareFixedHexagonSpawn(prefab);
            }
            
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
        
        private void PrepareFixedHexagonSpawn(GameObject prefab)
        {
            // Get safe minimum spacing to avoid overlap
            Bounds bounds = GroundExtend.GetRendererBounds(prefab);
            float minSpacing = Mathf.Max(bounds.size.x, bounds.size.z) * 1.1f;
            spacing = minSpacing;

            int target = fixedSpawnAmount;
            int r = 0;

            if (spawnType == SpawnTypes.Full)
            {
                while (true)
                {
                    int count = 1 + 3 * r * (r + 1);
                    if (count >= target)
                        break;
                    r++;
                }
            }
            else if (spawnType == SpawnTypes.Border)
            {
                while (true)
                {
                    int count = r == 0 ? 1 : 6 * r;
                    if (count >= target)
                        break;
                    r++;
                }
            }

            radius = r;
        }
    }
    [Serializable]
    public class PyramidRectancleSpawnArea : SpawnArea
    {
        public override SpawnAreaShapes SpawnShape => SpawnAreaShapes.Pyramid;
        public int width = 10;
        public int length = 10;
        public float spacing = 1.5f;
        public int layerSizeReduce;

        public override GameObject CreatePreview(GameplayItemPrefabConfig itemPrefabConfig, string folderPathItemInGame, string itemId)
        {
            GameObject previewGameObject;
            //GameObject prefab = Utilities.FindAssetInFolder<GameObject>(folderPathItemInGame, itemId);
            var prefab = itemPrefabConfig.ListItemDefinition.Find(x => x.GetId() == itemId).GetPrefab();
            if (prefab == null)
            {
                Debug.LogError($"Missing itemId {itemId}");
                return null;
            }


            previewGameObject = new GameObject("Preview");
            previewGameObject.transform.position = spawnPos;
            var itemHeight = 1f;
            var renderer  = prefab.GetComponentInChildren<MeshRenderer>();;
            if (renderer != null)
            {
                itemHeight = renderer.bounds.size.y;
            }

            int numberlayer = width / layerSizeReduce;
            for (int i = 0; i < numberlayer; i++)
            {
                var sizeWidth = (width-i*layerSizeReduce) * spacing;
                var sizeLength = (length-i*layerSizeReduce) * spacing;
                var bottomLeftPos = previewGameObject.transform.position + Vector3.left * (sizeWidth / 2f) + Vector3.back * (sizeLength / 2f);
                for (int x = 0; x < width-i*layerSizeReduce; x++)
                {
                    for (int y = 0; y < length-i*layerSizeReduce; y++)
                    {
                        if (spawnType == SpawnTypes.Border)
                        {
                            if (x != 0 && x != width-i*layerSizeReduce - 1 && y != 0 && y != length-i*layerSizeReduce - 1)
                            {
                                //Only draw object border
                                continue;
                            }
                        }

                        var pos = bottomLeftPos + Vector3.right * (spacing * x) + Vector3.forward * (spacing * y) + Vector3.up*i *itemHeight;
                        CreateItems(itemPrefabConfig, prefab, pos, previewGameObject, itemId, 0);
                    }
                }
            }


            return previewGameObject;
        }
    }
    public class PyramidCircleSpawnArea : SpawnArea
    {
       public override SpawnAreaShapes SpawnShape => SpawnAreaShapes.Circle;

        public int radius = 3;
        public float spacing = 1.5f;
        public float layerSizeReduce;
        public override GameObject CreatePreview(GameplayItemPrefabConfig itemPrefabConfig, string folderPathItemInGame,
            string itemId)
        {
            GameObject previewGameObject;
            //var prefab = Utilities.FindAssetInFolder<GameObject>(folderPathItemInGame, itemId);
            var prefab = itemPrefabConfig.ListItemDefinition.Find(x => x.GetId() == itemId).GetPrefab();
            if (prefab == null)
            {
                Debug.LogError($"Missing itemId {itemId}");
                return null;
            }

            previewGameObject = new GameObject("Preview");
            previewGameObject.transform.position = spawnPos;

            previewGameObject = new GameObject("Preview");
            previewGameObject.transform.position = spawnPos;
            var itemHeight = 1f;
            var renderer  = prefab.GetComponentInChildren<MeshRenderer>();;
            if (renderer != null)
            {
                itemHeight = renderer.bounds.size.y;
            }

            int numberlayer = (int)(radius / layerSizeReduce);
            if (spawnType == SpawnTypes.Border)
            {
                for (int i = 0; i < numberlayer; i++)
                {
                    float circumference = 2 * Mathf.PI * (radius - layerSizeReduce * i) * spacing;
                    int itemCount = Mathf.FloorToInt(circumference / spacing);

                    for (int k = 0; k < itemCount; k++)
                    {
                        float angle = k * (360f / itemCount);
                        var pos = spawnPos + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * (radius - layerSizeReduce * i) * spacing, 0 +i*itemHeight,
                            Mathf.Sin(angle * Mathf.Deg2Rad) * (radius - layerSizeReduce * i) * spacing);
                        CreateItems(itemPrefabConfig, prefab, pos, previewGameObject, itemId, 1);
                    }
                }
            }
            else
            {
                for (int i = 0; i < numberlayer; i++)
                {
                    for (int r = 0; r <= radius; r++)
                    {
                        int itemCount = Mathf.FloorToInt(2 * Mathf.PI * (r - layerSizeReduce*i) * (spacing ) / spacing  );
                        for (int k = 0; k < itemCount; k++)
                        {
                            float angle = k * (360f / itemCount);
                            var pos = spawnPos + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * (r - layerSizeReduce*i) * spacing, 0 +i*itemHeight, Mathf.Sin(angle * Mathf.Deg2Rad) * (r - layerSizeReduce*i) * spacing);

                            CreateItems(itemPrefabConfig, prefab, pos, previewGameObject, itemId, 1);
                        }
                    }
                }
            }

            return previewGameObject;
        }
    }

    [Serializable]
    public class PillarSpawnArea : SpawnArea
    {

        public const string pillarID = "food_190";
        public const string pillarTopID = "food_191";

        public float rotationEachLayer = -20f;
        public float spacingEachLayer = 0.6f;
        public float objSpacingFromPillar = 1.5f;
         
        public int verticalCount = 1;
        
        public override SpawnAreaShapes SpawnShape => SpawnAreaShapes.Pillar;
        public override GameObject CreatePreview(GameplayItemPrefabConfig itemPrefabConfig, string folderPathItemInGame, string itemId)
        {
            GameObject previewGameObject;
            
            //var prefab = Utilities.FindAssetInFolder<GameObject>(folderPathItemInGame, itemId);
            var prefab = itemPrefabConfig.ListItemDefinition.Find(x => x.GetId() == itemId).GetPrefab();
            if (prefab == null)
            {
                Debug.LogError($"Missing itemId {itemId}");
                return null;
            }
            
            var pillarPrefab = Utilities.FindAssetInFolder<GameObject>(folderPathItemInGame, pillarID);
            if (pillarPrefab == null)
            {
                Debug.LogError($"Missing itemId {itemId}");
                return null;
            }
            
            var pillarTopPrefab = Utilities.FindAssetInFolder<GameObject>(folderPathItemInGame, pillarTopID);
            if (pillarTopPrefab == null)
            {
                Debug.LogError($"Missing itemId {itemId}");
                return null;
            }
            
            previewGameObject = new GameObject("Preview");
            previewGameObject.transform.position = spawnPos;
            
            
            for (int i = 0; i < verticalCount; i++)
            {
                CreateItems(itemPrefabConfig,
                    i == verticalCount - 1 ? pillarTopPrefab : pillarPrefab, 
                    Vector3.zero + Vector3.up * spacingEachLayer * i, previewGameObject, pillarID, 1, rotationEachLayer * i);
            }
            
            CreateItems(itemPrefabConfig,
                prefab, 
                Vector3.zero + Vector3.up * (spacingEachLayer * verticalCount + objSpacingFromPillar), previewGameObject, itemId, 1);


            return previewGameObject;
        }
    }
    
    public enum SpawnAreaShapes
    {
        None,
        Rectangle,
        Circle,
        Triangle,
        Hexagon,
        Pyramid,
        Pillar
    }

    public enum SpawnTypes
    {
        Full,
        Border,
        Pyramid
    }
}