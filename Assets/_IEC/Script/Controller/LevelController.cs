using System.Collections;
using UnityEngine;

namespace IEC
{
    public class LevelController : _BaseBehaviour, IController, IEventAwake, IEventListener
    {
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
        }

        private IEnumerator IESpawnLevel()
        {
            OnResetAll();
            Game.levelSeed = PlayerData.level;

            Game.isStartLevelEffectType = Random.Range(0, 2);

            if (Game.isHaveBossLimitMoveLevels)
            {
                Game.isBossLevelLimitMoves = (Game.levelSeed % 5) == 0;
            }
            bool isBossMixColor = false;

            if (Game.isHaveBossMixColorLevels)
            {
                if (Game.levelSeed >= 15)
                {
                    isBossMixColor = (Game.levelSeed % 5) == 0;
                }
            }

            var levelSpawn = Game.isCheatLevel ? new LevelSpawn(Game.levelSeed, false) : GetLevelSpawn(Game.levelSeed);
            if (Game.isCheatLevel)
            {
                isBossMixColor = false;
            }

            SetupEnviroment();

            string[] leveText;
            int levelBoss = 0;
            if (isBossMixColor)
            {
                var matrix = Mathf.Clamp(4 + Game.levelSeed / 20, 4, 9);

                levelBoss = LevelHelper.GetMixLevelSpawn(matrix, Game.levelSeed);
                leveText = Game.levelMixColorDatas[levelBoss].Trim().Split(':');
            }
            else
            {
                leveText = Game.levelDatas[levelSpawn.level].Trim().Split(':');
            }

            string[] hintText;
            if (isBossMixColor)
            {
                hintText = Game.hintMixColorDatas[levelBoss].Trim().Split(':');
            }
            else
            {
                hintText = Game.hintDatas[levelSpawn.level].Trim().Split(':');
            }

            var strGridHints = hintText[3];

            var strMode = leveText[1];
            var strGrid = leveText[3];

            int size = int.Parse(leveText[0].Substring(leveText[0].Length - 1)) + 2;

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
                if (!Game.isMultiColor)
                {
                    Game.classicColor = RandomHelper.Range(0, 12);
                }
            }

            SpawnWallAll(levelConfig);
            SpawnGridAll(levelConfig, levelSpawn.isRandomRotate);
            yield return null;

            GameController.SetupCamera(levelSpawn, size);

            Hint.CheckSoulution();

            Game.totalMove = Mathf.RoundToInt(Game.totalMove * 1.3f);
            Game.moveRemains = Game.totalMove;
            if (Game.isBossLevelLimitMoves)
            {
                UI.mainUIGroup.UpdateMove();
            }

            UIController.SetLevel(Game.isBossLevelLimitMoves);

            var delayTime = Game.isStartLevelEffect ? 1.5f : 0f;
            EventController.RaiseEventDelay(delayTime, EventList.GamePlay.GAME_START_PLAY);
        }

        private void SetupEnviroment()
        {
            var listTheme = Config.shopConfig.listEnviromentConfigs;
            var index = Game.themeIndex % listTheme.Count;
            Game.enviromentConfig = Config.shopConfig.listEnviromentConfigs[index];

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

            var bgConfig = Config.shopConfig.listSpriteBackgrounds[Game.backGroundIndex];
            GamePlay.SetBackground(bgConfig);
        }

        private void SpawnGridAll(LevelConfig levelConfig, bool isRandomRot)
        {
            var size = levelConfig.size - 2;
            var startPosX = (size - 1) / -2f;
            var startPosZ = (size - 1) / 2f;

            int gridSeed = Game.levelSeed;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var posSpawn = new Vector3(startPosX + j, 0f, startPosZ - i);
                    var row = i + 1;
                    var col = j + 1;
                    var gridData = levelConfig.gridTypes[row, col].Trim();
                    SpawnGrid(GamePlay.transGridParent, posSpawn, row, col, gridData, levelConfig.gridHints[row, col], isRandomRot, gridSeed);
                    gridSeed += 1;
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
                    gridColor = int.Parse(gridData.Substring(3, 1));
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

            if (Game.isBossLevelLimitMoves)
            {
                Game.totalMove += LevelHelper.CalculatorMissingRot(gridRotCurrent, gridRotTrue);
            }
        }

        private void SpawnWallAll(LevelConfig levelConfig)
        {
            var size = levelConfig.size - 2;
            var startPos = (size - 1) / -2f;
            var wallPos = (size / 2f) + .18f;

            GamePlay.listWallCorners.ForEach(x => x.OnInit(Game.enviromentConfig.cornerMat));

            GamePlay.listWallCorners[0].transform.localPosition = new Vector3(-wallPos, 0f, wallPos);
            GamePlay.listWallCorners[1].transform.localPosition = new Vector3(wallPos, 0f, wallPos);
            GamePlay.listWallCorners[2].transform.localPosition = new Vector3(-wallPos, 0f, -wallPos);
            GamePlay.listWallCorners[3].transform.localPosition = new Vector3(wallPos, 0f, -wallPos);

            GamePlay.listWallCorners.ForEach(x => x.OnTweenMove());


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
                var posSpawn = new Vector3(startPos + (i - 1) * 1, 0f, 0f);
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
            if (level > Game.levelDatas.Length)
            {
                return new LevelSpawn((level + 99) % Game.levelDatas.Length, false);
            }
            else
            {
                if (Game.isLevelDifficultyV2 || Game.isLevelDifficultyV3 || Game.isLevelDifficultyV4)
                {
                    if (level > 2 && level <= GetConditionLevelDifficulty())
                    {
                        var levelSpawn = int.Parse(Game.levelDifficulty[levelIndex]);
                        bool isRotate = false;
                        if (levelSpawn < 0)
                        {
                            isRotate = true;
                            var matrix = int.Parse(Game.dataLevelDifficulty[levelIndex]);

                            levelSpawn = LevelHelper.GetLevelSpawn(matrix, level);
                        }
                        return new LevelSpawn(levelSpawn, isRotate);
                    }
                }
                return new LevelSpawn((level - 1) % Game.levelDatas.Length, false);
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
