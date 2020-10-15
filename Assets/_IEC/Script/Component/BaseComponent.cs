using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseComponent : _BaseBehaviour
    {
        public int ID => gameObject.GetInstanceID();

        public virtual bool IsCanRotate() => false;

        public bool IsHaveWater;

        public int WaterColor, Row, Col;

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

    }
}
