using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace IEC
{
    public class GameController : _BaseBehaviour, IController, IEventStart, IEventListener, IEventAwake
    {
        public Transform[] m_WallParents;   // TOP, BOT, LEFT, RIGHT
        public Transform m_GridParent;

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

        public void BehaviourStart()
        {
            EventController.RaiseEvent(EventList.GamePlay.START_LEVEL);
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.START_LEVEL:
                    SpawnLevel();
                    break;

                case EventList.GamePlay.CHECK_GAME_OVER:
                    CheckGameOver();
                    break;
            }
        }

        const string jsonSavePath = "Assets/_IEC/Config/Levels/Level_5.txt";

        private void SpawnLevel()
        {
            OnClearAll();

            var file = File.ReadAllText(jsonSavePath);
            string[] f = file.Split(new string[] { "\n", "\r", "\r\n" },
                             System.StringSplitOptions.RemoveEmptyEntries);

            LevelConfig levelConfig = new LevelConfig();

            int size = int.Parse(f[0]);
            string[,] gridTypes= GetGridValue(f, size, 1);
            levelConfig.SetSize(size, gridTypes);
            Game.size = size;
            Game.grids = new BaseComponent[size, size];

            SpawnWalls(levelConfig);
            SpawnGrid(levelConfig);

            EventController.RaiseEventDelay(.2f, EventList.GamePlay.CHECK_GAME_OVER);
        }

        private void OnClearAll()
        {
            Game.listGrids.ForEach(x => x.OnDestroyMe());
            Game.listGrids.Clear();

            Game.listSource.Clear();
            Game.listTarget.Clear();
        }

        private string[,] GetGridValue(string[] str, int size, int startRow)
        {
            var grid = new string[size, size];
            var count = startRow + size;
            for (int i = startRow; i < count; i++)
            {
                var row = str[i].Trim().Split(' ');
                for (int j = 0; j < size; j++)
                {
                    grid[i - 1, j] = row[j];
                }
            }

            return grid;
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
                    var grid = levelConfig.gridTypes[row, col].Trim().Split('_');
                    SpawnGrid(m_GridParent, posSpawn, row, col, grid);
                }
            }
        }

        private void SpawnGrid(Transform parent, Vector3 posSpawn, int row, int col, string[] gridtype)
        {
            var type = int.Parse(gridtype[0]);
            var grid = PrefabHelper.Clone(GetGridObj(type)).transform;
            var gridComp = grid.GetComponent<BaseComponent>();


            var gridColor = 0;
            var gridStyle = 0;

            if (type == 5)
            {
                var sourceComp = grid.GetComponent<GridSourceComponent>();
                Game.listSource.Add(sourceComp);
                gridColor = int.Parse(gridtype[2]);
                gridStyle = int.Parse(gridtype[3]);
            }
            else if(type == 10)
            {
                var targetComp = grid.GetComponent<GridTargetComponent>();
                Game.listTarget.Add(targetComp);
                gridColor = int.Parse(gridtype[2]);
            }
            var gridRot = int.Parse(gridtype[1]);
            grid.SetParent(parent);
            grid.gameObject.name = "Row " + row + " | Col " + col;
            grid.localPosition = posSpawn;
            grid.localRotation = Quaternion.identity;
            grid.localScale = Vector3.one;

            gridComp.OnInit(GetType(type), row, col, gridRot, gridColor, gridStyle);

            Game.grids[row, col] = gridComp;
            Game.listGrids.Add(gridComp);
        }

        private GridType GetType(int gridType)
        {
            GridType type = GridType.NONE;
            switch (gridType)
            {
                case 5:
                    type = GridType.GRID_SOURCE;
                    break;
                case 10:
                    type = GridType.GRID_TARGET;
                    break;
                case 15:
                    type = GridType.GRID_STRAIGHT;
                    break;
                case 16:
                    type = GridType.GRID_CROSS;
                    break;
                case 17:
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
                case 5:
                    obj = ComponentConfig.sourceObj;
                    break;
                case 10:
                    obj = ComponentConfig.targetObj;
                    break;
                case 15:
                    obj = ComponentConfig.straightObj;
                    break;
                case 16:
                    obj = ComponentConfig.crossObj;
                    break;
                case 17:
                    obj = ComponentConfig.curveObj;
                    break;
            }

            return obj;
        }

        private void SpawnWalls(LevelConfig levelConfig)
        {
            var size = levelConfig.size - 2;
            var startPos = (size - 1) / -2f;
            var wallPos = (size / 2f) + .18f;

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
                var wallValue = walls[i - 1];

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
            bool isChanged = false;

            while ((m_ListGridHaveWater.Count < totalGrid) && !isChanged)
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
                    isChanged = true;
                }
            }

            Game.listGrids.ForEach(x => x.AfterCheckConnect());

            if (IsWin())
            {
                Debug.LogError("Win");
            }
        }

        private bool IsWin()
        {
            var count = Game.listTarget.Count;
            for (int i = 0; i < count; i++)
            {
                if (!Game.listTarget[i].IsHaveWater) return false;
            }

            return true;
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
                    }else if (i == size - 1)
                    {
                        botWall[j] = tempValue;
                    }else if (j == 0)
                    {
                        leftWall[i] = tempValue;
                    }else if (j == size - 1)
                    {
                        rightWall[i] = tempValue;
                    }
                }
            }
        }
    }
}
