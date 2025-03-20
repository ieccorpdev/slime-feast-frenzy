using System;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.ExportLevel.OriginalLevel;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _IEC.Features.Gameplay
{
    public class MapEditor : MonoBehaviour
    {
        public RuleTile ruleTile;
        public Tilemap tilemap;
        public MapTileConfig mapTileConfig;

        [Button]
        public void ClearMap()
        {
            tilemap.ClearAllTiles();
        }
   
        [Button]
        public void LoadMap(GroundJsonData groundJsonData)
        {
            // Clear existing tiles
            tilemap.ClearAllTiles();

            // Iterate through elements in GroundJsonData
            foreach (var kvp in groundJsonData.elts)
            {
                // Find the tile pair corresponding to this id
                var id = mapTileConfig.mapTilePairs.Find(pair => pair.id == kvp.Key).id;
                if (id == "")
                    continue;

                foreach (var element in kvp.Value)
                {
                    // Extract position, scale, and rotation
                    var centerPosition = new Vector3Int(
                        Mathf.FloorToInt(element.p.x / 4),
                        Mathf.FloorToInt(element.p.y / 4),
                        0
                    );

                    var scaleX = Mathf.RoundToInt(id == "S" ? element.s.y / 8 : element.s.x / 8);
                    var scaleY = Mathf.RoundToInt(id == "S" ? element.s.x / 8 : element.s.y / 8);
                    
                    if (scaleX == 0 && scaleY == 0)
                    {
                        tilemap.SetTile(centerPosition, ruleTile);
                    }
                    else if (scaleX == 0)
                    {
                        for (int y = -scaleY; y <= scaleY; y++)
                        {
                            var position = new Vector3Int(centerPosition.x, centerPosition.y + y, 0);
                            tilemap.SetTile(position, ruleTile);
                        }
                    }
                    else if (scaleY == 0)
                    {
                        for (int x = -scaleX; x <= scaleX; x++)
                        {
                            var position = new Vector3Int(centerPosition.x + x, centerPosition.y, 0);
                            tilemap.SetTile(position, ruleTile);
                        }
                    }
                    else
                    {

                        for (int x = -scaleX; x <= scaleX; x++)
                        {
                            for (int y = -scaleY; y <= scaleY; y++)
                            {
                                var position = new Vector3Int(centerPosition.x + x, centerPosition.y + y, 0);
                                tilemap.SetTile(position, ruleTile);
                            }
                        }
                    }
                }
            }
        }


        [Button]
        public GroundJsonData ProcessTilemap()
        {
            var mapMinX = tilemap.cellBounds.xMin;
            var mapMaxX = tilemap.cellBounds.xMax;
            var mapMinY = tilemap.cellBounds.yMin;
            var mapMaxY = tilemap.cellBounds.yMax;

            var currentSpriteName = "";
            var minX = mapMaxX;
            var maxX = mapMinX;
            var minY = mapMaxY;
            var maxY = mapMinY;

            var tileDataDict = new Dictionary<string, List<GroundElement>>();
            foreach (var pair in mapTileConfig.mapTilePairs)
            {
                tileDataDict[pair.id] = new List<GroundElement>();
            }

            for (int x = mapMinX; x <= mapMaxX; x++)
            {
                for (int y = mapMinY; y <= mapMaxY; y++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    var sprite = tilemap.GetSprite(pos);
                    if (sprite == null)
                    {
                        continue;
                    }
                    
                    if (currentSpriteName == "")
                    {
                        currentSpriteName = sprite.name;
                        minX = maxX = x;
                        minY = maxY = y;
                        // continue;
                    }

                    if (currentSpriteName == sprite.name)
                    {
                        SetMinMax(x, y, ref minX, ref maxX, ref minY, ref maxY);
                    }

                    if (currentSpriteName != sprite.name)
                    {
                        var pair = GetPairFromSprite(currentSpriteName);
                        if (pair == null)
                        {
                            continue;
                        }
                        
                        CreateTileData(pair, tileDataDict[pair.id], minX, maxX, minY, maxY);

                        currentSpriteName = sprite.name;
                        minX = maxX = x;
                        minY = maxY = y;
                    }
                }
            }
            
            var lastPair = GetPairFromSprite(currentSpriteName);
            if (lastPair != null)
            {
                CreateTileData(lastPair, tileDataDict[lastPair.id], minX, maxX, minY, maxY);
            }

            var listKeys = (from kvp in tileDataDict where kvp.Value.Count == 0 select kvp.Key).ToList();
            foreach (var key in listKeys)
            {
                tileDataDict.Remove(key);
            }

            var groundJsonData = new GroundJsonData
            {
                elts = tileDataDict
            };

            return groundJsonData;
        }

        private void SetMinMax(int x, int y, ref int minX, ref int maxX, ref int minY, ref int maxY)
        {
            if (x < minX)
                minX = x;

            if (x > maxX)
                maxX = x;

            if (y < minY)
                minY = y;

            if (y > maxY)
                maxY = y;
        }

        private MapTilePair GetPairFromSprite(string spriteName)
        {
            var pair = mapTileConfig.mapTilePairs.Find(pair => pair.tileSprite.name == spriteName);
            return pair;
        }

        private void CreateTileData(MapTilePair pair, List<GroundElement> dataList,
            int minX, int maxX, int minY, int maxY)
        {
            var centerPos = GetCenterPos(minX, maxX, minY, maxY);
            
            if (pair.id == "IC")
            {
                centerPos = ProcessInvertCorner(pair.rotation, centerPos);
            }
            var xScale = (Mathf.Abs(maxX - minX) + 1) * 4;
            var yScale = (Mathf.Abs(maxY - minY) + 1) * 4;
            var scale = new GroundScale
            {
                x = pair.id == "S" ? yScale : xScale,
                y = pair.id == "S" ? xScale : yScale
            };
            var rotation = pair.rotation;

            var groundElement = new GroundElement
            {
                p = centerPos,
                r = rotation,
                s = scale
            };

            dataList.Add(groundElement);
        }

        private GroundPosition GetCenterPos(int minX, int maxX, int minY, int maxY)
        {
            var minXminYPos = tilemap.GetCellCenterWorld(new Vector3Int(minX, minY, 0));
            var maxXMaxYPos = tilemap.GetCellCenterWorld(new Vector3Int(maxX, maxY, 0));
            var center = (minXminYPos + maxXMaxYPos) / 2f;
            center.x = minX == maxX ? minXminYPos.x : center.x;
            center.z = minY == maxY ? minXminYPos.z : center.z;

            return new GroundPosition
            {
                x = center.x,
                y = center.z
            };
        }

        private GroundPosition ProcessInvertCorner(float rotation, GroundPosition position)
        {
            switch (rotation)
            {
                case 0:
                    position.x -= 4;
                    position.y -= 4;
                    break;
                case 90:
                    position.x -= 4;
                    position.y += 4;
                    break;
                case 180:
                    position.x += 4;
                    position.y += 4;
                    break;
                case 270:
                    position.x += 4;
                    position.y -= 4;
                    break;
            }

            return position;
        }
    }
}