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

        public bool isPlaying;

        public int size;

        public BaseComponent[,] grids;

        public BaseComponent GetGrid(int row, int col)
        {
            if (row < 0 || row > size - 1) return null;
            if (col < 0 || col > size - 1) return null;

            return grids[row, col];
        }
    }

    public enum GridType
    {
        NONE,
        WALL_TOP,
        WALL_BOT,
        WALL_LEFT,
        WALL_RIGHT,
        GRID_SOURCE,
        GRID_TARGET,
        GRID_STRAIGHT,
        GRID_CROSS,
        GRID_CURVE
    }

    // Wall 1
    // Wall hole 2
    // Grid source 5
    // Grid target 10
    // Grid straight 15
    // Grid cross 16
    // Grid curve 17

    // Color [1, 5]

    // 10_1_1 GridType_Rot_Style_Color
}