using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;

namespace IEC
{
    public class GameController : _BaseBehaviour, IController, IEventStart, IEventListener, IEventAwake, IEventUpdate
    {
        public Transform[] m_WallParents;   // TOP, BOT, LEFT, RIGHT
        public Transform[] m_WallCorners;   // TOP_LEFT, TOP_RIGHT, BOT_LEFT, BOT_RIGHT
        public Transform m_GridParent;

        private static float RATIO_DEFAULT = 16f / 9; //size 3 60 21:9 75
        private static float RATIO_DISTANCE = 11f / 6; //size 3 60 21:9 75
        private static float RATIO_SIZE = 3f / (21f / 9 - 16f / 9);

        private float m_ScreenRatio, m_DeltaRatio, m_AddDist;

        public Vector3 m_CamDirOrigin, m_CamOriginPos;

        private bool m_IsInitCam;

        private System.DateTime m_TimeStart;

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

            var strLevels = Resources.Load<TextAsset>("levelsList1");

            var file = strLevels.text;
            Game.levelDatas = file.Split(new string[] { "\n", "\r", "\r\n" },
                             System.StringSplitOptions.RemoveEmptyEntries);

            m_CamOriginPos = GamePlay.mainCamera.transform.position;
            m_CamDirOrigin = GamePlay.mainCamera.transform.forward;
        }

        [NaughtyAttributes.Button]
        private void GetDistance()
        {
            var currentPos = GamePlay.mainCamera.transform.position;
            var dist = Vector3.Distance(m_CamOriginPos, currentPos);
            Debug.LogError("Dist: " + dist);
        }

