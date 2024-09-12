using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    /// <summary>
    /// object contain game session data (no save)
    /// </summary>
    [System.Serializable]
    public class Game : IData
    {
        #region Hybrid

        public HybridGame hybrid = new HybridGame();

        #endregion
        
        public bool isDebugReset;
        public int emptyData;
        public int abVersion;

        public float timeDelayFullAds;

        public System.DateTime timeStartLevel;
        public System.DateTime timeEndLevel;
        public int TimeFinishLevel => (int)(timeEndLevel - timeStartLevel).TotalSeconds;

        public void SetTimeStartLevel()
        {
            timeStartLevel = System.DateTime.Now;
        }

        public void SetTimeEndLevel()
        {
            timeEndLevel = System.DateTime.Now;
        }
    }
}