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
    }

    public enum WallType
    {
        TOP,
        BOT,
        LEFT,
        RIGHT
    }
}