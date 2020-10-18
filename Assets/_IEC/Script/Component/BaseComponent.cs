using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseComponent : _BaseBehaviour
    {
        public int ID => gameObject.GetInstanceID();

        public virtual bool IsCanRotate() => false;

        [HideInInspector]
        public bool IsHaveWater, IsRotating;

        [HideInInspector]
        public int WaterColor, Row, Col;

        public int m_RotY;

        protected virtual int[,] DefaultLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 0, 1, 0},
            { 0, 0, 0}
        };

        protected int[,] CurerentLine = new int[3, 3];

        public bool IsTopHaveWater() => IsHaveWater && CurerentLine[0, 1] == 1;
        public bool IsBotHaveWater() => IsHaveWater && CurerentLine[2, 1] == 1;
        public bool IsLeftHaveWater() => IsHaveWater && CurerentLine[1, 0] == 1;
        public bool IsRightHaveWater() => IsHaveWater && CurerentLine[1, 2] == 1;

        public virtual void OnInit(int row, int col, int rot = 0, int color = 0)
        {
            Row = row;
            Col = col;

            CurerentLine = DefaultLine;
        }

    }
}
