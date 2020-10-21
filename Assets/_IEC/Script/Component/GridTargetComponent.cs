using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace IEC
{
    public class GridTargetComponent : BaseGrid
    {
        [SerializeField]
        private ParticleSystem m_PsWin;

        [SerializeField]
        private ParticleSystem[] m_PsWins;

        [SerializeField]
        private MeshRenderer[] m_MeshTargets;

        [SerializeField]
        private Transform[] m_TranTargets;

        private Tween m_TweenScale;

        private int m_TrueWaterColor;

        public bool IsDone => IsHaveWater && (m_TrueWaterColor == WaterColor);

        public override void OnInit(GridType gridType, int row, int col, int rot = 0, int color = 0, int style = 0)
        {
            base.OnInit(gridType, row, col, rot, color, style);
            m_TrueWaterColor = color;

            for (int i = 0; i < m_TranTargets.Length; i++)
            {
                m_TranTargets[i].localScale = Vector3.one * .5f;
            }
        }

        public override void AfterInit(bool isMultiColor)
        {
            base.AfterInit(isMultiColor);
            var mat = isMultiColor ? Config.targetMats[m_TrueWaterColor] : Config.targetDefaultMat;

            var color = isMultiColor ? Config.waterColor[m_TrueWaterColor] : Config.targetColorDefault;

            for (int i = 0; i < m_PsWins.Length; i++)
            {
                m_PsWins[i].startColor = color;
            }

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
            if (IsHaveWater)
            {
                if(m_OldWaterColor != WaterColor)
                {
                    m_IsOldStatusWater = !IsHaveWater;
                }
            }

            if (m_IsOldStatusWater == IsHaveWater) return;
            SetActiveWater(IsHaveWater);

            m_TweenScale?.Kill(true);
            Sequence sequence = DOTween.Sequence();
            m_TweenScale = sequence;
            for (int i = 0; i < m_TranTargets.Length; i++)
            {
                sequence.Join(m_TranTargets[i].DOScale(IsDone ? Vector3.one : Vector3.one * .5f, .4f));
            }

            sequence.OnComplete(() => {
                if (IsDone)
                {
                    m_PsWin.Play();
                }
            });
        }

        public override void OnDestroyMe()
        {
            base.OnDestroyMe();
            m_TweenScale?.Kill(true);
        }
    }
}

