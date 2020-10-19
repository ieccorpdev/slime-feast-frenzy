using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GridNoneComponent : BaseComponent
    {
        public override void OnInit(GridType gridType, int row, int col, int rot = 0, int color = 0)
        {
            base.OnInit(gridType, row, col, rot, color);
            IsHaveWater = true;
        }
    }
}
