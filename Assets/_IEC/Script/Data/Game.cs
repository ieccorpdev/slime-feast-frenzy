using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public enum GameState
    {
        Playing, Win, Lose
    }

    /// <summary>
    /// object contain game session data (no save)
    /// </summary>
    [System.Serializable]
    public class Game
    {
        public bool isDebugReset;
        public int emptyData;

        public float timeDelayFullAds;

        public System.DateTime timeStartLevel;
        public System.DateTime timeEndLevel;
        public int TimeFinishLevel => (int)(timeEndLevel - timeStartLevel).TotalSeconds;

        public GameState gameState = GameState.Playing;

        public bool IsWin => gameState == GameState.Win;

        public void SetGameState(GameState state)
        {
            gameState = state;
        }

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