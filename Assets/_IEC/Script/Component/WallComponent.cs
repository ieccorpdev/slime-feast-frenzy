using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class WallComponent : BaseComponent
    {
        private WallType m_WallType;
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

        public override void OnInit(int row, int col, int rot = 0, int color = 0)
        {
            base.OnInit(row, col, rot, color);
            IsRotating = false;
        }

        public void SetupWall(WallType type, bool isHaveHole)
        {
            m_WallType = type;
            m_IsHaveHole = isHaveHole;

            m_TransWallHole.localRotation = Quaternion.Euler(0f, (type == WallType.BOT && isHaveHole) ? 180f : 0f, 0f);
        }

    }
}
