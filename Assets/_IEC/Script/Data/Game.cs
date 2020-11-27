using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    /// <summary>
    /// object contain game session data (no save)
    /// </summary>
    [System.Serializable]
    public class Game
    {
        public bool isDebugReset;
        public int emptyData;

        public float timeDelayFullAds;

        public bool isShowPopup;

        public bool isBlockButton => !isPlaying || isWaitAfterRV;

        public bool isWaitAfterRV;

        public bool isPlaying, isMultiColor, isTutorial, isAllowHint;

        public int size, seed, timePlay, gridTutId, gridRotCount, tutCount;

        public int maxGrassOnWall, classicColor;

        public RotTutorialConfig rotTutConfig;

        public BaseComponent[,] grids;

        public List<GridSourceComponent> listSource = new List<GridSourceComponent>();
        public List<GridTargetComponent> listTarget = new List<GridTargetComponent>();
        public List<BaseComponent> listGrids = new List<BaseComponent>();

        public List<BaseComponent> listHints = new List<BaseComponent>();
        public int maxHintIndex;

        public List<int> listTerrainType = new List<int>();

        public string[] levelDatas;
        public string[] hintDatas;

        public string[] levelSort1;

        [Header("AB Test")]
        public bool isHaveMixColor;
        public bool isFasterRotate;
        public bool isHaveSetting;
        public bool isHaveTutorital;
        public bool isHaveHint;
        public bool isReduceHintGrid;
        public bool isFixedHintGrid;
        public bool isFixLevelDifficulty;
        public bool isLevelDifficultyV2;
        public bool isRandomColorClassicMode;
        public bool isStartLevelEffect;

        //Prepared
        public bool isLeavesEffect;
        public bool isUIEffect;


        public BaseComponent GetGrid(int row, int col, int color)
        {
            if (row < 0 || row > size - 1) return null;
            if (col < 0 || col > size - 1) return null;

            var grid = grids[row, col];
            if(grid != null)
            {
                if (grid.IsAllowConnect(color))
                {
                    return grid;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}