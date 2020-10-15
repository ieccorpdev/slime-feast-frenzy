using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseGrid : BaseComponent
    {
        protected Tween m_Tween;

        [HideInInspector]
        public bool IsRotating;

        protected static int[] TotalRots = new int[] { 0, 90, 180, 270 };

        public void OnInit(int rot)
        {
            transform.localRotation = Quaternion.Euler(0f, TotalRots[rot], 0f);
        }
    }
}
