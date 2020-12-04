using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GridStraightComponent : BaseGrid
    {
        public override void OnInit(GridType gridType, int row, int col, int rotTrue = 0, int rot = 0, int color = 0, int style = 0)
        {
            base.OnInit(gridType, row, col, rotTrue, rot, color, style);
            m_LineMeshs[0].material = m_LineMats[1];
        }

        protected override int[,] OriginLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 1},
            { 0, 0, 0}
        };
    }
}

