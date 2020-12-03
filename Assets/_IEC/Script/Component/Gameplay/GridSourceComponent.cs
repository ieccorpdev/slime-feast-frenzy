using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GridSourceComponent : BaseGrid
    {
        [SerializeField]
        private GameObject[] m_StyleObj;

        [SerializeField]
        private GameObject m_DefaultSourceObj, m_NewSourceObj;

        [SerializeField]
        private Transform m_TransWaterFall;

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

        public override void OnInit(GridType gridType, int row, int col, int rotTrue = 0, int rot = 0, int color = 0, int style = 0)
        {
            base.OnInit(gridType, row, col, rotTrue, rot, color, style);
            for (int i = 0; i < m_StyleObj.Length; i++)
            {
                m_StyleObj[i].SetActive(i == style);
            }

            for (int i = 0; i < m_WaterCount - 3; i++)
            {
                m_WaterComponent[i].gameObject.SetActive(i == style);
            }

            m_WaterComponent[m_WaterCount - 4].gameObject.SetActive(true);

            bool isNewTheme = Game.abTheme > 0;

            for (int i = m_WaterCount - 3; i < m_WaterCount; i++)
            {
                m_WaterComponent[i].gameObject.SetActive(isNewTheme);
            }

            m_TransWaterFall.localPosition = new Vector3(isNewTheme ? -.1f : 0f, .2f, 0f);

            m_DefaultSourceObj.SetActive(!isNewTheme);
            m_NewSourceObj.SetActive(!isNewTheme);

            ConfigWater();
        }

        protected override int[,] GetDefaultLine(int style)
        {
            if (style == 1)
            {
                return StraightLine;
            }
            else if (style == 2)
            {
                return CurveLine;
            }
            else if(style == 3)
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
            base.AfterInit();

            SetSetGrowingWater(true, WaterPoint.CENTER);
        }

        private void ConfigWater()
        {
            isHavingWater = true;
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

        public override void PrepareCheckConnect()
        {
            
        }



    }
}