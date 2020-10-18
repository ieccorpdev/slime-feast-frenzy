using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GridTargetComponent : BaseGrid
    {
        protected override int[,] DefaultLine => new int[3, 3]
        {
            { 0, 1, 0},
            { 0, 1, 0},
            { 0, 0, 0}
        };
    }
}

