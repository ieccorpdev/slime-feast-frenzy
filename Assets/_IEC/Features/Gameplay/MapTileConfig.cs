using System;
using System.Collections.Generic;
using _IEC.Features.ExportLevel.OriginalLevel;
using Sirenix.OdinInspector;
using UnityEngine;
using GroundElement = IEC.GroundElement;

namespace _IEC.Features.Gameplay
{
    [CreateAssetMenu(fileName = "MapTileConfig", menuName = "Game/MapTileConfig")]
    public class MapTileConfig : ScriptableObject
    {
        [ListDrawerSettings(NumberOfItemsPerPage = 3)]
        public List<MapTilePair> mapTilePairs;
    }

    [Serializable]
    public class MapTilePair
    {
        public string id; 
        [PreviewField] public Sprite tileSprite;
        [PreviewField] public GroundElement groundElement;
        public float rotation;

        [Button]
        public void GetId()
        {
            id = groundElement.Id;
        }
    }
}