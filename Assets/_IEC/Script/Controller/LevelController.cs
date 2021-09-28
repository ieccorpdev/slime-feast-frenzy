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
            UIController.OnResetAll();

            GamePlay.listWallCorners.ForEach(x => x.OnDestroyMe());
            Game.OnResetAll();
            Sound.SetupAudio();

            m_ListIsland.ForEach(x => x.OnDestroyMe());
            m_ListIsland.Clear();

            Game.listBigSquare.ForEach(x => x.OnDestroyMe());
            Game.listBigSquare.Clear();
        }

        private IEnumerator IESpawnLevel()
        {
            OnResetAll();
            yield return null;

            Game.levelSeed = PlayerData.level;
            Game.introType = Random.Range(0, 2);
            SetupTheme(false);

            bool isMergeLevel = Game.isLevelDifficultyV5 && (Game.levelSeed % 10) == 0;

            Game.isBossLevel = (Game.isHaveBossLimitMoveLevels || Game.isHaveBossTimerLevels)
                && Game.levelSeed >= 20 && (Game.levelSeed % 10) == 0;

            bool isMixColorLevel = (Game.isHaveMixColorLevel && Game.levelSeed >= 15 && (Game.levelSeed % 5) == 0)
                && !Game.isCheatLevel && !isMergeLevel && !Game.isBossLevel;

            var levelSpawn = Game.isCheatLevel ? new LevelSpawn(Game.levelSeed, false)
                : GetLevelSpawn(Game.levelSeed, isMixColorLevel);

            HolePos holeInBoardPos = HolePos.None;

            if (isMergeLevel)
            {
                levelSpawn.isRandomRotate = true;
                holeInBoardPos = (HolePos)RandomHelper.RangeWithSeed(Game.levelSeed, 1, 5);
            }

            var sizeLevelMerge = Game.levelSeed > 30 ? 8 : 6;
            var levelConfig = GetLevelConfig(isMergeLevel, isMixColorLevel, sizeLevelMerge, levelSpawn.level);

            var rotIndex = 0;
            if (levelSpawn.isRandomRotate)
            {
                rotIndex = RandomHelper.RangeWithSeed(Game.levelSeed, 1, 4);
            }

            Game.isCamRotated = (rotIndex == 1) || (rotIndex == 3);
            GameController.SetupCamera(Game.size, rotIndex);

            SpawnWallAll(levelConfig);
            SpawnGridAll(levelConfig, levelSpawn.isRandomRotate, holeInBoardPos);

            if (isMergeLevel)
            {
                var isLandSize = sizeLevelMerge / 2;
                var islandObj = PrefabHelper.Clone(ComponentConfig.islandObj);
                var isLandComp = islandObj.GetComponent<IslandComponent>();

                isLandComp.OnInit(holeInBoardPos, isLandSize);
                m_ListIsland.Add(isLandComp);
            }

            yield return null;
            Hint.CheckSoulution();
            AfterSpawnLevel();
        }

        private LevelConfig GetLevelConfig(bool isMergeLevel, bool isMixColorLevel, int sizeLevelMerge, int levelSpawn)
        {
            string[] levelText;
            string[] hintText;

            if (isMergeLevel)
            {
                levelText = GetMergeLevel(sizeLevelMerge, Game.levelSeed);
                hintText = levelText;
            }
            else
            {
                if (isMixColorLevel)
                {
                    levelText = Config.ListLevelsMixColorsConfig.listText[levelSpawn].Trim().Split(':');
                    hintText = Config.ListSolutionMixColorsConfig.listText[levelSpawn].Trim().Split(':');
                }
                else
                {
                    levelText = Config.ListLevelRemoveMixColorConfig.listText[levelSpawn].Trim().Split(':');
                    hintText = Config.ListSolutionRemoveMixColorConfig.listText[levelSpawn].Trim().Split(':');
                }
            }

            var strMode = levelText[1];
            var strGrid = levelText[3];

            var strGridHints = hintText[3];

            Game.size = int.Parse(levelText[0].Substring(levelText[0].Length - 1)) + 2;
            Game.maxGrassOnWall = 15 - Game.size;
            Game.grids = new BaseComponent[Game.size, Game.size];

            Game.isMultiColor = LevelHelper.IsMultiColor(strMode);

            if (!Game.isMultiColor && Game.isRandomColorClassicMode && !Game.isCheatWaterColor)
            {
                Game.classicColor = RandomHelper.Range(0, 12);
            }

            var isLoopLevel = LevelHelper.IsLoopLevel(strMode);

            var gridsText = strGrid.Trim().Split('_');
            string[,] gridData = LevelHelper.GetGridValue(gridsText, Game.size, isLoopLevel);

            var gridHintText = strGridHints.Trim().Split('_');
            string[,] gridHintData = LevelHelper.GetGridValue(gridHintText, Game.size, isLoopLevel);

           return new LevelConfig(Game.size, gridData, gridHintData);
        }

        private void AfterSpawnLevel()
        {
            if (Game.isBossLevel)
            {
                Game.totalMove = Mathf.RoundToInt(Game.totalMove * 1.3f);
                Game.moveRemains = Game.totalMove;

                UI.mainUIGroup.UpdateMove();
                Game.timeMaxBossLevel = Game.GetTimerLevel(Game.size - 2);
                Game.timeShowRVAddTime = (int)(Game.timeMaxBossLevel * .3f);
                Game.timeLeftBossLevel = Game.timeMaxBossLevel;
            }

            var level = PlayerData.level;
            bool isHaveFixedOrOneWayBlock = (Game.isHaveFixedBlock || Game.isHaveOneWayBlock) && !Game.isHaveBigSquare;

            if (Game.isHaveBigSquare)
            {
                if (level > 5)
                {
                    var seed = level;
                    var isHaveBigSquare = RandomHelper.RangeWithSeed(seed, 0, 2) == 0;
                    seed += 1;
                    if (isHaveBigSquare)
                    {
                        var maxSquareSize = MathHelper.ClampInt(Game.size - 2, 3, 6);
                        var sizeBigSquare = RandomHelper.RangeWithSeed(seed, 2, maxSquareSize);
                        seed += 1;
                        var maxIndex = Game.size - sizeBigSquare;

                        var radRow = RandomHelper.RangeWithSeed(seed, 1, maxIndex);
                        seed += 1;
                        var radCol = RandomHelper.RangeWithSeed(seed, 1, maxIndex);

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

                        Game.listBigSquare.Add(bigSquareComp);
                    }
                }
            }

            if (isHaveFixedOrOneWayBlock)
            {
                if (level > 10)
                {
                    if (Game.isHaveOneWayBlock)
                    {
                        if ((level % 5) != 0)
                        {
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
                Game.listBigSquare.ForEach(x => x.OnTweenMove());
            }
            var maxTime = (Config.startEffectMoveTime * 2f) + ((Game.size - 2) * .1f) + .1f;
            var delayTime = Game.isStartLevelEffect ? maxTime : .1f;
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
                    Game.listTiles[listFixedGrid[i]].SetLock();
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

        public void SetupTheme(bool isUpdateNow)
        {
            Game.listTerrainType.Clear();

            GamePlay.SetBackground(PlayerData.backgroundSelected);

            var themeIndex = PlayerData.themeSelected % ShopConfig.listThemeConfigs.Count;
            Game.enviromentConfig = ShopConfig.listThemeConfigs[themeIndex];

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
            var posY = Game.isStartLevelEffect ? 20f : 0f;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (IsBreak(holePos, halfSize, i, j)) continue;

                    var row = i + 1;
                    var col = j + 1;

                    var posSpawn = new Vector3(startPosX + j, posY, startPosZ - i);
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
                if (holePos == HolePos.TopLeft)
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
                    if (gridData.Length > 3)
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

            var posY = Game.isStartLevelEffect ? 20f : 0f;

            GamePlay.listWallCorners[0].transform.localPosition = new Vector3(-wallPos, posY, wallPos);
            GamePlay.listWallCorners[1].transform.localPosition = new Vector3(wallPos, posY, wallPos);
            GamePlay.listWallCorners[2].transform.localPosition = new Vector3(-wallPos, posY, -wallPos);
            GamePlay.listWallCorners[3].transform.localPosition = new Vector3(wallPos, posY, -wallPos);

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
            var posY = Game.isStartLevelEffect ? 20f : 0f;

            for (int i = 1; i <= count; i++)
            {
                var posSpawn = new Vector3(startPos + (i - 1) * 1, posY, 0f);
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


        private LevelSpawn GetLevelSpawn(int level, bool isMixColorLevel)
        {
            if (isMixColorLevel)
            {
                var leveSize = Mathf.Clamp(4 + level / 20, 4, 9);
                var mixColorlevel = LevelHelper.GetMixLevelSpawn(leveSize, level);
                return new LevelSpawn(mixColorlevel, false);
            }
            else
            {
                var levelIndex = level - 1;
                var count = Config.ListLevelRemoveMixColorConfig.totalItem;
                if (level > count)
                {
                    //return new LevelSpawn((level + 99) % count, true);
                    return new LevelSpawn(RandomHelper.RangeWithSeed(level, 100, count), true);
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
