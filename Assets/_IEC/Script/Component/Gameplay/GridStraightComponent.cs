using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GridStraightComponent : BaseGrid
    {
        public override bool IsTrueRot()
        {
            if (IsOneWayBlock)
            {
                return m_CurrentRot == TrueRot;
            }
            else if (TrueRot == 0 || TrueRot == 2)
            {
                return (m_CurrentRot == 0 || m_CurrentRot == 2);
            }
            else
            {
                return (m_CurrentRot == 1 || m_CurrentRot == 3);
            }
        }

        public override void OnInit(GridType gridType, int row, int col, int rotTrue = 0, int rot = 0, int color = 0, int lineType = 0)
        {
            base.OnInit(gridType, row, col, rotTrue, rot, color, lineType);
        }

        protected override int[,] OriginLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 1},
            { 0, 0, 0}
        };

        protected override int GetTrueRotHint()
        {
            if (IsOneWayBlock)
            {
                return TrueRot;
            }
            return (m_CurrentRot + 1) % 4;
        }
    }
}

