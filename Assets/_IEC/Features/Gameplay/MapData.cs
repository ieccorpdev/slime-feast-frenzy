using System;
using System.Collections.Generic;
using _IEC.Features.ExportLevel.OriginalLevel;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class MapData
    {
        public string groupSkinId;
        public List<MapTileData> listMapTileData;
        
    }

    public class MapTileData
    {
        public string id;
        public Vector2 position;
        public Vector2 scale;
        public float rotateAngle;

        public int GetLayer()
        {
            if (id == "W") return LayerMask.NameToLayer("Wall");
            if (id == "H20") return LayerMask.NameToLayer("Water");
            return LayerMask.NameToLayer("Ground");
        }
    }

    [Serializable]
    public class PrefabMapData
    {
        public GameObject mapPrefab;
        public Position pos;
        public Rotation rot;
        public Scale scale;
    }
}