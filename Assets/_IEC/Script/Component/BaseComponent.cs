using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseComponent : _BaseBehaviour
    {
        public int ID => gameObject.GetInstanceID();

        public GridType GridType;

        public virtual bool IsCanRotate() => false;

        public bool IsHaveWater;

        [HideInInspector]
        public bool IsRotating, IsCheckedConnect;

        [HideInInspector]
        public int WaterColor, Row, Col;

        protected int m_RotY;

        protected static int[] TotalRots = new int[] { 0, 90, 180, 270 };

        protected virtual int[,] DefaultLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 0, 1, 0},
            { 0, 0, 0}
        };

        protected int[,] CurerentLine = new int[3, 3];

        public bool IsHaveLineTop() => CurerentLine[0, 1] == 1;
        public bool IsHaveLineBot() => CurerentLine[2, 1] == 1;
        public bool IsHaveLineLeft() => CurerentLine[1, 0] == 1;
        public bool IsHaveLineRight() => CurerentLine[1, 2] == 1;

        private List<BaseComponent> listGridConnect = new List<BaseComponent>();

        public virtual void OnInit(GridType gridType , int row, int col, int rot = 0, int color = 0)
        {
            GridType = gridType;
            Row = row;
            Col = col;
            CurerentLine = DefaultLine;
            m_RotY = rot;

            transform.localRotation = Quaternion.Euler(0f, TotalRots[m_RotY], 0f);
        }

        public virtual void PrepareCheckConnect()
        {
            IsCheckedConnect = false;
            IsHaveWater = false;
        }

        public void SetConnect()
        {
            IsHaveWater = true;
            IsCheckedConnect = true;
        }

        private BaseComponent m_TempGrid;

        public List<BaseComponent> GetGridNearCanConnect()
        {
            listGridConnect.Clear();

            if (IsHaveLineTop())
            {
                m_TempGrid = Game.GetGrid(Row + 1, Col);
                if (m_TempGrid != null)
                {
                    if (m_TempGrid.IsHaveLineBot())
                    { 
                        listGridConnect.Add(m_TempGrid);
                    }
                }
            }

            if (IsHaveLineBot())
            {
                m_TempGrid = Game.GetGrid(Row - 1, Col);
                if (m_TempGrid != null)
                {
                    if (m_TempGrid.IsHaveLineTop())
                    {
                        listGridConnect.Add(m_TempGrid);
                    }
                }
            }

            if (IsHaveLineLeft())
            {
                m_TempGrid = Game.GetGrid(Row, Col - 1);
                if (m_TempGrid != null)
                {
                    if (m_TempGrid.IsHaveLineRight())
                    {
                        listGridConnect.Add(m_TempGrid);
                    }
                }
            }

            if (IsHaveLineRight())
            {
                m_TempGrid = Game.GetGrid(Row, Col + 1);
                if(m_TempGrid != null)
                {
                    if (m_TempGrid.IsHaveLineLeft())
                    {
                        listGridConnect.Add(m_TempGrid);
                    }
                }
            }

            return listGridConnect;
        }

        public void OnDestroyMe()
        {
            PrefabHelper.Destroy(gameObject);
        }
    }
}
