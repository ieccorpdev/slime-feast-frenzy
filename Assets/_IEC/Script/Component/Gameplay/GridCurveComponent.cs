using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GridCurveComponent : BaseGrid
    {
        public override void OnInit(GridType gridType, int row, int col, int rotTrue = 0, int rot = 0, int color = 0, int style = 0)
        {
            base.OnInit(gridType, row, col, rotTrue, rot, color, style);
        }

        protected override int[,] OriginLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 0},
            { 0, 1, 0}
        };
    }
}