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

        public bool isCheatLevel;

        public bool isShowPopup;

        public GameState GameState;

        public void SetGameState(GameState state)
        {
            GameState = state;
        }

        public bool isPlaying => GameState == GameState.Playing;

        public bool isBlockButton => GameState != GameState.Playing || isWaitAfterRV;

        public bool isWaitAfterRV;

        public bool isMultiColor, isTutorial, isAllowHint, isGameWin;

        public int size, levelSeed, timePlay, gridTutId, gridRotCount, tutCount;

        public int maxGrassOnWall, classicColor;

        public int goldReward;

        public RotTutorialConfig rotTutConfig;

        public BaseComponent[,] grids;

        public List<GridSourceComponent> listSource = new List<GridSourceComponent>();
        public List<GridTargetComponent> listTarget = new List<GridTargetComponent>();
        public List<BaseComponent> listGrids = new List<BaseComponent>();
        public List<BaseComponent> listTiles = new List<BaseComponent>();

        public List<BaseComponent> listHints = new List<BaseComponent>();
        public int maxHintIndex;

        public List<Material> listTerrainType = new List<Material>();

        //Boss level
        public bool isBossLevel;
        public int totalMove, moveRemains;
        public int moveCount;

        public Stack<RotConfig> listUndo = new Stack<RotConfig>();
        public int undoRemains, hintRemains;

        public float timeLeftBossLevel, timeMaxBossLevel, timeShowRVAddTime;
        

        public ThemeConfig enviromentConfig;

        public int isStartLevelEffectType;

        public bool isCamRotated;

        public InterDelayConfig interDelayConfig;

        public float timeLeftAddGold;

        public float DelayInterstitial
        {
            get
            {
                if (!isHaveDynamicInter) return timeDelayFullAds;
                var showInterCount = Pool.Get<PlayerSecret>().GetNumberInterShow();
                if (interDelayConfig.inter120CountRequire != 0 && showInterCount >= interDelayConfig.inter120CountRequire) return 120f;
                if (interDelayConfig.inter90CountRequire != 0 && showInterCount >= interDelayConfig.inter90CountRequire) return 90f;
                if (interDelayConfig.inter60CountRequire != 0 && showInterCount >= interDelayConfig.inter60CountRequire) return 60f;
                if (interDelayConfig.inter45CountRequire != 0 && showInterCount >= interDelayConfig.inter45CountRequire) return 45f;
                return 30f;
            }
        }

        public bool IsAllowUndo()
        {
            var count = listGrids.Count;
            for (int i = 0; i < count; i++)
            {
                var grid = listGrids[i];
                if (!grid.IsAllowRotation) return false;
            }
            return true;
        }

        [Header("AB Test")]
        public bool isHaveNomalLevelsMixColor;
        public bool isFasterRotate;
        public bool isHaveSetting;
        public bool isHaveTutorital;
        public bool isHaveHint;
        public bool isReduceHintGrid;
        public bool isFixedHintGrid;
        public bool isFixLevelDifficulty;
        public bool isLevelDifficultyV2;
        public bool isRandomColorClassicMode;
        
        public bool isLevelDifficultyV3;
        public bool isHaveNewWaterSkin;
        public bool isHaveBossLimitMoveLevels;
        
        public bool isHaveNotification;

        public int themeDefault;
        public int backGroundDefault;

        public bool isHaveBossMixColorLevels;
        public bool isLevelDifficultyV4;

        public bool isNewUI;
        public bool isHaveNewTrees;
        public bool isStartLevelEffect;
        public bool isHintStyleV2;

        public bool isHintStyleBSP;
        public bool isHaveBossTimerLevels;
        public bool isHaveBossTimerLevelsV2;
        public bool isNewWaterEffect;

        public bool isLevelDifficultyV5;
        public bool isTreeColorV1;
        public bool isHaveUIAnimation;
        public bool isChangeGridStatus;
        public bool isHaveDynamicInter;

        public bool isHaveShop;
        public bool isRootTreeOnlyGreenColor;
        public bool isHaveBossTimerLevelsV3;
        public bool isHaveBossTimerLevelsV4;
        public bool isNoShadow;
        public bool isHintStyleV3;

        public bool isHaveEndGameReward;
        public bool isGreenifyHydrated;
        public bool isRootTreeChangeColorByWater;

        public bool isHaveNoAdsInMainScreen;
        public bool isHaveNoAdsInShopTab;
        public bool isCameraTopView;
        public bool isNewGameOver;
        public bool isHaveConfettiGameOver;

        public bool isHaveFixedBlock;
        public bool isHintFixedBlockStyle;

        public bool isHaveOneWayBlock;
        public bool isHaveBigSquare = true;
        public bool isHaveNoAdsV3;
        public bool isTreeColorV2;


        public bool isLeavesEffect;

        public void OnResetAll()
        {
            listGrids.ForEach(x => x.OnDestroyMe());
            listGrids.Clear();

            listSource.Clear();
            listTarget.Clear();
            listHints.Clear();

            isMultiColor = false;
            isTutorial = false;
            isAllowHint = true;
            isWaitAfterRV = false;
            maxHintIndex = 0;
            SetGameState(GameState.Init);

            totalMove = 0;
            moveRemains = 0;

            listGrids.Clear();
            listUndo.Clear();
            listTiles.Clear();

            isBossLevel = false;
            undoRemains = 2;
            hintRemains = 1;
            moveCount = 0;
        }
        
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

        public void AddUndoData(int row, int col)
        {
            listUndo.Push(new RotConfig(row, col));
        }

        public RotConfig GetLastUndo()
        {
            return listUndo.Pop();
        }

        public int GetTimerLevel(int matrix)
        {
            var time = 30f;
            var count = matrix - 3;
            if (isHaveBossTimerLevelsV3)
            {
                for (int i = 0; i < count; i++)
                {
                    time *= 1.5f;
                }
            }else if (isHaveBossTimerLevelsV4)
            {
                for (int i = 0; i < count; i++)
                {
                    time += 15f;
                }
            }
            else
            {
                time = 90f;
            }
            return Mathf.RoundToInt(time);
        }

    }

    [System.Serializable]
    public struct RotConfig
    {
        public int Row, Col;

        public RotConfig(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }

    public struct LevelSpawn
    {
        public int level;
        public bool isRandomRotate;

        public LevelSpawn(int level, bool isRandomRotate)
        {
            this.level = level;
            this.isRandomRotate = isRandomRotate;
        }
    }

    public enum GameState
    {
        Init,
        Pause,
        Playing,
        GameOver
    }

    public enum HolePos
    {
        None,
        TopLeft,
        TopRight,
        BotLeft,
        BotRight
    }

    public enum IapPackages
    {
        None,
        NoAds
    }
}