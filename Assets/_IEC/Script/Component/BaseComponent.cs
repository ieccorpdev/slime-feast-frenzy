using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseComponent : _BaseBehaviour
    {
        public int ID => gameObject.GetInstanceID();

        public virtual bool IsCanRotate() => false;

        [HideInInspector]
        public bool IsHaveWater, IsRotating, IsCheckedConnect;

        [HideInInspector]
        public int WaterColor, Row, Col;

        public int m_RotY;

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

        public virtual void OnInit(int row, int col, int rot = 0, int color = 0)
        {
            Row = row;
            Col = col;
            CurerentLine = DefaultLine;
        }

        public virtual void PrepareCheckConnect()
        {
            IsCheckedConnect = false;
        }

        private BaseComponent m_TempGrid;
        public virtual void CheckConnect()
        {
            if (IsCheckedConnect) return;
            IsHaveWater = true;
            listGridConnect.Clear();

            if (IsHaveLineTop())
            {
                m_TempGrid = Game.GetGrid(Row + 1, Col);
                if (m_TempGrid.IsHaveLineBot())
                {
                    listGridConnect.Add(m_TempGrid);
                }
            }

            if (IsHaveLineBot())
            {
                m_TempGrid = Game.GetGrid(Row - 1, Col);
                if (m_TempGrid.IsHaveLineTop())
                {
                    listGridConnect.Add(m_TempGrid);
                }
            }

            if (IsHaveLineLeft())
            {
                m_TempGrid = Game.GetGrid(Row, Col - 1);
                if (m_TempGrid.IsHaveLineRight())
                {
                    listGridConnect.Add(m_TempGrid);
                }
            }

            if (IsHaveLineRight())
            {
                m_TempGrid = Game.GetGrid(Row, Col + 1);
                if (m_TempGrid.IsHaveLineLeft())
                {
                    listGridConnect.Add(m_TempGrid);
                }
            }

            for (int i = 0; i < listGridConnect.Count; i++)
            {
                m_TempGrid = listGridConnect[i];
                if (m_TempGrid.IsCheckedConnect) continue;
                m_TempGrid.CheckConnect();
            }

            IsCheckedConnect = true;
        }
    }
}
