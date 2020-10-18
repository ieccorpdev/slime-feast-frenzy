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

    public enum WallType
    {
        TOP,
        BOT,
        LEFT,
        RIGHT
    }
}