using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.NiceVibrations;

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

        [SerializeField]
        private GameObject[] m_TargetStyles;

        [SerializeField]
        private GameObject[] m_StyleLineObj;

        private Tween m_TweenScale;

        private int m_TrueWaterColor, m_TargetStyle;

        public bool IsDone => IsHaveWater && (m_TrueWaterColor == WaterColor);

        protected override int[,] DefaultLine => new int[3, 3]
{
            { 0, 0, 0},
            { 0, 1, 0},
            { 0, 0, 0}
};

        private int[,] EndLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 0},
            { 0, 0, 0}
        };

        private int[,] CrossLine => new int[3, 3]
        {
            { 0, 1, 0},
            { 1, 1, 0},
            { 0, 1, 0}
        };

        private int[,] CurveLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 0},
            { 0, 1, 0}
        };

        private int[,] StraightLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 1},
            { 0, 0, 0}
        };

        protected override int[,] GetDefaultLine(int style)
        {
            if (style == 1)
            {
                return StraightLine;
            }
            else if (style == 2)
            {
                return CurveLine;
            }
            else if (style == 3)
            {
                return CrossLine;
            }
            else
            {
                return EndLine;
            }
        }

        public override void OnInit(GridType gridType, int row, int col, int rot = 0, int color = 0, int style = 0)
        {
            base.OnInit(gridType, row, col, rot, color, style);
            m_TrueWaterColor = color;

            m_TargetStyle = RandomHelper.RangeWithSeed(Game.seed, 0, m_TargetStyles.Length);

            Game.seed += 1;

            for (int i = 0; i < m_TargetStyles.Length; i++)
            {
                m_TargetStyles[i].SetActive(i == m_TargetStyle);
            }

            m_TranTargets[m_TargetStyle].localScale = Vector3.one * .5f;

            // Line
            for (int i = 0; i < m_StyleLineObj.Length; i++)
            {
                m_StyleLineObj[i].SetActive(i == style);
            }

            for (int i = 0; i < m_WaterComponent.Length; i++)
            {
                m_WaterComponent[i].gameObject.SetActive(i == style);
            }
        }

        public override void AfterInit(bool isMultiColor)
        {
            base.AfterInit(isMultiColor);
            var mat = Config.GetWaterColorMat(m_TrueWaterColor, isMultiColor);

            var color = mat.color;

            for (int i = 0; i < m_PsWins.Length; i++)
            {
                m_PsWins[i].startColor = color;
            }

            for (int i = 0; i < m_MeshTargets.Length; i++)
            {
                m_MeshTargets[i].material = mat;
            }
        }

        public override void AfterCheckConnect()
        {
            base.AfterCheckConnect();

            if (m_IsOldStatusWater == IsHaveWater && m_OldWaterColor == WaterColor) return;
            m_TweenScale?.Kill(true);
            m_TweenScale = m_TranTargets[m_TargetStyle].DOScale(IsDone ? Vector3.one : Vector3.one * .1f, .4f)
                .OnComplete(() => {
                    if (IsDone)
                    {
                        m_PsWin.Play();
                        if (PlayerData.isVibrateOn)
                        {
                            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                        }
                        EventController.RaiseEvent(EventList.GamePlay.PLAY_SOUND, new EventParam(SoundName.TARGET_DONE));
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

