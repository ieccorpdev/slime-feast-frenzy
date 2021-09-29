using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GridSourceComponent : BaseGrid
    {
        [SerializeField]
        private Transform m_TranScale, m_TransWater, m_TransCircle1, m_TransCircle2, m_TransCircle3;

        [SerializeField]
        private GameObject m_OldFountainObj;

        protected override int[,] OriginLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 0, 1, 0},
            { 0, 0, 0}
        };

        protected int[,] EndLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 0},
            { 0, 0, 0}
        };

        protected int[,] CrossLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 1},
            { 0, 1, 0}
        };

        protected int[,] CurveLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 0},
            { 0, 1, 0}
        };

        protected int[,] StraightLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 1},
            { 0, 0, 0}
        };

        private int m_LineType;

        public override bool IsTrueRot()
        {
            if(m_LineType == 1)
            {
                if (IsOneWayBlock)
                {
                    return m_CurrentRot == TrueRot;
                }
                else if(TrueRot == 0 || TrueRot == 2)
                {
                    return (m_CurrentRot == 0 || m_CurrentRot == 2);
                }
                else
                {
                    return (m_CurrentRot == 1 || m_CurrentRot == 3);
                }
            }
            else
            {
                return m_CurrentRot == TrueRot;
            }
        }

        public override void OnInit(GridType gridType, int row, int col, int rotTrue = 0, int rot = 0, int color = 0, int lineType = 0)
        {
            base.OnInit(gridType, row, col, rotTrue, rot, color, lineType);

            m_TranScale.localScale = Vector3.one * (Game.isBiggerFoundtain ? 1.5f : 1f);
            m_LineType = lineType;

            for (int i = 0; i < 4; i++)
            {
                m_LineStyleObjs[i].SetActive(i == lineType);
            }

            for (int i = 0; i < m_WaterCount - 4; i++)
            {
                m_WaterComponent[i].gameObject.SetActive(i == lineType);
            }

            ConfigWater();
        }

        protected override int[,] GetDefaultLine(int lineTyle)
        {
            if (lineTyle == 1)
            {
                return StraightLine;
            }
            else if (lineTyle == 2)
            {
                return CurveLine;
            }
            else if(lineTyle == 3)
            {
                return CrossLine;
            }
            else
            {
                return EndLine;
            }
        }

        public override void AfterInit()
        {
            m_StartWater = WaterPoint.CENTER;
            SetGrowingWater(true, m_StartWater);
        }

        private void ConfigWater()
        {
            m_IsHavingWater = true;
            m_IsHaveWater = true;
            m_IsOldHaveWater = true;
            m_OldWaterIndexColor = TrueColor;

            if (m_ListWaterColor.Count == 0)
            {
                m_ListWaterColor.Add(TrueColor);
            }

            for (int i = 0; i < m_WaterCount; i++)
            {
                m_WaterComponent[i].SetColor(CurrentWaterColor, true);
            }
        }

        protected override int GetTrueRotHint()
        {
            if (m_LineType == 1)
            {
                if (IsOneWayBlock)
                {
                    return TrueRot;
                }
                return (m_CurrentRot + 1) % 4;
            }
            else
            {
                return TrueRot;
            }
                
        }
    }
}