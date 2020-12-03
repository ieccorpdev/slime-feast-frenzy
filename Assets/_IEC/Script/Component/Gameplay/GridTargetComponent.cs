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

        private int m_TargetStyle;

        public bool IsDone => m_IsHaveWater && (TrueColor == WaterIndexColor);

        public bool IsGrowUpComplete;

        public bool IsComplete => IsDone && IsGrowUpComplete;

        private static float SMALL_SCALE = .3f;
        private static float LARGE_SCALE = 1.5f;

        private Material CurrentMat
        {
            get
            {
                if (Game.isMultiColor)
                {
                    return Config.GetWaterMat(TrueColor);
                }
                else
                {
                    return Config.GetWaterClassicMat(Game.isRandomColorClassicMode, Game.classicColor);
                }
            }
        }

        protected override int[,] OriginLine => new int[3, 3]
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
            { 0, 0, 0},
            { 1, 1, 1},
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

        public override void OnInit(GridType gridType, int row, int col, int rotTrue = 0, int rot = 0, int color = 0, int style = 0)
        {
            base.OnInit(gridType, row, col, rotTrue, rot, color, style);
            
            m_TargetStyle = RandomHelper.RangeWithSeed(Game.seed, 0, m_TargetStyles.Length);

            Game.seed += 1;

            for (int i = 0; i < m_TargetStyles.Length; i++)
            {
                m_TargetStyles[i].SetActive(i == m_TargetStyle);
            }

            m_TranTargets[m_TargetStyle].localScale = Vector3.one * SMALL_SCALE;

            // Line
            for (int i = 0; i < m_StyleLineObj.Length; i++)
            {
                m_StyleLineObj[i].SetActive(i == style);
            }

            for (int i = 0; i < m_WaterCount; i++)
            {
                m_WaterComponent[i].gameObject.SetActive(i == style);
            }

            var mat = CurrentMat;
            var colorEffect = mat.color;
            for (int i = 0; i < m_PsWins.Length; i++)
            {
                m_PsWins[i].startColor = colorEffect;
            }

            for (int i = 0; i < m_MeshTargets.Length; i++)
            {
                m_MeshTargets[i].material = mat;
            }

            for (int i = 0; i < m_LineMeshs.Length; i++)
            {
                m_LineMeshs[i].material = m_LineMats[i];
            }
        }


        public override void AfterCheckConnect()
        {
            base.AfterCheckConnect();

            if (!m_IsHaveWater)
            {
                IsGrowUpComplete = false;
            }
            if (m_IsOldHaveWater == m_IsHaveWater && m_OldWaterIndexColor == WaterIndexColor) return;
            if (!m_IsHaveWater || !IsDone)
            {
                IsGrowUpComplete = false;
                TweenScale(false);
            }
        }

        protected override void OnWaterGrowUpDone()
        {
            if (IsDone)
            {
                TweenScale(true, () => {
                    IsGrowUpComplete = true;

                    m_PsWin.Play();
                    if (PlayerData.isVibrateOn)
                    {
                        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                    }
                    EventController.RaiseEvent(EventList.GamePlay.PLAY_SOUND, new EventParam(SoundName.TARGET_DONE));
                    EventController.RaiseEvent(EventList.GamePlay.GAME_CHECK_COMPLETE);
                });
            }
        }

        private void TweenScale(bool isGrowUp, System.Action callBack = null)
        {
            m_TweenScale?.Kill(true);
            m_TweenScale = m_TranTargets[m_TargetStyle].DOScale(isGrowUp ? Vector3.one * LARGE_SCALE : Vector3.one * SMALL_SCALE, .4f)
                .OnComplete(() => {
                    callBack?.Invoke();
                });
        }

        public override void OnDestroyMe()
        {
            base.OnDestroyMe();
            m_TweenScale?.Kill(true);
            IsGrowUpComplete = false;
        }
    }
}

