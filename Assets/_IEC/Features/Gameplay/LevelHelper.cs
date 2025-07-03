using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
            if (mapData == null) return;
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

        public static List<BaseItem> GenerateRandomLevelAllItems(Transform levelObjectHolder,
            GameplayItemPrefabConfig itemConfig)
        {
            List<BaseItem> listBaseItems = new();
            float spacing = 1f;
            Vector2 cursor = Vector2.zero;
            float rowHeight = 0f;
            float levelWidth = 0f;

            float estimatedSide = Mathf.Sqrt(itemConfig.ListItemDefinition.Count) * 10f; // tweak this estimate

            List<Bounds> placedBounds = new();

            // bool IsOverlapping(Bounds newBounds) {
            //     foreach (var existing in placedBounds) {
            //         if (existing.Intersects(newBounds)) return true;
            //     }
            //     return false;
            // }

            for (int i = 0; i < itemConfig.ListItemDefinition.Count; i++)
            {
                if (itemConfig.ListItemDefinition[i].GetPrefab() == null)
                    continue;

                var bound = GroundExtend.GetRendererBounds(itemConfig.ListItemDefinition[i].GetPrefab());
                var boundSize = bound.size;

                // Try to place it at cursor position
                UnityEngine.Vector3 tryPosition = new UnityEngine.Vector3(cursor.x, 0f, cursor.y);
                Bounds futureBounds = new Bounds(bound.center + tryPosition, boundSize);

                //if (!IsOverlapping(futureBounds)) 
                {
                    var newItem = itemConfig.ListItemDefinition[i].GetPrefab().Spawn();
                    //GameObject instance = Instantiate(Config.itemPrefabConfig.ListItemDefinition[i].GetPrefab(), tryPosition, Quaternion.identity, itemHolder);
                    Bounds placed = GroundExtend.GetRendererBounds(newItem);
                    placed.center += tryPosition;

                    var itemComponent = newItem.GetComponent<BaseItem>();
                    if (itemComponent == null)
                    {
                        itemComponent = newItem.AddComponent<BaseItem>();
                    }

                    newItem.transform.parent = levelObjectHolder;

                    itemComponent.LoadData(new ItemData()
                        {
                            itemId = itemConfig.ListItemDefinition[i].GetId(),
                            //position = newItem.transform.position,
                            position = tryPosition,
                            eulerAngle = newItem.transform.eulerAngles,
                            isStartKinematic = true
                        },
                        itemConfig.ListItemDefinition[i]);

                    placedBounds.Add(placed);

                    // Move cursor forward
                    cursor.x += boundSize.x + spacing;
                    rowHeight = Mathf.Max(rowHeight, boundSize.z);

                    // Wrap if we get too wide
                    levelWidth += boundSize.x + spacing;
                    if (levelWidth > estimatedSide)
                    {
                        cursor.x = 0f;
                        cursor.y += rowHeight + spacing;
                        rowHeight = 0f;
                        levelWidth = 0f;
                    }

                    listBaseItems.Add(itemComponent);
                }
            }

            return listBaseItems;
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

        public static List<BaseItem> SpawnAllItem(List<ItemSpawn> listItemData, Transform levelObjectHolder,
            int versionItem)
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

                var themeConfig = Config.themeConfig.GetWaterThemeForLevel(PlayerData.level);
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetColor("_BaseColor", themeConfig.baseItemColor);
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
                    if (Pool.Get<Game>().iosFeature)
                    {
                        TCP2_GetPosOnWater waterWave = newItem.AddComponent<TCP2_GetPosOnWater>();
                        waterWave.WaterMaterial = Config.waveWaterMaterial;

                        float objectSize = itemComponent.MyRenderer.bounds.size.y;
                        float objectOffset = 0;
                        if (newLogicWhirlpool)
                        {
                            objectOffset = -objectSize / 2;
                        }

                        waterWave.heightOffset = spawnData.ToItemData(itemSpawn.id).position.y - objectOffset;
                        waterWave.InitShader();
                        itemComponent.SetPosOnwater(waterWave);
                    }

                    //Set ColorTheme
                    var meshRederers = itemComponent.gameObject.GetComponentsInChildren<MeshRenderer>();

                    foreach (var mr in meshRederers)
                    {
                        mr.SetPropertyBlock(block);
                    }

                    itemIds.Add(itemSpawn.id);
                }
            }

            return res;
        }

        public static List<BaseItem> SpawnBoosterItems(int count, Transform levelObjectHolder,
            BoosterData boosterData)
        {
            var res = new List<BaseItem>();
            var prefab = boosterData.itemDefinition.GetPrefab();

            if (count <= 0)
                return res;

            if (prefab == null)
            {
                return res;
            }

            for (int i = 0; i < count; i++)
            {
                var newItem = prefab.Spawn();
                var itemComponent = newItem.GetComponent<BaseBoosterItem>();
                if (itemComponent == null)
                {
                    itemComponent = newItem.AddComponent<BaseBoosterItem>();
                }

                itemComponent.SetBoosterData(boosterData);

                var itemData = new ItemData()
                {
                    itemId = boosterData.itemDefinition.GetId(),
                    position = Vector3.zero, //Do random pos later
                    eulerAngle = Vector3.zero,
                    localScale = Vector3.one,
                    isStartKinematic = true,
                };

                var itemTransform = itemComponent.transform;
                itemTransform.parent = levelObjectHolder;
                itemComponent.LoadData(itemData, boosterData.itemDefinition);
                res.Add(itemComponent);
                itemIds.Add(itemData.itemId);
            }

            return res;
        }
        
        public static List<BaseCharacterShardItem> SpawnCharacterShardItems(Bounds bounds, int count, Transform levelObjectHolder,
            ItemDefinition itemDefinition)
        {
            var res = new List<BaseCharacterShardItem>();
            if (itemDefinition == null) return res;
            var prefab = itemDefinition.GetPrefab();
            if (prefab == null)
            {
                return res;
            }
            List<CharacterConfig> ignoreCharacterList = new List<CharacterConfig>();
            foreach (var characterConfig in Config.characterShardObjectConfig.listCharacterConfigForShard)
            {
                var CurrentCharacterConfig =
                    Config.allCharactersConfig.GetCharacterConfig(characterConfig.characterName);
                var CurrentCharacterLevel = PlayerData.GetCharacterLevel(CurrentCharacterConfig.characterName);
                var upgradeConfig = CurrentCharacterConfig.GetCharacterUpgrade(CurrentCharacterLevel + 1);
                if (upgradeConfig == null)
                {
                    ignoreCharacterList.Add(characterConfig);
                }
            }
            var charConfig = Config.characterShardObjectConfig.GetRandomCharacterConfigForShard(ignoreCharacterList);
            if (charConfig is null) return res;
            for (int i = 0; i < count; i++)
            {
                var newItem = prefab.Spawn();
                var itemComponent = newItem.GetComponent<BaseCharacterShardItem>();
                if (itemComponent == null)
                {
                    itemComponent = newItem.AddComponent<BaseCharacterShardItem>();
                }

                var itemTransform = itemComponent.transform;
                //float scale = Config.specialObjectConfig.GetRandomSpawnSize(itemDefinition.GetId());
                var itemData = new ItemData()
                {
                    itemId = itemDefinition.GetId(),
                    position = Vector3.zero, //Do random pos later
                    eulerAngle = Vector3.zero,
                    localScale = Vector3.one,
                    isStartKinematic = false,
                };
                itemTransform.parent = levelObjectHolder;
                //itemTransform.GetComponentInChildren<SpecialItemParticleEffectHighlight>().transform.localScale /= scale;

                itemComponent.Setup(charConfig);
                itemComponent.LoadData(itemData, itemDefinition);
                itemComponent.ItemData.position =
                    FindValidSpawnPosition(
                        bounds,
                        itemComponent.GetComponentInChildren<Collider>()?.bounds.extents.magnitude ?? 2f,
                        1000,
                        itemComponent.gameObject.layer
                    );
                itemComponent.transform.position = itemComponent.ItemData.position + Vector3.up * 10;
                res.Add(itemComponent);
                itemIds.Add(itemData.itemId);
            }

            return res;
        }

        public static List<BaseItem> SpawnSpecialItems(Bounds bounds, int count, Transform levelObjectHolder,
            ItemDefinition itemDefinition)
        {
            var res = new List<BaseItem>();
            if (itemDefinition == null) return res;
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
                float scale = Config.specialObjectConfig.GetRandomSpawnSize(itemDefinition.GetId());
                var itemData = new ItemData()
                {
                    itemId = itemDefinition.GetId(),
                    position = Vector3.zero, //Do random pos later
                    eulerAngle = Vector3.zero,
                    localScale = Vector3.one * scale,
                    isStartKinematic = false,
                };
                itemTransform.parent = levelObjectHolder;
                //itemTransform.GetComponentInChildren<SpecialItemParticleEffectHighlight>().transform.localScale /= scale;
                itemComponent.LoadData(itemData, itemDefinition);
                itemComponent.ItemData.position =
                    FindValidSpawnPosition(
                        bounds,
                        itemComponent.GetComponentInChildren<Collider>()?.bounds.extents.magnitude ?? 2f,
                        1000,
                        itemComponent.gameObject.layer
                    );
                itemComponent.transform.position = itemComponent.ItemData.position + Vector3.up * 10;
                res.Add(itemComponent);
                itemIds.Add(itemData.itemId);
            }

            return res;
        }

        public static List<BaseItem> SpawnRelicItems(Bounds bounds, int spawnTime, Transform levelObjectHolder,
            ItemDefinition itemDefinition)
        {
            var res = new List<BaseItem>();
            if (itemDefinition == null) return res;
            var prefab = itemDefinition.GetPrefab();
            if (prefab == null)
            {
                return res;
            }

            //List<BaseItem> tempList = new();
            for (int i = 0; i < spawnTime; i++)
            {
                var spawnSize = Config.relicConfig.GetRandomSpawnSize();
                var spawmAmount = Config.relicConfig.GetRandomSpawnAmount();
                Vector3? spawnPos = null;
                for (int j = 0; j < spawmAmount; j++)
                {
                    var newItem = prefab.Spawn();
                    spawnPos ??= FindValidSpawnPosition(
                        bounds,
                        newItem.GetComponentInChildren<Collider>()?.bounds.extents.magnitude ?? 2f,
                        1000,
                        newItem.gameObject.layer
                    );
                    var itemComponent = newItem.GetComponent<BaseItem>();
                    if (itemComponent == null)
                    {
                        itemComponent = newItem.AddComponent<BaseItem>();
                    }

                    var itemTransform = itemComponent.transform;
                    var itemData = new ItemData()
                    {
                        itemId = itemDefinition.GetId(),
                        position = spawnPos.Value,
                        eulerAngle = Vector3.zero,
                        localScale = Vector3.one * spawnSize,
                        isStartKinematic = true,
                    };
                    itemTransform.parent = levelObjectHolder;
                    itemComponent.LoadData(itemData, itemDefinition);
                    itemComponent.transform.position = spawnPos.Value;
                    res.Add(itemComponent);
                    itemIds.Add(itemData.itemId);
                    // hard code logic
                    if (j == 0) //first object spawn
                    {
                        Debug.Log($"itemComponent.GetComponent<AddForceOnCollision>().OnCollision");

                        itemComponent.GetComponent<AddForceOnCollision>().OnCollision = () =>
                        {
                            switch (spawmAmount)
                            {
                                case 3:
                                    EventController.RaiseEvent(EventList.Audio.RELIC_CLONE_3);
                                    Debug.Log($"EventList.Audio.RELIC_CLONE_3");
                                    break;
                                case 4:
                                    EventController.RaiseEvent(EventList.Audio.RELIC_CLONE_4);
                                    Debug.Log($"EventList.Audio.RELIC_CLONE_4");

                                    break;
                                case 5:
                                    EventController.RaiseEvent(EventList.Audio.RELIC_CLONE_5);
                                    Debug.Log($"EventList.Audio.RELIC_CLONE_5");

                                    break;
                            }
                        };
                    }
                }

                // for (int j = 0; j < tempList.Count; j++)
                // {
                //     tempList[i].transform.position = spawnPos.Value;
                // }
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

        public static Vector3 FindValidSpawnPosition(Bounds bounds, float checkRadius, int maxAttempts,
            LayerMask itemLayer)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                Vector3 randomPos = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    0,
                    Random.Range(bounds.min.z, bounds.max.z)
                );

                if (IsPositionAvailable(randomPos, checkRadius, itemLayer))
                {
                    return randomPos;
                }
            }

            return Vector3.zero; // Couldn't find a free spot
        }

        public static bool IsPositionAvailable(Vector3 position, float checkRadius, LayerMask itemLayerMask)
        {
            // Check for any colliders in the area
            Collider[] hits = Physics.OverlapSphere(position, checkRadius);

            foreach (var hit in hits)
            {
                if (hit.transform.parent.GetComponent<BaseItem>() != null)
                {
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