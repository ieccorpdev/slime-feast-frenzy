using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseComponent : _BaseBehaviour
    {
        [SerializeField]
        protected WaterComponent[] m_WaterComponent;

        public int ID => gameObject.GetInstanceID();

        public GridType GridType;

        public bool IsHaveWater;

        [HideInInspector]
        public bool IsRotating, IsCheckedConnect;

        [HideInInspector]
        public int WaterColor, Row, Col;

        protected int m_RotY;

        protected bool m_IsStatusWater;

        protected static int[] TotalRots = new int[] { 0, 90, 180, 270 };

        protected virtual int[,] DefaultLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 0, 1, 0},
            { 0, 0, 0}
        };

        protected int[,] CurerentLine = new int[3, 3];

        public bool IsHaveLineTop => CurerentLine[0, 1] == 1;
        public bool IsHaveLineBot => CurerentLine[2, 1] == 1;
        public bool IsHaveLineLeft => CurerentLine[1, 0] == 1;
        public bool IsHaveLineRight => CurerentLine[1, 2] == 1;

        public virtual void OnInit(GridType gridType , int row, int col, int rot = 0, int color = 0, int style = 0)
        {
            GridType = gridType;
            Row = row;
            Col = col;
            m_RotY = rot;
            CurerentLine = GetDefaultLine(style);

            transform.localRotation = Quaternion.Euler(0f, TotalRots[m_RotY], 0f);

            for (int i = 0; i < m_WaterComponent.Length; i++)
            {
                m_WaterComponent[i].OnInit();
            }
        }

        protected virtual int[,] GetDefaultLine(int style)
        {
            return DefaultLine;
        }

        public virtual void PrepareCheckConnect()
        {
            m_IsStatusWater = IsHaveWater;

            IsCheckedConnect = false;
            IsHaveWater = false;
        }

        public virtual void SetConnect(int color)
        {
            IsHaveWater = true;
            IsCheckedConnect = true;
            WaterColor = color;
        }

        public virtual void AfterCheckConnect()
        {

        }

        private BaseComponent m_TempGrid;
        private List<BaseComponent> m_ListGridConnect = new List<BaseComponent>();

        public List<BaseComponent> GetGridNearCanConnect()
        {
            m_ListGridConnect.Clear();

            if (IsHaveLineTop)
            {
                m_TempGrid = Game.GetGrid(Row - 1, Col);
                if (m_TempGrid != null)
                {
                    if (m_TempGrid.IsHaveLineBot)
                    { 
                        m_ListGridConnect.Add(m_TempGrid);
                    }
                }
            }

            if (IsHaveLineBot)
            {
                m_TempGrid = Game.GetGrid(Row + 1, Col);
                if (m_TempGrid != null)
                {
                    if (m_TempGrid.IsHaveLineTop)
                    {
                        m_ListGridConnect.Add(m_TempGrid);
                    }
                }
            }

            if (IsHaveLineLeft)
            {
                m_TempGrid = Game.GetGrid(Row, Col - 1);
                if (m_TempGrid != null)
                {
                    if (m_TempGrid.IsHaveLineRight)
                    {
                        m_ListGridConnect.Add(m_TempGrid);
                    }
                }
            }

            if (IsHaveLineRight)
            {
                m_TempGrid = Game.GetGrid(Row, Col + 1);
                if (m_TempGrid != null)
                {
                    if (m_TempGrid.IsHaveLineLeft)
                    {
                        m_ListGridConnect.Add(m_TempGrid);
                    }
                }
            }

            if(GridType == GridType.WALL)
            {
                var m_TempGrid = GetOppositeWall();
                if (m_TempGrid != null)
                {
                    m_ListGridConnect.Add(m_TempGrid);
                }
            }

            return m_ListGridConnect;
        }

        private BaseComponent GetOppositeWall()
        {
            var maxIndex = Game.size - 1;
            var row = Row;
            var col = Col;

            if (Row == 0)
            {
                row = maxIndex;
            }

            if (Row == maxIndex)
            {
                row = 0;
            }

            if (Col == 0)
            {
                col = maxIndex;
            }

            if (Col == maxIndex)
            {
                col = 0;
            }

            return Game.GetGrid(row, col);

        }

        public void OnDestroyMe()
        {
            PrefabHelper.Destroy(gameObject);
        }

        [NaughtyAttributes.Button]
        protected void CheckLine()
        {
            for (int i = 0; i < 3; i++)
            {
                var str = "";
                for (int j = 0; j < 3; j++)
                {
                    str += CurerentLine[i, j];
                }
                Debug.LogError(str);
            }

            Debug.LogError("IsHaveLineTop: " + IsHaveLineTop + " | IsHaveLineBot: " + IsHaveLineBot
                + " | IsHaveLineLeft: " + IsHaveLineLeft + " | IsHaveLineRight: " + IsHaveLineRight);
        }
    }
}