        public void BehaviourStart()
        {
            EventController.RaiseEvent(EventList.GamePlay.GAME_INIT);
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.GAME_INIT:
                    StartCoroutine(SpawnLevel());
                    break;

                case EventList.GamePlay.CHECK_GAME_OVER:
                    CheckGameOver();
                    break;
            }
        }

        public void BehaviourUpdate(float deltaTime)
        {
            //if (!Game.isPlaying) return;
            Game.listGrids.ForEach(x => x.OnUpdate(deltaTime));
        }

        private IEnumerator SpawnLevel()
        {
            OnClearAll();
            Game.seed = PlayerData.level;

            var levelSpawn = 0;

            if(Game.seed > Game.levelDatas.Length)
            {
                levelSpawn = (Game.seed + 99) % Game.levelDatas.Length;
            }
            else
            {
                levelSpawn = (Game.seed - 1) % Game.levelDatas.Length;
            }

            var leveText = Game.levelDatas[levelSpawn].Trim().Split(':');
            var strMode = leveText[1];
            var strGrid = leveText[3];

            int size = int.Parse(leveText[0].Substring(leveText[0].Length - 1)) + 2;

            var gridsData = strGrid.Trim().Split('_');
            string[,] grids = GetGridValue(gridsData, size, IsLoopLevel(strMode));

            LevelConfig levelConfig = new LevelConfig();
            levelConfig.SetSize(size, grids);
            Game.size = size;
            Game.grids = new BaseComponent[size, size];

            Game.isMultiColor = IsMultiColor(strMode);

            for (int i = 0; i < 2; i++)
            {
                var type = MathHelper.GetRandomIndexByList(Game.listTerrainType, 4);
                Game.listTerrainType.Add(type);
            }

            SpawnWalls(levelConfig);
            SpawnGrid(levelConfig);

            yield return null;
            m_TimeStart = System.DateTime.Now;
            Game.listGrids.ForEach(x => x.AfterInit(Game.isMultiColor));

            if (!m_IsInitCam)
            {
                m_ScreenRatio = Screen.height / (float)Screen.width;
                m_DeltaRatio = m_ScreenRatio - RATIO_DEFAULT;
                m_AddDist = RATIO_SIZE * m_DeltaRatio;

                m_IsInitCam = true;
            }

            var delta = ((size - 5) * RATIO_DISTANCE) + m_AddDist;
            var cameraPos = m_CamOriginPos - (delta * m_CamDirOrigin);
            GamePlay.mainCamera.transform.position = cameraPos;

            Game.isPlaying = true;
            EventController.RaiseEvent(EventList.GamePlay.CHECK_GAME_OVER);

            PushPlayLevelEvent();
        }

        private void OnClearAll()
        {
            Game.listGrids.ForEach(x => x.OnDestroyMe());
            Game.listGrids.Clear();

            Game.listSource.Clear();
            Game.listTarget.Clear();
            Game.listTerrainType.Clear();

            Game.isMultiColor = false;
        }

        private string[,] GetGridValue(string[] strGrids, int size, bool isLoop)
        {
            var grids = new string[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == 0 || j == 0 || i == size - 1 || j == size - 1)
                    {
                        if (grids[i, j] == null)
                        {
                            grids[i, j] = "1";
                        }
                    }
                    else
                    {
                        var index = (i - 1) * (size - 2) + (j - 1);
                        var gridData = strGrids[index];
                        var gridType = "";

                        if (isLoop)
                        {
                            var startIndex = gridData.IndexOf('(');
                            var endIndex = gridData.IndexOf(')');
                            gridType = gridData.Substring(0, startIndex);

                            var edgeData = gridData.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
                            var edges = edgeData.Split(',');

                            if (i == 1)
                            {
                                if (edges[0] != "-1")
                                {
                                    grids[0, j] = "2";
                                }
                            }

                            if (j == 1)
                            {
                                if (edges[1] != "-1")
                                {
                                    grids[i, 0] = "2";
                                }
                            }

                            if (i == size - 2)
                            {
                                if (edges[2] != "-1")
                                {
                                    grids[size - 1, j] = "2";
                                }
                            }

                            if (j == size - 2)
                            {
                                if (edges[3] != "-1")
                                {
                                    grids[i, size - 1] = "2";
                                }
                            }
                        }
                        else
                        {
                            gridType = gridData;
                        }

                        grids[i, j] = gridType;
                    }
                }
            }

            return grids;
        }

        private void SpawnGrid(LevelConfig levelConfig)
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
                    SpawnGrid(m_GridParent, posSpawn, row, col, gridData);
                }
            }
        }

        private void SpawnGrid(Transform parent, Vector3 posSpawn, int row, int col, string gridData)
        {
            var gridType = int.Parse(gridData.Substring(0, 2));
            var gridRot = int.Parse(gridData.Substring(2, 1));

            var gridColor = 0;
            var gridStyle = 0;

            var grid = PrefabHelper.Clone(GetGridObj(gridType)).transform;
            var gridComp = grid.GetComponent<BaseComponent>();

            if (IsSourceGird(gridType) || IsTargetGrid(gridType))
            {
                if (Game.isMultiColor)
                {
                    gridColor = int.Parse(gridData.Substring(3, 1));
                }
            }

            grid.SetParent(parent);
            grid.gameObject.name = "Row " + row + " | Col " + col;
            grid.localPosition = posSpawn;
            grid.localRotation = Quaternion.identity;
            grid.localScale = Vector3.one;

            if (IsSourceGird(gridType))
            {
                var sourceComp = grid.GetComponent<GridSourceComponent>();
                Game.listSource.Add(sourceComp);
                gridStyle = GetSourceStyle(gridType);
            }
            
            if (IsTargetGrid(gridType))
            {
                var targetComp = grid.GetComponent<GridTargetComponent>();
                Game.listTarget.Add(targetComp);
                gridStyle = GetTargetStyle(gridType);
            }

            gridComp.OnInit(GetType(gridType), row, col, gridRot, gridColor, gridStyle);

            Game.grids[row, col] = gridComp;
            Game.listGrids.Add(gridComp);
        }

        private bool IsLoopLevel(string mode)
        {
            return (mode.IndexOf("Loop") > -1);
        }

        private bool IsMultiColor(string mode)
        {
            return (mode.IndexOf("colors") > -1);
        }

        private bool IsSourceGird(int gridType)
        {
            return (gridType == 51 || gridType == 11 || gridType == 21 || gridType == 31);
        }

        private bool IsTargetGrid(int gridType)
        {
            return (gridType == 52 || gridType == 12 || gridType == 22 || gridType == 32);
        }

        private int GetSourceStyle(int input)
        {
            if(input == 11)
            {
                return 1;
            }else if(input == 21)
            {
                return 2;
            }
            else if(input == 31)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }

        private int GetTargetStyle(int input)
        {
            if (input == 11)
            {
                return 1;
            }
            else if (input == 21)
            {
                return 2;
            }
            else if (input == 31)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }

        private GridType GetType(int gridType)
        {
            GridType type = GridType.NONE;
            switch (gridType)
            {
                case 11:
                case 21:
                case 31:
                case 51:
                    type = GridType.GRID_SOURCE;
                    break;
                case 52:
                case 22:
                case 32:
                case 12:
                    type = GridType.GRID_TARGET;
                    break;
                case 10:
                    type = GridType.GRID_STRAIGHT;
                    break;
                case 30:
                    type = GridType.GRID_CROSS;
                    break;
                case 20:
                    type = GridType.GRID_CURVE;
                    break;
            }

            return type;
        }

        private GameObject GetGridObj(int gridType)
        {
            GameObject obj = null;
            switch (gridType)
            {
                case 11:
                case 21:
                case 31:
                case 51:
                    obj = ComponentConfig.sourceObj;
                    break;
                case 52:
                case 12:
                case 22:
                case 32:
                    obj = ComponentConfig.targetObj;
                    break;
                case 10:
                    obj = ComponentConfig.straightObj;
                    break;
                case 30:
                    obj = ComponentConfig.crossObj;
                    break;
                case 20:
                    obj = ComponentConfig.curveObj;
                    break;
                default:
                    Debug.LogError("gridType: " + gridType);
                    obj = ComponentConfig.straightObj;
                    break;
            }

            return obj;
        }

        private void SpawnWalls(LevelConfig levelConfig)
        {
            var size = levelConfig.size - 2;
            var startPos = (size - 1) / -2f;
            var wallPos = (size / 2f) + .18f;

            m_WallCorners[0].localPosition = new Vector3(-wallPos, 0f, wallPos);
            m_WallCorners[1].localPosition = new Vector3(wallPos, 0f, wallPos);
            m_WallCorners[2].localPosition = new Vector3(-wallPos, 0f, -wallPos);
            m_WallCorners[3].localPosition = new Vector3(wallPos, 0f, -wallPos);

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

                var wallTrans = PrefabHelper.Clone(ComponentConfig.wallObj).transform;
                var wallComp = wallTrans.GetComponent<WallComponent>();

                wallTrans.SetParent(parent);
                wallTrans.localPosition = posSpawn;
                wallTrans.localRotation = Quaternion.identity;
                wallTrans.localScale = Vector3.one;

                var row = isBotTop ? index : i;
                var col = isBotTop ? i : index;

                wallTrans.gameObject.name = "Row " + row + " | Col " + col;

                wallComp.OnInit(GridType.WALL, row, col);
                wallComp.SetupWall(wallValue == 2, isBotTop, isReverse);

                Game.grids[row, col] = wallComp;
                Game.listGrids.Add(wallComp);
            }
        }

        private List<BaseComponent> m_ListGridHaveWater = new List<BaseComponent>();

        private void CheckGameOver()
        {
            Game.listGrids.ForEach(x => x.PrepareCheckConnect());
            Game.listSource.ForEach(x => x.SetConnect(x.WaterColor));

            m_ListGridHaveWater.Clear();
            m_ListGridHaveWater.AddRange(Game.listSource);

            var totalGrid = (Game.size * Game.size) - 4;
            bool isChanged = true;

            while ((m_ListGridHaveWater.Count < totalGrid) && isChanged)
            {
                var count = m_ListGridHaveWater.Count;
                for (int i = 0; i < count; i++)
                {
                    var grid = m_ListGridHaveWater[i];
                    var listGridNear = grid.GetGridNearCanConnect();

                    if(listGridNear.Count > 0)
                    {
                        listGridNear.ForEach(x => x.SetConnect(grid.WaterColor));
                        m_ListGridHaveWater.AddRange(listGridNear);
                    }
                }

                if(m_ListGridHaveWater.Count == count)
                {
                    isChanged = false;
                }
            }

            Game.listGrids.ForEach(x => x.AfterCheckConnect());

            if (IsWin())
            {
                Game.timePlay = (int)(System.DateTime.Now - m_TimeStart).TotalSeconds;
                Services.PushEvent(new IEC.TrackingEvent.GameOver());
                PlayerData.countPlayThisLevel = 0;
                PlayerData.level += 1;
                Game.isPlaying = false;
                EventController.RaiseEventDelay(2f, EventList.GamePlay.GAME_OVER);
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

    [Serializable]
    public class LevelConfig
    {
        public int size;
        public int[] topWall, botWall, leftWall, rightWall;
        public string[,] gridTypes;

        public void SetSize(int size, string[,] gridTypes)
        {
            this.size = size;

            topWall = new int[size];
            botWall = new int[size];
            leftWall = new int[size];
            rightWall = new int[size];

            this.gridTypes = gridTypes;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var grid = gridTypes[i, j].Trim().Split('_');
                    var tempValue = int.Parse(grid[0]);

                    if (i == 0)
                    {
                        topWall[j] = tempValue;
                    }
                    if (i == size - 1)
                    {
                        botWall[j] = tempValue;
                    }

                    if (j == 0)
                    {
                        leftWall[i] = tempValue;
                    }

                    if (j == size - 1)
                    {
                        rightWall[i] = tempValue;
                    }
                }
            }
        }
    }
}
