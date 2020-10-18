using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using NaughtyAttributes;


namespace IEC
{
    public class GameController : _BaseBehaviour, IController, IEventStart, IEventListener, IEventAwake
    {
        //[SerializeField]
        //private ParticleSystem m_PsWin;

        //[SerializeField]
        //private List<GridComponent> m_Grids;

        public Transform[] m_WallParents;   // TOP, BOT, LEFT, RIGHT
        public Transform m_GridParent;

        private List<GameObject> m_ListObj = new List<GameObject>();

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
        }

        public void BehaviourStart()
        {
            //OnStartLevel();
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.START_LEVEL:
                    //OnStartLevel();
                    break;

                case EventList.GamePlay.CHECK_GAME_OVER:
                    //CheckGameOver();
                    break;
            }
        }

        public int size;

        private void RemoveWall()
        {
            for (int i = 0; i < m_ListObj.Count; i++)
            {
                var obj = m_ListObj[i];
                if(obj != null)
                {
                    PrefabHelper.Destroy(obj);
                }
            }

            m_ListObj.Clear();
        }

        [Button]
        private void SpawnLevel()
        {
            RemoveWall();

            SpawnWalls();
            SpawnGrid();
        }

        private void SpawnGrid()
        {
            var startPosX = (size - 1) / -2f;
            var startPosZ = (size - 1) / 2f;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var posSpawn = new Vector3(startPosX + j, 0f, startPosZ - i);
                    SpawnGrid(m_GridParent, posSpawn, i, j, RandomHelper.Range(0, 4));
                }
            }
        }

        private void SpawnGrid(Transform parent, Vector3 posSpawn, int row, int col, int rot)
        {
            var grid = PrefabHelper.Clone(ComponentConfig.straightObj).transform;
            var gridComp = grid.GetComponent<BaseGrid>();
            grid.SetParent(parent);
            grid.gameObject.name = "Row " + row + " | Col " + col;
            grid.localPosition = posSpawn;
            grid.localRotation = Quaternion.identity;
            grid.localScale = Vector3.one;

            gridComp.OnInit(row, col, rot);

            m_ListObj.Add(grid.gameObject);
        }

        private void SpawnWalls()
        {
            var startPos = (size - 1) / -2f;
            var wallPos = (size / 2f) + .18f;

            m_WallParents[0].localPosition = new Vector3(0f, 0f, wallPos);
            SpawnWall(m_WallParents[0], size, startPos, WallType.TOP, true, 0);

            m_WallParents[1].localPosition = new Vector3(0f, 0f, -wallPos);
            SpawnWall(m_WallParents[1], size, startPos, WallType.BOT, true, size - 1);

            m_WallParents[2].localPosition = new Vector3(-wallPos, 0f, 0f);
            SpawnWall(m_WallParents[2], size, startPos, WallType.LEFT, false, 0);

            m_WallParents[3].localPosition = new Vector3(wallPos, 0f, 0f);
            SpawnWall(m_WallParents[3], size, startPos, WallType.RIGHT, false, size - 1);
        }

        private void SpawnWall(Transform parent, int count, float startPos, WallType wallType, bool isSameRow, int index)
        {
            for (int i = 0; i < count; i++)
            {
                var posSpawn = new Vector3(startPos + i * 1, 0f, 0f);

                var wallTrans = PrefabHelper.Clone(ComponentConfig.wallObj).transform;
                var wallComp = wallTrans.GetComponent<WallComponent>();

                wallTrans.SetParent(parent);
                wallTrans.localPosition = posSpawn;
                wallTrans.localRotation = Quaternion.identity;
                wallTrans.localScale = Vector3.one;

                var row = isSameRow ? index : i;
                var col = isSameRow ? i : index;

                wallComp.OnInit(row, col);
                wallComp.SetupWall(wallType, false);

                m_ListObj.Add(wallTrans.gameObject);
            }
        }

        //private void OnStartLevel()
        //{
        //    m_Grids.ForEach(x => x.OnInit());
        //    Game.isPlaying = true;
        //}

        //private void CheckGameOver()
        //{
        //    if (IsWin())
        //    {
        //        Game.isPlaying = false;
        //        m_PsWin.Play();
        //        EventController.RaiseEventDelay(3f, EventList.GamePlay.START_LEVEL);
        //    }
        //}

        //private bool IsWin()
        //{
        //    for (int i = 0; i < m_Grids.Count; i++)
        //    {
        //        if (!m_Grids[i].IsTrueRot()) return false;
        //    }
        //    return true;
        //}




    }
}
