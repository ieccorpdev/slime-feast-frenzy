using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class WallComponent : BaseComponent
    {
        private bool m_IsHaveHole;

        [SerializeField]
        private Transform m_TransWallHole;

        protected override int[,] DefaultLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 0, 1, 0},
            { 0, 0, 0}
        };

        protected int[,] DefaultLineWallTopBot => new int[3, 3]
        {
            { 0, 1, 0},
            { 0, 1, 0},
            { 0, 1, 0}
        };

        protected int[,] DefaultLineWallLeftRight => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 1},
            { 0, 0, 0}
        };


        public override bool IsCanRotate() => false;

        public virtual void OnInit(GridType gridType, int row, int col, int rot = 0, int color = 0)
        {
            base.OnInit(gridType, row, col, rot, color);
            IsRotating = false;
        }

        public void SetupWall(bool isHaveHole, bool isBotTop, bool isReverse)
        {
            m_IsHaveHole = isHaveHole;
            m_TransWallHole.localRotation = Quaternion.Euler(0f, (isReverse && isHaveHole) ? 180f : 0f, 0f);
            CurerentLine = isHaveHole ? (isBotTop ? DefaultLineWallTopBot : DefaultLineWallLeftRight) : DefaultLine;
        }
    }
}
