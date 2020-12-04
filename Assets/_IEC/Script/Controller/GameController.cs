using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace IEC
{
    public class GameController : _BaseBehaviour, IController, IEventStart, IEventListener, IEventAwake, IEventUpdate
    {
        public Transform[] m_WallParents;   // TOP, BOT, LEFT, RIGHT

        [SerializeField]
        private List<CornerComponent> m_ListWallCorners;   // TOP_LEFT, TOP_RIGHT, BOT_LEFT, BOT_RIGHT
        public Transform m_GridParent;

        private static float RATIO_DEFAULT = 16f / 10;
        private static float RATIO_CAMERA_SIZE_MIN = 1.45f / (21f / 9 - 16f / 10);
        private static float RATIO_CAMERA_SIZE_MAX = 5.6f / (21f / 9 - 16f / 10);

        private float m_ScreenRatio, m_DeltaScreenRatio, m_DeltaSizeMinCam, m_RatioCamSize;

        private Vector3 m_CamDirOrigin, m_CamOriginPos;

        private bool m_IsInitCam;

        private System.DateTime m_TimeStart;

        private List<BaseComponent> m_ListGridHaveWater = new List<BaseComponent>();

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

            m_CamOriginPos = GamePlay.mainCamera.transform.position;
            m_CamDirOrigin = GamePlay.mainCamera.transform.forward;
        }

        public void GetLevelsData()
        {
            var strLevelPath = Game.isHaveMixColor ? "ListLevelFull" : "ListLevelRemoveMixColor";
            var strLevels = Resources.Load<TextAsset>(strLevelPath);

            var strHintPath = Game.isHaveMixColor ? "ListSolutionFull" : "ListSolutionRemoveMixColor";
            var strHints = Resources.Load<TextAsset>(strHintPath);

            var levelDatas = strLevels.text;
            Game.levelDatas = levelDatas.Split(new string[] { "\n", "\r", "\r\n" },
                             System.StringSplitOptions.RemoveEmptyEntries);

            var hintDatas = strHints.text;
            Game.hintDatas = hintDatas.Split(new string[] { "\n", "\r", "\r\n" },
                             System.StringSplitOptions.RemoveEmptyEntries);
        }

        public void GetLevelDifficulty()
        {
            var levelDifficulty = Resources.Load<TextAsset>(Game.isLevelDifficultyV3 ? "LevelDifficutyV3" : "LevelDifficutyV2");
            var levelDifficultyText = levelDifficulty.text;
            Game.levelDifficulty = levelDifficultyText.Split(new string[] { "\n", "\r", "\r\n" },
                             System.StringSplitOptions.RemoveEmptyEntries);

        }

        public void GetLevelsMixColor()
        {
            var strLevels = Resources.Load<TextAsset>("LevelsMixColors");
            var strHints = Resources.Load<TextAsset>("ListSolutionMixColors");

            var levelDatas = strLevels.text;
            Game.levelMixColorDatas = levelDatas.Split(new string[] { "\n", "\r", "\r\n" },
                             System.StringSplitOptions.RemoveEmptyEntries);

            var hintDatas = strHints.text;
            Game.hintMixColorDatas = hintDatas.Split(new string[] { "\n", "\r", "\r\n" },
                             System.StringSplitOptions.RemoveEmptyEntries);


            var dataLevelDifficulty = Resources.Load<TextAsset>(Game.isLevelDifficultyV3 ? "DataDifficutyV3" : "DataDifficutyV2");
            var dataLevelDifficultyText = dataLevelDifficulty.text;
            Game.dataLevelDifficulty = dataLevelDifficultyText.Split(new string[] { "\n", "\r", "\r\n" },
                             System.StringSplitOptions.RemoveEmptyEntries);
        }

        public void BehaviourStart()
        {
            EventController.RaiseEvent(EventList.GamePlay.GAME_SPAWN_LEVEL);
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.GAME_SPAWN_LEVEL:
                    StartCoroutine(SpawnLevel());
                    break;
                case EventList.GamePlay.GAME_CHECK_CONNECT:
                    if (!Game.isPlaying) return;
                    CheckConnect();
                    break;
                case EventList.GamePlay.GAME_START:
                    GameStart();
                    break;
                case EventList.GamePlay.GAME_CHECK_COMPLETE:
                    if (IsComplete())
                    {
                        EventController.RaiseEventDelay(1f, EventList.GamePlay.GAME_OVER_UI, new EventParam(this, true));
                    }
                    break;
                case EventList.GamePlay.GAME_UNDO:
                    OnUndo();
                    break;
            }
        }

        public void BehaviourUpdate(float deltaTime)
        {
            Game.listGrids.ForEach(x => x.OnUpdate(deltaTime));
        }

        private void GameStart()
        {
            if (Game.isFixLevelDifficulty)
            {
                if (PlayerData.level < 11)
                {
                    var count = Game.listHints.Count;
                    var seed = PlayerData.level;
                    var moveCountSpawn = seed * 2;
                    var gridCondition = moveCountSpawn / 2;

                    BaseComponent grid = null;
                    var gridRot = 0;
                    var maxReduce = 0;
                    var minReduce = 0;
                    var reduce = 0;

                    for (int i = 0; i < count; i++)
                    {
                        grid = Game.listHints[i];
                        gridRot = grid.TrueRot;

                        if (moveCountSpawn > 0)
                        {
                            maxReduce = Mathf.Min(moveCountSpawn, 3) + 1;
                            minReduce = (i > gridCondition) ? (Mathf.Max(maxReduce, 1) - 1) : 1;
                            reduce = RandomHelper.RangeWithSeed(seed, minReduce, maxReduce);
                            moveCountSpawn -= reduce;
                            gridRot = LevelHelper.CalculatorAddRot(grid.TrueRot, reduce);
                        }

                        grid.SetTempRot(gridRot);
                    }
                }
            }
             
            Game.listGrids.ForEach(x => x.AfterInit());
            Game.isPlaying = true;
            m_TimeStart = System.DateTime.Now;
            PushPlayLevelEvent();

            EventController.RaiseEvent(EventList.GamePlay.GAME_CHECK_TUTORIAL);
            EventController.RaiseEvent(EventList.GamePlay.GAME_CHECK_CONNECT);
        }

        private IEnumerator SpawnLevel()
        {
            OnClearAll();
            Game.seed = PlayerData.level;
            

            if (Game.isHaveBossLevel)
            {
                Game.isBossLevel = (Game.seed % 5) == 0;
            }

            bool isBossMixColor = false;

            if (Game.isHaveBossLevelsMixColor)
            {
                if(Game.seed >= 15)
                {
                    isBossMixColor = (Game.seed % 5) == 0;
                }
            }

            var levelSpawn = GetLevelSpawn();

            SetupEnviroment();

            string[] leveText;
            int levelBoss = 0;
            if (isBossMixColor)
            {
                //if(Game.seed < Game.dataLevelDifficulty.Length)
                //{
                //    matrix = int.Parse(Game.dataLevelDifficulty[Game.seed]);
                //}

                var matrix = Mathf.Clamp(4 + Game.seed / 20, 4, 9);

                levelBoss = LevelHelper.GetMixLevelSpawn(matrix, Game.seed);
                leveText = Game.levelMixColorDatas[levelBoss].Trim().Split(':');

            }
            else
            {
                leveText = Game.levelDatas[levelSpawn].Trim().Split(':');
            }

            string[] hintText;
            if (isBossMixColor)
            {
                hintText = Game.hintMixColorDatas[levelBoss].Trim().Split(':');
            }
            else
            {
                hintText = Game.hintDatas[levelSpawn].Trim().Split(':');
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
            SpawnGridAll(levelConfig);
            yield return null;

            if (!m_IsInitCam)
            {
                m_ScreenRatio = Screen.height / (float)Screen.width;
                m_DeltaScreenRatio = m_ScreenRatio - RATIO_DEFAULT;

                m_DeltaSizeMinCam = RATIO_CAMERA_SIZE_MIN * m_DeltaScreenRatio;
                m_IsInitCam = true;

                var distMax = 9f + (RATIO_CAMERA_SIZE_MAX * m_DeltaScreenRatio);

                m_RatioCamSize = (distMax - m_DeltaSizeMinCam) / 6f;
            }

            var delta = ((size - 5) * m_RatioCamSize) + m_DeltaSizeMinCam; 
            var cameraPos = m_CamOriginPos - (delta * m_CamDirOrigin);

            GamePlay.mainCamera.transform.position = cameraPos;

            Hint.CheckSoulution();

            Game.totalMove = Mathf.RoundToInt(Game.totalMove * 1.3f);
            Game.moveRemains = Game.totalMove;
            if (Game.isBossLevel)
            {
                UIController.UpdateMove();
            }

            UIController.SetupBossLevel(Game.isBossLevel);

            var delayTime = Game.isStartLevelEffect ? 1.5f : 0f;
            EventController.RaiseEventDelay(delayTime, EventList.GamePlay.GAME_START);
        }

        private int GetLevelSpawn()
        {
            var levelSpawn = 0;

            if (Game.seed > Game.levelDatas.Length)
            {
                levelSpawn = (Game.seed + 99) % Game.levelDatas.Length;
            }
            else
            {
                if (Game.isLevelDifficultyV2 || Game.isLevelDifficultyV3)
                {
                    if (Game.seed > 2 && Game.seed <= GetConditionLevelDifficulty())
                    {
                        levelSpawn = int.Parse(Game.levelDifficulty[Game.seed - 1]);
                    }
                    else
                    {
                        levelSpawn = (Game.seed - 1) % Game.levelDatas.Length;
                    }
                }
                else
                {
                    levelSpawn = (Game.seed - 1) % Game.levelDatas.Length;
                }
            }

            return levelSpawn;
        }

        private int GetConditionLevelDifficulty()
        {
            if (Game.isLevelDifficultyV3)
            {
                return 110;
            }
            else
            {
                return 100;
            }
        }

        private void SetupEnviroment()
        {
            Game.enviromentConfig = Config.listEnviromentConfigs[Game.abTheme];

            var type0 = RandomHelper.Range(0, 2);

            if(type0 == 0)
            {
                Game.listTerrainType.Add(Game.enviromentConfig.listTerrains[0]);
                Game.listTerrainType.Add(Game.enviromentConfig.listTerrains[1]);
            }
            else
            {
                Game.listTerrainType.Add(Game.enviromentConfig.listTerrains[1]);
                Game.listTerrainType.Add(Game.enviromentConfig.listTerrains[0]);
            }

            GamePlay.backgroundGroup.SetBackground(Config.listSpriteBackgrounds[Game.abTheme], Game.abTheme == 0);
        }

        private void OnClearAll()
        {
            m_ListWallCorners.ForEach(x => x.OnDestroyMe());
            Game.listGrids.ForEach(x => x.OnDestroyMe());
            Game.listGrids.Clear();

            Game.listSource.Clear();
            Game.listTarget.Clear();
            Game.listTerrainType.Clear();
            Game.listHints.Clear();

            Game.isMultiColor = false;
            Game.isTutorial = false;
            Game.isAllowHint = true;
            Game.isWaitAfterRV = false;
            Game.maxHintIndex = 0;
            Game.isPlaying = false;

            Game.totalMove = 0;
            Game.moveRemains = 0;

            Game.listGrids.Clear();
            Game.listUndo.Clear();

            Game.isBossLevel = false;
            Game.undoRemains = 2;
            Game.moveCount = 0;

            UIController.OnResetAll();
        }

        private void SpawnGridAll(LevelConfig levelConfig)
        {
            var size = levelConfig.size - 2;
            var startPosX = (size - 1) / -2f;
            var startPosZ = (size - 1) / 2f;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var posSpawn = new Vector3(startPosX + j, 0f, startPosZ - i);
                    var row = i + 1;
                    var col = j + 1;
                    var gridData = levelConfig.gridTypes[row, col].Trim();
                    SpawnGrid(m_GridParent, posSpawn, row, col, gridData, levelConfig.gridHints[row, col]);
                }
            }
        }

        private void SpawnGrid(Transform parent, Vector3 posSpawn, int row, int col, 
            string gridData, int gridRotTrue)
        {
            var gridType = int.Parse(gridData.Substring(0, 2));
            var gridRotCurrent = int.Parse(gridData.Substring(2, 1));

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

            m_ListWallCorners.ForEach(x => x.OnInit(Game.enviromentConfig.cornerMat));

            m_ListWallCorners[0].transform.localPosition = new Vector3(-wallPos, 0f, wallPos);
            m_ListWallCorners[1].transform.localPosition = new Vector3(wallPos, 0f, wallPos);
            m_ListWallCorners[2].transform.localPosition = new Vector3(-wallPos, 0f, -wallPos);
            m_ListWallCorners[3].transform.localPosition = new Vector3(wallPos, 0f, -wallPos);

            m_ListWallCorners.ForEach(x => x.OnTweenMove());


            m_WallParents[0].localPosition = new Vector3(0f, 0f, wallPos);
            SpawnWall(m_WallParents[0], size, startPos, levelConfig.topWall, true, 0, false);

            m_WallParents[1].localPosition = new Vector3(0f, 0f, -wallPos);
            SpawnWall(m_WallParents[1], size, startPos, levelConfig.botWall, true, size + 1, true);

            m_WallParents[2].localPosition = new Vector3(-wallPos, 0f, 0f);
            SpawnWall(m_WallParents[2], size, startPos, levelConfig.leftWall, false, 0, false);

            m_WallParents[3].localPosition = new Vector3(wallPos, 0f, 0f);
            SpawnWall(m_WallParents[3], size, startPos, levelConfig.rightWall, false, size + 1, true);
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

        private void CheckConnect()
        {
            Game.listGrids.ForEach(x => x.PrepareCheckConnect());

            var sourceCount = Game.listSource.Count;
            var totalGrid = Game.size * Game.size;

            for (int i = 0; i < sourceCount; i++)
            {
                var source = Game.listSource[i];

                m_ListGridHaveWater.Clear();
                m_ListGridHaveWater.Add(source);

                bool isChanged = true;

                var totalGridHaveWater = m_ListGridHaveWater.Count;
                var startIndex = 0;

                while (totalGridHaveWater < totalGrid && isChanged)
                {
                    var count = m_ListGridHaveWater.Count;
                    for (int j = startIndex; j < count; j++)
                    {
                        var grid = m_ListGridHaveWater[j];
                        var listGrids = grid.GetGridsCanConnect(source.WaterIndexColor);
                        startIndex += 1;
                        var listGridNearCount = listGrids.Count;
                        if (listGrids.Count > 0)
                        {
                            for (int x = 0; x < listGridNearCount; x++)
                            {
                                var gridNearConnect = listGrids[x];

                                if (!m_ListGridHaveWater.Contains(gridNearConnect))
                                {
                                    m_ListGridHaveWater.Add(gridNearConnect);
                                    gridNearConnect.SetConnect(source.WaterIndexColor, grid.DelayStart);
                                }
                            }
                        }
                    }

                    totalGridHaveWater = m_ListGridHaveWater.Count;
                    if (totalGridHaveWater == count)
                    {
                        isChanged = false;
                    }
                }
            }

            Game.listGrids.ForEach(x => x.AfterCheckConnect());

            if (IsWin())
            {
                GameOver(true);
            }
            else
            {
                if (Game.isBossLevel)
                {
                    if(Game.moveRemains <= 0)
                    {
                        GameOver(false);
                    }
                }
            }
        }

        private void GameOver(bool isWin)
        {
            Game.isGameWin = isWin;
            Game.isPlaying = false;

            Game.timePlay = (int)(System.DateTime.Now - m_TimeStart).TotalSeconds;
            Services.PushEvent(new IEC.TrackingEvent.GameOver());

            if (isWin)
            {
                PlayerData.countPlayThisLevel = 0;
                PlayerData.level += 1;
            }
            else
            {
                EventController.RaiseEventDelay(.5f, EventList.GamePlay.GAME_OVER_UI, new EventParam(this, false));
            }

        }

        private bool IsWin()
        {
            var count = Game.listTarget.Count;
            for (int i = 0; i < count; i++)
            {
                if (!Game.listTarget[i].IsDone) return false;
            }

            return true;
        }

        private bool IsComplete()
        {
            var count = Game.listTarget.Count;
            for (int i = 0; i < count; i++)
            {
                if (!Game.listTarget[i].IsComplete) return false;
            }
            return true;
        }

        private void OnUndo()
        {
            var rotConfig = Game.GetLastUndo();
            var grid = Game.grids[rotConfig.Row, rotConfig.Col];
            grid.OnUndo();
        }

        public static void PushPlayLevelEvent()
        {
            PlayerData.countPlayThisLevel += 1;
            var level = PlayerData.level;
            Services.PushEvent(new IEC.TrackingEvent.PlayLevel());

            if (PlayerData.levelFirstTime != level)
            {
                PlayerData.levelFirstTime = level;
                Services.PushEvent(new IEC.TrackingEvent.PlayLevelFirstTime());
                Services.SetUserProperty("Level", level.ToString());
            }

            if (level % Mathf.FloorToInt(Mathf.Pow(10, level.ToString().Length - 1)) == 0)
                Services.PushEvent($"play_level_{level}");
        }

    }
}
