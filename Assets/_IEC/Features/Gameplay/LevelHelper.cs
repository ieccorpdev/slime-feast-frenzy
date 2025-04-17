using System.Collections.Generic;
using System.Linq;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public static class LevelHelper
    {
        private static GameConfig Config => Pool.Get<GameConfig>();
        private static HashSet<string> itemIds = new HashSet<string>();

        public static string GetAllItemsId()
        {
            string result = "";
            foreach (var id in itemIds)
            {
                result += $"{id}\n";
            }

            return result;
        }

        private static PlayerSecret PlayerData => Pool.Get<PlayerSecret>();

        public static void SpawnMap(LevelDataSO level, Transform levelObjectHolder, ThemeConfig config = null)
        {
            SpawnMap(level.GetMapData(), levelObjectHolder, config);
        }
        
        public static void SpawnMap(LevelConfigModel level, Transform levelObjectHolder, ThemeConfig config = null)
        {
            SpawnMap(level.GetMapData(), levelObjectHolder, config);
        }

        public static void SpawnMap(PrefabMapData prefabMapData, Transform levelObjectHolder)
        {
            var mapPrefab = prefabMapData.mapPrefab;
            var map = mapPrefab.Spawn(levelObjectHolder);
            map.transform.position = new Vector3(prefabMapData.pos.x, prefabMapData.pos.y, prefabMapData.pos.z);
            map.transform.eulerAngles = new Vector3(prefabMapData.rot.x, prefabMapData.rot.y, prefabMapData.rot.z);
            map.transform.localScale = new Vector3(prefabMapData.scale.x, prefabMapData.scale.y, prefabMapData.scale.z);
        }

        public static void SpawnMap(MapData mapData, Transform levelObjectHolder, ThemeConfig config = null)
        {
            var themeConfig = config == null ? Config.themeConfig : config;
            var listTileObjects = new List<GroundElement>();
            foreach (var tileMapData in mapData.listMapTileData)
            {
                var prefab = themeConfig.GetLevelMapTile(tileMapData.id, mapData.groupSkinId);
                if (prefab == null) continue;
                var tileObject = prefab.Spawn();
                var tileTransform = tileObject.transform;
                tileTransform.parent = levelObjectHolder;
                tileTransform.position = new Vector3(tileMapData.position.x, 0, tileMapData.position.y);
                tileTransform.localScale = new Vector3(tileMapData.scale.x, 1, tileMapData.scale.y);
                tileTransform.rotation = Quaternion.AngleAxis(tileMapData.rotateAngle, Vector3.up);
                if (prefab.name.Contains("Cave_Darkness"))
                {
                    tileTransform.position -= Vector3.up * 15f;
                }

                tileObject.gameObject.layer = tileMapData.GetLayer();
                var tileCollider = tileObject.GetComponentInChildren<Collider>();
                if (tileCollider != null &&
                    tileCollider.gameObject.layer != LayerMask.NameToLayer("GroundValidatorDebug"))
                {
                    tileCollider.gameObject.layer = tileMapData.GetLayer();
                }

                listTileObjects.Add(tileObject);
            }

            CheckMapCorners(listTileObjects);
        }

        private static void CheckMapCorners(List<GroundElement> listTileObjects)
        {
            // foreach (var tile in listTileObjects)
            // {
            //     if (tile.Id == "S")
            //     {
            //         var side = tile.GetComponent<GroundSideStraightElement>();
            //         if (side != null)
            //         {
            //             side.AdjustScalesAndPositions();
            //         }
            //     }
            // }
        }

        public static List<BaseItem> SpawnAllItem(List<ItemData> listItemData, Transform levelObjectHolder,
            LevelConfig levelConfig, GameplayItemPrefabConfig itemConfig, int versionItem)
        {
            itemIds.Clear();

            var res = new List<BaseItem>();
            foreach (var itemData in listItemData)
            {
                var itemDefinition = levelConfig.GetItemDefinition(itemData.itemId, itemConfig, versionItem);
                if (itemDefinition == null)
                {
                    Debug.LogError("Cant find item definition with id: " + itemData.itemId);
                    continue;
                }

                var prefab = itemDefinition.GetPrefab();
                if (prefab == null)
                {
                    Debug.LogError("Cant find prefab with id: " + itemData.itemId);
                    continue;
                }

                var newItem = prefab.Spawn();
                var itemComponent = newItem.GetComponent<BaseItem>();
                if (itemComponent == null)
                {
                    itemComponent = newItem.AddComponent<BaseItem>();
                }

                var itemTransform = itemComponent.transform;
                if (!string.IsNullOrEmpty(itemData.groupID))
                {
                    GameObject groupHolder = levelObjectHolder.GetComponentsInChildren<Transform>().ToList()
                        .Find(obj => obj.name == itemData.groupID)?.gameObject;
                    if (groupHolder == null)
                    {
                        groupHolder = new GameObject(itemData.groupID);
                        groupHolder.transform.parent = levelObjectHolder;
                        groupHolder.transform.position = itemData.groupPosition;
                    }
                    
                    itemTransform.parent = groupHolder.transform;
                }
                else
                {
                    itemTransform.parent = levelObjectHolder;
                }

                itemComponent.LoadData(itemData, itemDefinition);
                res.Add(itemComponent);
                itemIds.Add(itemData.itemId);
            }

            return res;
        }


        public static List<BaseItem> SpawnAllItem(List<ItemData> listItemData, Transform levelObjectHolder,
            int versionItem)
        {
            itemIds.Clear();
            var res = new List<BaseItem>();
            foreach (var itemData in listItemData)
            {
                var itemDefinition = Config.levelConfig.GetItemDefinition(itemData.itemId, versionItem);
                if (itemDefinition == null)
                {
                    Debug.LogError("Cant find item definition with id: " + itemData.itemId);
                    continue;
                }

                var prefab = itemDefinition.GetPrefab();
                if (prefab == null)
                {
                    Debug.LogError("Cant find prefab with id: " + itemData.itemId);
                    continue;
                }

                var newItem = prefab.Spawn();
                var itemComponent = newItem.GetComponent<BaseItem>();
                if (itemComponent == null)
                {
                    if (itemDefinition.GetId() == "Coin_Small" || itemDefinition.GetId() == "Coin_Medium")
                    {
                        itemComponent = newItem.AddComponent<CoinItem>();
                    }
                    else
                    {
                        itemComponent = newItem.AddComponent<BaseItem>();
                    }
                }

                var itemTransform = itemComponent.transform;
                itemTransform.parent = levelObjectHolder;
                itemComponent.LoadData(itemData, itemDefinition);
                res.Add(itemComponent);
                itemIds.Add(itemData.itemId);
            }

            return res;
        }

        public static List<BaseItem> SpawnAllItem(List<ItemSpawn> listItemData, Transform levelObjectHolder, int versionItem)
        {
            itemIds.Clear();
            var res = new List<BaseItem>();
            foreach (var itemSpawn in listItemData)
            {
                var itemDefinition = Config.levelConfig.GetItemDefinition(itemSpawn.id, versionItem);
                if (itemDefinition == null)
                {
                    Debug.LogError("Cant find item definition with id: " + itemSpawn.id);
                    continue;
                }

                var prefab = itemDefinition.GetPrefab();
                if (prefab == null)
                {
                    Debug.LogError("Cant find prefab with id: " + itemSpawn.id);
                    continue;
                }

                bool newLogicWhirlpool = Pool.Get<Game>().usingNewLogicPull;
                foreach (var spawnData in itemSpawn.listSpawnDatas)
                {
                    var newItem = prefab.Spawn();
                    var itemComponent = newItem.GetComponent<BaseItem>();
                    if (itemComponent == null)
                    {
                        if (itemDefinition.GetId() == "Coin_Small" || itemDefinition.GetId() == "Coin_Medium")
                        {
                            itemComponent = newItem.AddComponent<CoinItem>();
                        }
                        else
                        {
                            itemComponent = newItem.AddComponent<BaseItem>();
                        }
                    }

                    var itemTransform = itemComponent.transform;
                    
                    itemTransform.parent = levelObjectHolder;
                    itemComponent.LoadData(spawnData.ToItemData(itemSpawn.id), itemDefinition);

                    var itemData = itemComponent.ItemData;
                    if (!string.IsNullOrEmpty(itemData.groupID))
                    {
                        GameObject groupHolder = levelObjectHolder.GetComponentsInChildren<Transform>().ToList()
                            .Find(obj => obj.name == itemData.groupID)?.gameObject;
                        if (groupHolder == null)
                        {
                            groupHolder = new GameObject(itemData.groupID);
                            groupHolder.transform.parent = levelObjectHolder;
                            groupHolder.transform.position = itemData.groupPosition;
                        }
                    
                        itemTransform.parent = groupHolder.transform;
                    }
                    
                    res.Add(itemComponent);
                    TCP2_GetPosOnWater waterWave = newItem.AddComponent<TCP2_GetPosOnWater>();
                    waterWave.WaterMaterial = Config.waveWaterMaterial;

                    float objectSize = itemComponent.MyRenderer.bounds.size.y;
                    float objectOffset = 0;
                    if (newLogicWhirlpool)
                    {
                        objectOffset = -objectSize / 2;
                    }
                    waterWave.heightOffset = spawnData.ToItemData(itemSpawn.id).position.y - objectOffset ;
                    #if UNITY_EDITOR
                    waterWave.OnValidate();
                    #endif
                    itemComponent.posOnWater = waterWave; 
                    itemIds.Add(itemSpawn.id);
                }
                
            }

            return res;
        }

        public static List<BaseItem> SpawnBoosterItems(int count, Transform levelObjectHolder,
            ItemDefinition itemDefinition, ItemData itemData)
        {
            var res = new List<BaseItem>();
            var prefab = itemDefinition.GetPrefab();
            if (prefab == null)
            {
                return res;
            }

            for (int i = 0; i < count; i++)
            {
                var newItem = prefab.Spawn();
                var itemComponent = newItem.GetComponent<BaseItem>();
                if (itemComponent == null)
                {
                    itemComponent = newItem.AddComponent<BaseItem>();
                }

                var itemTransform = itemComponent.transform;
                itemTransform.parent = levelObjectHolder;
                itemComponent.LoadData(itemData, itemDefinition);
                res.Add(itemComponent);
                itemIds.Add(itemData.itemId);
            }

            return res;
        }
        
        public static List<BaseItem> SpawnSpecialItems(Bounds bounds, int count, Transform levelObjectHolder,
            ItemDefinition itemDefinition)
        {
            var res = new List<BaseItem>();
            var prefab = itemDefinition.GetPrefab();
            if (prefab == null)
            {
                return res;
            }

            for (int i = 0; i < count; i++)
            {
                var newItem = prefab.Spawn();
                var itemComponent = newItem.GetComponent<BaseItem>();
                if (itemComponent == null)
                {
                    itemComponent = newItem.AddComponent<BaseItem>();
                }

                var itemTransform = itemComponent.transform;
                var itemData = new ItemData()
                {
                    itemId = itemDefinition.GetId(),
                    position = Vector3.zero, //Do random pos later
                    eulerAngle = Vector3.zero,
                    localScale = Vector3.one * Config.specialObjectConfig.GetRandomSpawnSize(),
                    isStartKinematic =  true,
                };
                
                itemTransform.parent = levelObjectHolder;
                itemComponent.LoadData(itemData, itemDefinition);

                itemComponent.ItemData.position =
                    FindValidSpawnPosition(
                        bounds,
                        itemComponent.GetComponentInChildren<Collider>()?.bounds.extents.magnitude ?? 2f,
                        1000,
                        itemComponent.gameObject.layer
                    );
                
                itemComponent.transform.position = itemComponent.ItemData.position;
                
                res.Add(itemComponent);
                itemIds.Add(itemData.itemId);
            }

            return res;
        }

        public static List<BombItem> SpawnAllBomb(LevelConfigModel levelConfigModel, List<BaseItem> allItemInMap,
            Transform levelObjectHolder, PrefabConfig config = null)
        {
            var prefabConfig = config == null ? Config.prefabConfig : config;
            var res = new List<BombItem>();
            if (levelConfigModel.levelBoomData == null) return null;
            var levelBombData = levelConfigModel.levelBoomData;

            if (levelBombData.isOverrideBombData && levelBombData.listBombDatas.Count > 0)
            {
                foreach (var bombData in levelBombData.listBombDatas)
                {
                    var bomb = prefabConfig.bombPrefab.Spawn(levelObjectHolder);
                    var bombTransform = bomb.transform;
                    bombTransform.localScale = bombData.scale.ToVector3();
                    bombTransform.position = bombData.pos.ToVector3();
                    res.Add(bomb);
                }
            }
            else
            {
                var number = levelConfigModel.levelBoomData.number;
                var cacheAllItems = allItemInMap.ToList();
                for (int i = 0; i < number; i++)
                {
                    float scale = RandomHelper.RangeWithSeed(levelBombData.seedScale + i,
                        (int)(100 * levelBombData.scaleMin),
                        (int)(100 * levelBombData.scaleMax)) / 100f;
                    var itemSelected = cacheAllItems.SampleWithSeed(levelBombData.seedPosition + i);
                    cacheAllItems.Remove(itemSelected);
                    if (itemSelected.ItemData.itemId.StartsWith("Column_") &&
                        itemSelected.ItemData.itemId.EndsWith("_Part"))
                    {
                        continue;
                    }

                    var position = itemSelected.transform.position + Vector3.up * 10;

                    var offSet = 1;
                    var randValue = RandomHelper.RangeWithSeed(i, 0, 1);
                    Vector3 randomOffset = new Vector3(
                        randValue > 0.5f ? offSet : -offSet,
                        0f,
                        randValue > 0.5f ? offSet : -offSet
                    );
                    position += randomOffset;

                    var bomb = prefabConfig.bombPrefab.Spawn(levelObjectHolder);
                    var bombTransform = bomb.transform;
                    bombTransform.localScale = scale * Vector3.one;
                    bombTransform.position = position;
                    res.Add(bomb);
                }
            }

            return res;
        }

        public static List<BombItem> SpawnAllBomb(LevelDataSO levelConfigModel, List<BaseItem> allItemInMap,
            Transform levelObjectHolder, PrefabConfig config = null)
        {
            var prefabConfig = config == null ? Config.prefabConfig : config;
            var res = new List<BombItem>();
            if (levelConfigModel.levelBoomData == null) return null;
            var levelBombData = levelConfigModel.levelBoomData;

            if (levelBombData.isOverrideBombData && levelBombData.listBombDatas.Count > 0)
            {
                foreach (var bombData in levelBombData.listBombDatas)
                {
                    var bomb = prefabConfig.bombPrefab.Spawn(levelObjectHolder);
                    var bombTransform = bomb.transform;
                    bombTransform.localScale = bombData.scale.ToVector3();
                    bombTransform.position = bombData.pos.ToVector3();
                    res.Add(bomb);
                }
            }
            else
            {
                var number = levelConfigModel.levelBoomData.number;
                var cacheAllItems = allItemInMap.ToList();
                if (allItemInMap.Count <= 0) return res;
                for (int i = 0; i < number; i++)
                {
                    float scale = RandomHelper.RangeWithSeed(levelBombData.seedScale + i,
                        (int)(100 * levelBombData.scaleMin),
                        (int)(100 * levelBombData.scaleMax)) / 100f;
                    var itemSelected = cacheAllItems.SampleWithSeed(levelBombData.seedPosition + i);
                    cacheAllItems.Remove(itemSelected);
                    if (itemSelected.ItemData.itemId.StartsWith("Column_") &&
                        itemSelected.ItemData.itemId.EndsWith("_Part"))
                    {
                        continue;
                    }

                    var position = itemSelected.transform.position + Vector3.up * 10;

                    var offSet = 1;
                    var randValue = RandomHelper.RangeWithSeed(i, 0, 1);
                    Vector3 randomOffset = new Vector3(
                        randValue > 0.5f ? offSet : -offSet,
                        0f,
                        randValue > 0.5f ? offSet : -offSet
                    );
                    position += randomOffset;

                    var bomb = prefabConfig.bombPrefab.Spawn(levelObjectHolder);
                    var bombTransform = bomb.transform;
                    bombTransform.localScale = scale * Vector3.one;
                    bombTransform.position = position;
                    res.Add(bomb);
                }
            }

            return res;
        }

        //public static List<SpecialObject> SpawnAllSpecialObject(List<BaseItem> allItemInMap,
        //    Transform levelObjectHolder, int level)
        //{
        //    var res = new List<SpecialObject>();
//
        //    var treasureConfig = Config.treasureHuntProgressConfig;
        //    var currentSpecialId =
        //        treasureConfig.GetSpecialObjectId(Controllers.Get<TreasureHuntController>().treasureHuntData
        //            .progressTimeCount);
        //    var specialObjectDefinition = Config.treasureHuntProgressConfig.GetSpecialItemDefinition(currentSpecialId);
        //    var prefab = specialObjectDefinition.prefabInGame;
//
        //    int number = treasureConfig.GetSpecialObjectNumber(level);
        //    var cacheAllItems = allItemInMap.ToList();
        //    for (int i = 0; i < number; i++)
        //    {
        //        var itemSize = RandomHelper.RangeWithSeed(level + i, (int)(specialObjectDefinition.minSize * 100),
        //            (int)(specialObjectDefinition.maxSize * 100)) / 100f;
        //        var itemSelected = cacheAllItems.SampleWithSeed(level + 100 + i * 2);
        //        cacheAllItems.Remove(itemSelected);
        //        var position = itemSelected.transform.position + Vector3.up;
        //        var itemObject = prefab.Spawn();
        //        var itemTransform = itemObject.transform;
        //        itemTransform.parent = levelObjectHolder;
        //        itemTransform.localScale = Vector3.one * itemSize;
        //        itemTransform.position = position + Vector3.up;
        //        res.Add(itemObject);
        //    }
//
        //    return res;
        //}
        
        public static Vector3 FindValidSpawnPosition(Bounds bounds, float checkRadius, int maxAttempts, LayerMask itemLayer) {
            for (int i = 0; i < maxAttempts; i++) {
                Vector3 randomPos = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    0,
                    Random.Range(bounds.min.z, bounds.max.z)
                );

                if (IsPositionAvailable(randomPos, checkRadius, itemLayer)) {
                    return randomPos;
                }
            }

            return Vector3.zero; // Couldn't find a free spot
        }
        
        public static bool IsPositionAvailable(Vector3 position, float checkRadius, LayerMask itemLayerMask) {
            // Check for any colliders in the area
            Collider[] hits = Physics.OverlapSphere(position, checkRadius);
    
            foreach (var hit in hits) {
                if (hit.transform.parent.GetComponent<BaseItem>() != null) {
                    return false; // Space is occupied
                }
            }

            return true; // No BaseItem found nearby
        }

        public static LevelData CreateLevelData(LevelConfigModel levelConfigModel, int level)
        {
            var levelData = new LevelData();
            levelData.time = levelConfigModel.duration;
            levelData.levelObjectives = new List<LevelObjective>();
            foreach (var levelObjective in levelConfigModel.levelObjectives)
            {
                levelData.levelObjectives.Add(levelObjective.Clone());
            }

            if (levelConfigModel.type == "normal")
            {
                levelData.levelType = LevelType.NORMAL;
            }
            else if (levelConfigModel.type == "breaktime")
            {
                levelData.levelType = LevelType.COIN;
            }

            levelData.level = level;
            return levelData;
        }

        public static LevelData CreateLevelData(LevelDataSO levelConfigModel, int level)
        {
            var levelData = new LevelData();
            levelData.time = levelConfigModel.time;
            levelData.levelObjectives = new List<LevelObjective>();
            foreach (var levelObjective in levelConfigModel.listObjectives)
            {
                levelData.levelObjectives.Add(levelObjective.ToLevelObjective());
            }

            if (levelConfigModel.difficulty == "normal")
            {
                levelData.levelType = LevelType.NORMAL;
            }
            else if (levelConfigModel.difficulty == "breaktime")
            {
                levelData.levelType = LevelType.COIN;
            }

            levelData.level = level;
            return levelData;
        }
    }
}