using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GridStraightComponent : BaseGrid
    {
        protected override int[,] OriginLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 1},
            { 0, 0, 0}
        };
    }
}

