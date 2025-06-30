using System.Collections.Generic;
using System.Linq;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public static class LevelHelper
    {
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

        public static void SpawnMap(MapData mapData, Transform levelObjectHolder, ThemeConfig config = null)
        {
            //var themeConfig = config;
            //var listTileObjects = new List<GroundElement>();
            //foreach (var tileMapData in mapData.listMapTileData)
            //{
            //    var prefab = themeConfig.GetLevelMapTile(tileMapData.id, mapData.groupSkinId);
            //    if (prefab == null) continue;
            //    //var tileObject = prefab.Spawn();
            //    var tileTransform = tileObject.transform;
            //    tileTransform.parent = levelObjectHolder;
            //    tileTransform.position = new Vector3(tileMapData.position.x, 0, tileMapData.position.y);
            //    tileTransform.localScale = new Vector3(tileMapData.scale.x, 1, tileMapData.scale.y);
            //    tileTransform.rotation = Quaternion.AngleAxis(tileMapData.rotateAngle, Vector3.up);
            //    if (prefab.name.Contains("Cave_Darkness"))
            //    {
            //        tileTransform.position -= Vector3.up * 15f;
            //    }

            //    tileObject.gameObject.layer = tileMapData.GetLayer();
            //    var tileCollider = tileObject.GetComponentInChildren<Collider>();
            //    if (tileCollider != null &&
            //        tileCollider.gameObject.layer != LayerMask.NameToLayer("GroundValidatorDebug"))
            //    {
            //        tileCollider.gameObject.layer = tileMapData.GetLayer();
            //    }

            //    listTileObjects.Add(tileObject);
            //}

            //CheckMapCorners(listTileObjects);
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

        

     
        

        public static List<BombItem> SpawnAllBomb(LevelDataSO levelConfigModel, List<BaseItem> allItemInMap,
            Transform levelObjectHolder, PrefabConfig config = null)
        {
            var res = new List<BombItem>();

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

        public static LevelData CreateLevelData(LevelConfigModel levelConfigModel)
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

            return levelData;
        }

        public static LevelData CreateLevelData(LevelDataSO levelConfigModel)
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

            return levelData;
        }
    }
}