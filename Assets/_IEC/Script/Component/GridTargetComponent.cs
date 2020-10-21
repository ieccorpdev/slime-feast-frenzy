using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GridTargetComponent : BaseGrid
    {
        [SerializeField]
        private ParticleSystem m_PsWin;

        [SerializeField]
        private MeshRenderer[] m_MeshTargets;

        private int m_TrueWaterColor;

        public bool IsDone => IsHaveWater && (m_TrueWaterColor == WaterColor);

        public override void OnInit(GridType gridType, int row, int col, int rot = 0, int color = 0, int style = 0)
        {
            base.OnInit(gridType, row, col, rot, color, style);
            m_TrueWaterColor = color;
        }

        public override void AfterInit(bool isMultiColor)
        {
            base.AfterInit(isMultiColor);
            var mat = isMultiColor ? Config.targetMats[m_TrueWaterColor] : Config.targetDefaultMat;

            for (int i = 0; i < m_MeshTargets.Length; i++)
            {
                m_MeshTargets[i].material = mat;
            }
        }

        protected override int[,] DefaultLine => new int[3, 3]
        {
            { 0, 1, 0},
            { 0, 1, 0},
            { 0, 0, 0}
        };

        public override void AfterCheckConnect()
        {
            if (m_IsOldStatusWater == IsHaveWater) return;
            SetActiveWater(IsHaveWater);
            if (IsDone)
            {
                m_PsWin.Play();
            }
        }
    }
}

