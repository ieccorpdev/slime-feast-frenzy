using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GridCurveComponent : BaseGrid
    {
        public override bool IsCanRotate() => true;

        protected override int[,] DefaultLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 0, 1, 1},
            { 0, 1, 0}
        };
    }
}