using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class LevelController : _BaseBehaviour, IController, IEventAwake, IEventListener
    {
        private static int[] matrix3 = new int[] { 0, 1, 2, 3, 5, 6, 8, 9, 10, 12 };
        private static int[] matrix4 = new int[] { 13, 16, 17, 20, 21, 23 };

        private List<int> m_ListLevelIndex = new List<int>();

        private List<string[,]> m_ListLevel = new List<string[,]>();

        private List<IslandComponent> m_ListIsland = new List<IslandComponent>();
        private List<BigSquareComponent> m_ListBigSquare = new List<BigSquareComponent>();

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourAwake()
        {
            PrefabHelper.SetUpPrefab(ComponentConfig.sourceObj);
            PrefabHelper.SetUpPrefab(ComponentConfig.targetObj);
            PrefabHelper.SetUpPrefab(ComponentConfig.wallObj);
            PrefabHelper.SetUpPrefab(ComponentConfig.cornerObj);
            PrefabHelper.SetUpPrefab(ComponentConfig.straightObj);
            PrefabHelper.SetUpPrefab(ComponentConfig.crossObj);
            PrefabHelper.SetUpPrefab(ComponentConfig.curveObj);
            PrefabHelper.SetUpPrefab(ComponentConfig.noneObj);

            PrefabHelper.SetUpPrefab(ComponentConfig.islandObj);
            PrefabHelper.SetUpPrefab(ComponentConfig.bigSquareObj);
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent) 
            {
                case EventList.GamePlay.GAME_SPAWN_LEVEL:
                    StartCoroutine(IESpawnLevel());
                    break;
            }
        }

        private void OnResetAll()
        {
            GamePlay.listWallCorners.ForEach(x => x.OnDestroyMe());
            Game.OnResetAll();
            UIController.OnResetAll();
            Sound.SetupAudio();

            m_ListIsland.ForEach(x => x.OnDestroyMe());
            m_ListIsland.Clear();

            m_ListBigSquare.ForEach(x => x.OnDestroyMe());
            m_ListBigSquare.Clear();
        }

        private IEnumerator IESpawnLevel()
        {
            OnResetAll();
            Game.levelSeed = PlayerData.level;

            Game.isStartLevelEffectType = Random.Range(0, 2);
            SetupEnviroment();

            bool isBossMixColor = false;
            bool isMergeLevelV5 = false;

            HolePos holePos = HolePos.None;

            if (Game.isHaveBossMixColorLevels)
            {
                if (Game.levelSeed >= 15)
                {
                    isBossMixColor = (Game.levelSeed % 5) == 0;
                }
            }

            if (Game.isHaveBossLimitMoveLevels || Game.isHaveBossTimerLevels)
            {
                if (Game.levelSeed >= 20)
                {
                    if((Game.levelSeed % 10) == 0)
                    {
                        Game.isBossLevel = true;
                        isBossMixColor = false;
                    }
                }
            }

            var levelSpawn = Game.isCheatLevel ? new LevelSpawn(Game.levelSeed, false) : GetLevelSpawn(Game.levelSeed);
            if (Game.isCheatLevel)
            {
                isBossMixColor = false;
            }

            if (Game.isLevelDifficultyV5)
            {
                if ((Game.levelSeed % 10) == 0)
                {
                    isMergeLevelV5 = true;
                    isBossMixColor = false;

                    levelSpawn.isRandomRotate = true;

                    holePos = (HolePos)RandomHelper.RangeWithSeed(Game.levelSeed, 1, 5);
                }
            }

            var sizeLevelMerge = 6;

            if (Game.levelSeed > 30)
            {
                sizeLevelMerge = 8;
            }

            string[] levelText;
            int levelBoss = 0;

            if (isMergeLevelV5)
            {
                levelText = GetMergeLevel(sizeLevelMerge, Game.levelSeed);
            }
            else
            {
                if (isBossMixColor)
                {
                    var matrix = Mathf.Clamp(4 + Game.levelSeed / 20, 4, 9);

                    levelBoss = LevelHelper.GetMixLevelSpawn(matrix, Game.levelSeed);
                    levelText = Config.ListLevelsMixColorsConfig.listText[levelBoss].Trim().Split(':');
                }
                else
                {
                    levelText = Config.ListLevelRemoveMixColorConfig.listText[levelSpawn.level].Trim().Split(':');
                }
            }

            string[] hintText;

            if (isMergeLevelV5)
            {
                hintText = levelText;
            }
            else
            {
                if (isBossMixColor)
                {
                    hintText = Config.ListSolutionMixColorsConfig.listText[levelBoss].Trim().Split(':');
                }
                else
                {
                    hintText = Config.ListSolutionRemoveMixColorConfig.listText[levelSpawn.level].Trim().Split(':');
                }
            }

            var strGridHints = hintText[3];

            var strMode = levelText[1];
            var strGrid = levelText[3];

            int size = int.Parse(levelText[0].Substring(levelText[0].Length - 1)) + 2;

            var gridsData = strGrid.Trim().Split('_');
            string[,] grids = LevelHelper.GetGridValue(gridsData, size, LevelHelper.IsLoopLevel(strMode));

            var gridHintDatas = strGridHints.Trim().Split('_');
            string[,] gridHints = LevelHelper.GetGridValue(gridHintDatas, size, LevelHelper.IsLoopLevel(strMode));

            LevelConfig levelConfig = new LevelConfig();
            levelConfig.SetSize(size, grids, gridHints);
            Game.size = size;
            Game.maxGrassOnWall = 15 - size;
            Game.grids = new BaseComponent[size, size];

            Game.isMultiColor = LevelHelper.IsMultiColor(strMode);

            if (Game.isRandomColorClassicMode)
            {
                Game.classicColor = RandomHelper.Range(0, 12);
            }

            var rotIndex = levelSpawn.isRandomRotate ? RandomHelper.RangeWithSeed(Game.levelSeed, 1, 4) : 0;

            Game.isCamRotated = (rotIndex == 1) || (rotIndex == 3);

            GameController.SetupCamera(size, rotIndex);

            SpawnWallAll(levelConfig);
            SpawnGridAll(levelConfig, levelSpawn.isRandomRotate, holePos);

            if (isMergeLevelV5)
            {
                var isLandSize = sizeLevelMerge / 2;
                var islandObj = PrefabHelper.Clone(ComponentConfig.islandObj);
                var isLandComp = islandObj.GetComponent<IslandComponent>();

                isLandComp.OnInit(holePos, isLandSize);
                m_ListIsland.Add(isLandComp);
            }

            yield return null;

            Hint.CheckSoulution();

            Game.totalMove = Mathf.RoundToInt(Game.totalMove * 1.3f);
            Game.moveRemains = Game.totalMove;
            if (Game.isBossLevel)
            {
                UI.mainUIGroup.UpdateMove();
                Game.timeMaxBossLevel = Game.GetTimerLevel(size - 2);
                Game.timeShowRVAddTime = (int)(Game.timeMaxBossLevel * .3f);
                Game.timeLeftBossLevel = Game.timeMaxBossLevel;
            }

            var level = PlayerData.level;
            bool isHaveFixedOrOneWayBlock = true;

            if (Game.isHaveBigSquare)
            {
                if(level > 5)
                {
                    var seed = level;
                    var isHaveBigSquare = RandomHelper.RangeWithSeed(seed, 0, 2) == 0;
                    seed += 1;
                    if (isHaveBigSquare)
                    {
                        isHaveFixedOrOneWayBlock = false;

                        var maxSquareSize = MathHelper.ClampInt(size - 2, 3, 6);
                        var sizeBigSquare = RandomHelper.RangeWithSeed(seed, 2, maxSquareSize);
                        seed += 1;
                        var maxIndex = size - sizeBigSquare;

                        var radRow = RandomHelper.RangeWithSeed(seed, 1, maxIndex);
                        seed += 1;
                        var radCol= RandomHelper.RangeWithSeed(seed, 1, maxIndex);

                        BaseComponent[,] gridMerge = new BaseComponent[sizeBigSquare, sizeBigSquare];

                        for (int i = 0; i < sizeBigSquare; i++)
                        {
                            for (int j = 0; j < sizeBigSquare; j++)
                            {
                                gridMerge[i, j] = Game.grids[i + radRow, j + radCol];
                            }
                        }

                        var bigSquareObj = PrefabHelper.Clone(ComponentConfig.bigSquareObj);
                        bigSquareObj.transform.SetParent(GamePlay.groupBigSquare);
                        var bigSquareComp = bigSquareObj.GetComponent<BigSquareComponent>();

                        bigSquareComp.OnInit(gridMerge, radRow, radCol);

                        m_ListBigSquare.Add(bigSquareComp);
                    }
                }
            }

            if (isHaveFixedOrOneWayBlock)
            {
                bool isHaveFixedBlock = Game.isHaveFixedBlock;
                if (level > 10)
                {
                    if (Game.isHaveOneWayBlock)
                    {
                        if ((level % 5) != 0)
                        {
                            isHaveFixedBlock = false;
                            SetFixedGrid(level, Game.size - 2, false);
                        }
                    }

                    SetFixedGrid(level + 1, Game.size - 2, true);
                }
            }

            UIController.SetLevel(Game.isBossLevel);
            UIController.UpdateTimerLevel();


            Game.listGrids.ForEach(x => x.BeforeInit());
            GamePlay.listWallCorners.ForEach(x => x.OnTweenMove());

            if (Game.isStartLevelEffect)
            {
                Game.listGrids.ForEach(x => x.OnTweenMove(true));
                m_ListBigSquare.ForEach(x => x.OnTweenMove());
            }

            var maxTime = (Config.startEffectMoveTime * 2f) + ((size - 2) * .1f) + .1f;
            var delayTime = Game.isStartLevelEffect ? maxTime : 0f;
            EventController.RaiseEventDelay(delayTime, EventList.GamePlay.GAME_START_PLAY);
        }

        private void SetFixedGrid(int seed, int size, bool isFixedBlock)
        {
            var totalLockGrid = RandomHelper.RangeWithSeed(seed, 1, size);

            var listFixedGrid = GetListFixedBlocks(seed, totalLockGrid, Game.listTiles.Count);
            var totalTile = listFixedGrid.Count;

            for (int i = 0; i < totalTile; i++)
            {
                if (isFixedBlock)
                {
                    Game.listTiles[listFixedGrid[i]].SetLock(true);
                }
                else
                {
                    Game.listTiles[listFixedGrid[i]].OnSetActiveArrow();
                }
                
            }
        }

        private List<int> m_TempListIndex = new List<int>();

        private List<int> GetListFixedBlocks(int level, int count, int totalGrid)
        {
            m_TempListIndex.Clear();

            for (int i = 0; i < count; i++)
            {
                var tileIndex = MathHelper.GetRandomIndexByList(m_TempListIndex, totalGrid, level);
                m_TempListIndex.Add(tileIndex);
            }

            return m_TempListIndex;
        }

        private string[] GetMergeLevel(int size, int seed)
        {
            m_ListLevelIndex.Clear();
            m_ListLevel.Clear();

            var matrix = (size == 6) ? matrix3 : matrix4;
            var halfSize = size / 2;
            for (int i = 0; i < 4; i++)
            {
                var index = MathHelper.GetRandomIndexByList(m_ListLevelIndex, matrix.Length, seed);
                m_ListLevelIndex.Add(index);
                seed += 1;
            }

            for (int i = 0; i < 4; i++)
            {
                var levelIndex = matrix[m_ListLevelIndex[i]];

                var levelData = Config.ListSolutionRemoveMixColorConfig.listText[levelIndex].Trim().Split(':');
                var gridDatas = levelData[3].Trim().Split('_');

                var gridsLevel = LevelHelper.SplitLevel(gridDatas, halfSize);
                m_ListLevel.Add(gridsLevel);
            }

            var gridsNewLevel = LevelHelper.MergeLevel(m_ListLevel, size);

            return LevelHelper.GetNewlevel(size, gridsNewLevel);
        }

        private void SetupEnviroment()
        {
            SetupTheme(false);
        }

        public void SetupTheme(bool isUpdateNow)
        {
            Game.listTerrainType.Clear();
            var listTheme = ShopConfig.listThemeConfigs;
            var index = PlayerData.themeSelected % listTheme.Count;
            Game.enviromentConfig = ShopConfig.listThemeConfigs[index];

            if(Game.isChangeGridStatus || Game.isGreenifyHydrated)
            {
                if (Game.isChangeGridStatus)
                {
                    Game.listTerrainType.Add(Game.enviromentConfig.listTerrains[0]);
                    Game.listTerrainType.Add(Game.enviromentConfig.listTerrains[1]);
                }
                else
                {
                    Game.listTerrainType.Add(ShopConfig.listThemeConfigs[0].listTerrains[1]);
                    Game.listTerrainType.Add(Game.enviromentConfig.listTerrains[1]);
                }
            }
            else
            {
                var type0 = RandomHelper.Range(0, 2);
                if (type0 == 0)
                {
                    Game.listTerrainType.Add(Game.enviromentConfig.listTerrains[0]);
                    Game.listTerrainType.Add(Game.enviromentConfig.listTerrains[1]);
                }
                else
                {
                    Game.listTerrainType.Add(Game.enviromentConfig.listTerrains[1]);
                    Game.listTerrainType.Add(Game.enviromentConfig.listTerrains[0]);
                }
            }


            if (isUpdateNow)
            {
                OnUpdateTheme();
                GamePlay.listWallCorners.ForEach(x => x.OnSetupMat(Game.enviromentConfig.cornerMat));
            }
        }

        private void OnUpdateTheme()
        {
            Game.listGrids.ForEach(x => x.OnUpdateSetTerrain());
        }

        private void SpawnGridAll(LevelConfig levelConfig, bool isRandomRot, HolePos holePos)
        {
            var size = levelConfig.size - 2;
            var startPosX = (size - 1) / -2f;
            var startPosZ = (size - 1) / 2f;

            var halfSize = size / 2;

            int gridSeed = Game.levelSeed;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (IsBreak(holePos, halfSize, i, j)) continue;

                    var row = i + 1;
                    var col = j + 1;

                    var posSpawn = new Vector3(startPosX + j, 20f, startPosZ - i);
                    var gridData = levelConfig.gridTypes[row, col].Trim();
                    SpawnGrid(GamePlay.transGridParent, posSpawn, row, col, gridData, 
                        levelConfig.gridHints[row, col], isRandomRot, gridSeed);
                    gridSeed += 1;
                }
            }
        }

        private bool IsBreak(HolePos holePos, int halfSize, int row, int col)
        {
            if (holePos == HolePos.None)
            {
                return false;
            }
            else
            {
                if(holePos == HolePos.TopLeft)
                {
                    return (row < halfSize && col < halfSize);
                }
                else if (holePos == HolePos.TopRight)
                {
                    return (row < halfSize && col >= halfSize);
                }
                else if (holePos == HolePos.BotLeft)
                {
                    return (row >= halfSize && col < halfSize);
                }
                else
                {
                    return (row >= halfSize && col >= halfSize);
                }
            }
        }

        private void SpawnGrid(Transform parent, Vector3 posSpawn, int row, int col,
            string gridData, int gridRotTrue, bool isRandomRot, int seed)
        {
            var gridType = int.Parse(gridData.Substring(0, 2));
            var gridRotCurrent = int.Parse(gridData.Substring(2, 1));

            if (isRandomRot)
            {
                gridRotCurrent = RandomHelper.RangeWithSeed(seed, 0, 4);
            }

            var gridColor = 0;
            var gridStyle = 0;

            var grid = PrefabHelper.Clone(LevelHelper.GetGridObj(ComponentConfig, gridType), parent).transform;
            var gridComp = grid.GetComponent<BaseComponent>();

            if (LevelHelper.IsSourceGird(gridType) || LevelHelper.IsTargetGrid(gridType))
            {
                if (Game.isMultiColor)
                {
                    if(gridData.Length > 3)
                    {
                        gridColor = int.Parse(gridData.Substring(3, 1));
                    }
                }
            }

            grid.gameObject.name = "Row " + row + " | Col " + col;
            grid.localPosition = posSpawn;
            grid.localRotation = Quaternion.identity;
            grid.localScale = Vector3.one;

            if (LevelHelper.IsSourceGird(gridType))
            {
                var sourceComp = grid.GetComponent<GridSourceComponent>();
                Game.listSource.Add(sourceComp);
                gridStyle = LevelHelper.GetSourceStyle(gridType);
            }

            if (LevelHelper.IsTargetGrid(gridType))
            {
                var targetComp = grid.GetComponent<GridTargetComponent>();
                Game.listTarget.Add(targetComp);
                gridStyle = LevelHelper.GetTargetStyle(gridType);
            }

            gridComp.OnInit(LevelHelper.GetType(gridType), row, col, gridRotTrue, gridRotCurrent, gridColor, gridStyle);

            Game.grids[row, col] = gridComp;
            Game.listGrids.Add(gridComp);

            bool isFixedBlock = Game.isHaveFixedBlock;

            if (Game.isHaveOneWayBlock)
            {
                if (gridComp.GridType == GridType.GRID_CURVE || gridComp.GridType == GridType.GRID_STRAIGHT)
                {
                    Game.listTiles.Add(gridComp);
                }
            }
            else
            {
                if (isFixedBlock)
                {
                    Game.listTiles.Add(gridComp);
                }
            }

            if (Game.isBossLevel)
            {
                Game.totalMove += LevelHelper.CalculatorMissingRot(gridRotCurrent, gridRotTrue);
            }
        }

        private void SpawnWallAll(LevelConfig levelConfig)
        {
            var size = levelConfig.size - 2;
            var startPos = (size - 1) / -2f;
            var wallPos = (size / 2f) + .18f;

            GamePlay.listWallCorners.ForEach(x => x.OnSetupMat(Game.enviromentConfig.cornerMat));

            GamePlay.listWallCorners[0].transform.localPosition = new Vector3(-wallPos, 0f, wallPos);
            GamePlay.listWallCorners[1].transform.localPosition = new Vector3(wallPos, 0f, wallPos);
            GamePlay.listWallCorners[2].transform.localPosition = new Vector3(-wallPos, 0f, -wallPos);
            GamePlay.listWallCorners[3].transform.localPosition = new Vector3(wallPos, 0f, -wallPos);

            GamePlay.wallParents[0].localPosition = new Vector3(0f, 0f, wallPos);
            SpawnWall(GamePlay.wallParents[0], size, startPos, levelConfig.topWall, true, 0, false);

            GamePlay.wallParents[1].localPosition = new Vector3(0f, 0f, -wallPos);
            SpawnWall(GamePlay.wallParents[1], size, startPos, levelConfig.botWall, true, size + 1, true);

            GamePlay.wallParents[2].localPosition = new Vector3(-wallPos, 0f, 0f);
            SpawnWall(GamePlay.wallParents[2], size, startPos, levelConfig.leftWall, false, 0, false);

            GamePlay.wallParents[3].localPosition = new Vector3(wallPos, 0f, 0f);
            SpawnWall(GamePlay.wallParents[3], size, startPos, levelConfig.rightWall, false, size + 1, true);
        }

        private void SpawnWall(Transform parent, int count, float startPos, int[] walls, bool isBotTop, int index, bool isReverse)
        {
            for (int i = 1; i <= count; i++)
            {
                var posSpawn = new Vector3(startPos + (i - 1) * 1, 20f, 0f);
                var wallValue = walls[i];

                var wallTrans = PrefabHelper.Clone(ComponentConfig.wallObj, parent).transform;
                var wallComp = wallTrans.GetComponent<WallComponent>();
                wallTrans.localRotation = Quaternion.identity;
                wallTrans.localScale = Vector3.one;
                wallTrans.localPosition = posSpawn;

                var row = isBotTop ? index : i;
                var col = isBotTop ? i : index;

                wallTrans.gameObject.name = "Row " + row + " | Col " + col;

                wallComp.OnInit(GridType.WALL, row, col);
                wallComp.SetupWall(wallValue == 2, isBotTop, isReverse);

                Game.grids[row, col] = wallComp;
                Game.listGrids.Add(wallComp);
            }
        }


        private LevelSpawn GetLevelSpawn(int level)
        {
            var levelIndex = level - 1;
            var count = Config.ListLevelRemoveMixColorConfig.totalItem;
            if (level > count)
            {
                return new LevelSpawn((level + 99) % count, true);
            }
            else
            {
                if (Game.isLevelDifficultyV2 || Game.isLevelDifficultyV3 || Game.isLevelDifficultyV4)
                {
                    if (level > 2 && level <= GetConditionLevelDifficulty())
                    {
                        var levelSpawn = int.Parse(Config.LevelDifficutyV4Config.listText[levelIndex]);
                        bool isRotate = false;
                        if (levelSpawn < 0)
                        {
                            isRotate = true;
                            var matrix = int.Parse(Config.DataDifficutyV4Config.listText[levelIndex]);

                            levelSpawn = LevelHelper.GetLevelSpawn(matrix, level);
                        }
                        return new LevelSpawn(levelSpawn, isRotate);
                    }
                }
                return new LevelSpawn((level - 1) % count, false);
            }
        }

        private int GetConditionLevelDifficulty()
        {
            if (Game.isLevelDifficultyV3)
            {
                return 110;
            }
            else if (Game.isLevelDifficultyV4)
            {
                return 115;
            }
            else
            {
                return 100;
            }
        }

    }
}
