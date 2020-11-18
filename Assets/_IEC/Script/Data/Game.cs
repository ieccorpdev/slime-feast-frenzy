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

        public bool isPlaying, isMultiColor, isHaveWaterConnection, isTutorial, isAllowHint;

        public int size, seed, timePlay, gridTutId, gridRotCount, tutCount;

        public int maxGrassOnWall;

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

        [Header("AB Test")]
        public bool isHaveMixColor;
        public bool isFasterRotate;
        public bool isHaveSetting;
        public bool isHaveTutorital;
        public bool isHaveHint;

        //Prepare
        public bool isLevelDifficult;

        public BaseComponent GetGrid(int row, int col, int color, bool isRequireTrueColor = true)
        {
            if (row < 0 || row > size - 1) return null;
            if (col < 0 || col > size - 1) return null;

            var grid = grids[row, col];
            if(grid != null)
            {
                if (isRequireTrueColor)
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
                    return grid;
                }

            }
            else
            {
                return null;
            }
        }
    }

    public enum GridType
    {
        NONE,
        WALL,
        GRID_SOURCE,
        GRID_TARGET,
        GRID_STRAIGHT,
        GRID_CROSS,
        GRID_CURVE
    }
}