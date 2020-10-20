using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GridSourceComponent : BaseGrid
    {
        [SerializeField]
        private GameObject[] m_StyleObj;

        protected override int[,] DefaultLine => new int[3, 3]
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
            { 0, 1, 0},
            { 1, 1, 0},
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

        public override void OnInit(GridType gridType, int row, int col, int rot = 0, int color = 0, int style = 0)
        {
            base.OnInit(gridType, row, col, rot, color, style);
            IsHaveWater = true;

            for (int i = 0; i < m_StyleObj.Length; i++)
            {
                m_StyleObj[i].SetActive(i == style);
            }
        }

        protected override int[,] GetDefaultLine(int style)
        {
            if (style == 1)
            {
                return CrossLine;
            }
            else if (style == 2)
            {
                return CurveLine;
            }
            else if(style == 3)
            {
                return StraightLine;
            }
            else
            {
                return EndLine;
            }
        }
    }
}