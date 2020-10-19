using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GridSourceComponent : BaseGrid
    {
        protected override int[,] DefaultLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 0},
            { 0, 0, 0}
        };

        public override void OnInit(GridType gridType, int row, int col, int rot = 0, int color = 0)
        {
            base.OnInit(gridType, row, col, rot, color);
            IsHaveWater = true;
        }
    }
}