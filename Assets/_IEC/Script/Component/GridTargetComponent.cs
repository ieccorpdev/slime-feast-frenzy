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

        public override void AfterCheckConnect()
        {
            base.AfterCheckConnect();
            if (m_IsStatusWater == IsHaveWater) return;
            if (IsHaveWater)
            {
                m_PsWin.Play();
            }
        }
    }
}

