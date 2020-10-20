using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GridTargetComponent : BaseGrid
    {
        [SerializeField]
        private ParticleSystem m_PsWin;

        protected override int[,] DefaultLine => new int[3, 3]
        {
            { 0, 1, 0},
            { 0, 1, 0},
            { 0, 0, 0}
        };

        public override void SetConnect(int color)
        {
            base.SetConnect(color);
            m_PsWin.Play();
        }
    }
}

