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
            PrefabHelper.SetUpPrefab(ComponentConfig.wallHoleObj);
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

        int[,] array = new int[3, 3] {
            { 0,1,0},
            { 0,1,0},
            { 0,1,0}
        };

        static int[,] RotateMatrix(int[,] matrix, int n)
        {
            int[,] ret = new int[n, n];

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    ret[i, j] = matrix[n - j - 1, i];
                }
            }

            return ret;
        }

        [Button]
        private void SpawnLevel()
        {
            //RemoveWall();

            //SpawnWalls();
            //SpawnGrid();

            int[,] rotated = RotateMatrix(array, 3);

            
            for (int i = 0; i < 3; i++)
            {
                var str = "";

                for (int j = 0; j < 3; j++)
                {
                    str += rotated[i, j];
                }
                Debug.LogError(str + "\n");
            }
        }

        private void SpawnGrid()
        {
            var startPos = (size - 1) / -2f;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var posSpawn = new Vector3(startPos + j, 0f, startPos + i);
                    SpawnGrid(m_GridParent, posSpawn);
                }
            }
        }

        private void SpawnGrid(Transform parent, Vector3 posSpawn)
        {
            var grid = PrefabHelper.Clone(ComponentConfig.straightObj).transform;
            grid.SetParent(parent);
            grid.localPosition = posSpawn;
            grid.localRotation = Quaternion.identity;
            grid.localScale = Vector3.one;

            m_ListObj.Add(grid.gameObject);
        }

        private void SpawnWalls()
        {
            var startPos = (size - 1) / -2f;
            var wallPos = (size / 2f) + .18f;

            m_WallParents[0].localPosition = new Vector3(0f, 0f, wallPos);
            m_WallParents[1].localPosition = new Vector3(0f, 0f, -wallPos);

            m_WallParents[2].localPosition = new Vector3(-wallPos, 0f, 0f);
            m_WallParents[3].localPosition = new Vector3(wallPos, 0f, 0f);

            for (int i = 0; i < 4; i++)
            {
                SpawnWall(m_WallParents[i], size, startPos);
            }
        }

        private void SpawnWall(Transform parent, int count, float startPos)
        {
            for (int i = 0; i < count; i++)
            {
                var posSpawn = new Vector3(startPos + i * 1, 0f, 0f);

                var wallTrans = PrefabHelper.Clone(ComponentConfig.wallObj).transform;
                wallTrans.SetParent(parent);
                wallTrans.localPosition = posSpawn;
                wallTrans.localRotation = Quaternion.identity;
                wallTrans.localScale = Vector3.one;

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
