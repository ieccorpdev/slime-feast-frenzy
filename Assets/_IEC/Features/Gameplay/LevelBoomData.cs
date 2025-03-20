using System;
using System.Collections.Generic;
using _IEC.Features.ExportLevel.OriginalLevel;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    [System.Serializable]
    public class LevelBoomData
    {
        public int number;
        public float scaleMin;
        public float scaleMax;
        public int seedScale;
        public int seedPosition;

        [Header("New")]
        public bool isOverrideBombData;
        public List<BombData> listBombDatas = new List<BombData>();
        
        public LevelBoomData()
        {
            number = 0;
            scaleMin = 0;
            scaleMax = 0;
            seedScale = 0;
            seedPosition = 0;
        }
        
        public LevelBoomData(int number, float scaleMin, float scaleMax, int seedScale, int seedPosition)
        {
            this.number = number;
            this.scaleMin = scaleMin;
            this.scaleMax = scaleMax;
            this.seedScale = seedScale;
            this.seedPosition = seedPosition;
        }
        
        public LevelBoomData(LevelBoomData levelBoomData)
        {
            this.number = levelBoomData.number;
            this.scaleMin = levelBoomData.scaleMin;
            this.scaleMax = levelBoomData.scaleMax;
            this.seedScale = levelBoomData.seedScale;
            this.seedPosition = levelBoomData.seedPosition;
        }
        
        public LevelBoomData(bool isOverrideBombData, List<BombData> listBombDatas)
        {
            this.isOverrideBombData = isOverrideBombData;
            this.listBombDatas = listBombDatas;
        }
    }

    [Serializable]
    public class BombData
    {
        public Position pos;
        public Scale scale;
    }
}