using System.Collections.Generic;
using _IEC.Features.Gameplay;
using Newtonsoft.Json;
using UnityEngine;

namespace _IEC.Features.ExportLevel.OriginalLevel
{
    public static class LevelDataAdapter
    {
        public static List<_IEC.Features.Gameplay.ItemData> GetAllItemData(string fileContent)
        {
            var res = new List<_IEC.Features.Gameplay.ItemData>();
            Root root = JsonConvert.DeserializeObject<Root>(fileContent);
            if (root == null) return null;
            foreach (var instantiateData in root.InstantiateData)
            {
                foreach (var originalItemData in instantiateData.itemData)
                {
                    var itemData = new _IEC.Features.Gameplay.ItemData();
                    itemData.itemId = instantiateData.itemId;
                    itemData.position = new Vector3(originalItemData.pos.x, originalItemData.pos.y, originalItemData.pos.z);
                    itemData.eulerAngle = new Vector3(originalItemData.rot.x, originalItemData.rot.y, originalItemData.rot.z);
                    if (originalItemData.scale != null)
                        itemData.localScale = new Vector3(originalItemData.scale.x, originalItemData.scale.y, originalItemData.scale.z);
                    itemData.cont = originalItemData.cont;
                    itemData.isStartKinematic = originalItemData.sk;
                    itemData.oLvl = originalItemData.oLvl;
                    res.Add(itemData);
                }
            }
            return res;
        }

        public static MapData GetMapData(string skinId, string contentFile)
        {
            var validString = contentFile.Replace("\\", "").Replace("\"{","{").Replace("}\"","}");
            var data = JsonConvert.DeserializeObject<GroundRoot>(validString);
            if (data == null) return null;
            var mapData = new MapData();
            if (string.IsNullOrEmpty(skinId))
            {
                if (string.IsNullOrEmpty(data.GroundSkin))
                {
                    mapData.groupSkinId = skinId;
                }
            }
            mapData.groupSkinId = skinId;
            mapData.listMapTileData = new List<MapTileData>();
            var elements = data.GroundJson.elts;
            LoadElements(elements, mapData);
            /*foreach (var entry in data.GroundEditorJson.elts)
            {
                foreach (var  element in entry.Value)
                {
                    var tile = new MapTileData();
                    tile.id = entry.Key;
                    tile.position = new Vector2(element.p.x,element.p.y);
                    tile.scale = new Vector2(element.s.x, element.s.y);
                    tile.rotateAngle = element.r;
                    mapData.listMapTileData.Add(tile);
                }
            }*/
            return mapData;
        }
        
        private static void LoadElements(Dictionary<string, List<GroundElement>> elements, MapData mapData)
        {
            foreach (var entry in elements)
            {
                foreach (var element in entry.Value)
                {
                    var tile = new MapTileData();
                    tile.id = entry.Key;
                    tile.position = new Vector2(element.p.x, element.p.y);
                    tile.scale = new Vector2(element.s.x, element.s.y);
                    tile.rotateAngle = element.r;
                    mapData.listMapTileData.Add(tile);
                }
            }
        }

        public static MapData GetMapData(string skinId, GroundShapePresetParameters preset)
        {
            var mapData = new MapData();
            mapData.groupSkinId = skinId;
            mapData.listMapTileData = new List<MapTileData>();
            var groundJson = JsonConvert.DeserializeObject<GroundJsonData>(preset.Json);
            if (groundJson == null) return null;
            var editorGroundJson = JsonConvert.DeserializeObject<GroundJsonData>(preset.EditorJson);
            //todo something with editor ground json
            LoadElements(groundJson.elts,mapData);
            return mapData;
        }
    }
}