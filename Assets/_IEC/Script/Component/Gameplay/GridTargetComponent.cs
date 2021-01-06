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
        private GameObject[] m_TreesStyles;

        private Tween m_TweenScale;

        private int m_TargetIndex;

        private List<Transform> m_ListTransTrees = new List<Transform>();

        public bool IsDone => m_IsHaveWater && (TrueColor == WaterIndexColor);

        private bool IsGrowUpComplete;

        public bool IsComplete => IsDone && IsGrowUpComplete;

        private static float SMALL_SCALE = .4f;
        private static float LARGE_SCALE = 1.5f;

        //private Material CurrentMat
        //{
        //    get
        //    {
        //        if (Game.isMultiColor)
        //        {
        //            return Config.GetWaterMat(Game.isNewTreeColor, TrueColor);
        //        }
        //        else
        //        {
        //            return Config.GetWaterClassicMat(Game.isRandomColorClassicMode, Game.isNewTreeColor, Game.classicColor);
        //        }
        //    }
        //}

        private Material CurrentMat
        {
            get
            {
                if (TrueColor > 0)
                {
                    return Config.GetWaterMat(Game.isNewTreeColor, TrueColor);
                }
                else
                {
                    return Config.GetWaterClassicMat(Game.isRandomColorClassicMode, Game.isNewTreeColor, Game.classicColor);
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

            // Line
            for (int i = 0; i < m_LineTyleObjs.Length; i++)
            {
                m_LineTyleObjs[i].SetActive(i == style);
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

            var minIndex = Game.isHaveNewTrees ? 4 : 0;
            var maxIndex = Game.isHaveNewTrees ? 15 : 5;

            m_TargetIndex = RandomHelper.RangeWithSeed(Game.levelSeed, minIndex, maxIndex);
            Game.levelSeed += 1;

            for (int i = 0; i < m_TreesStyles.Length; i++)
            {
                m_TreesStyles[i].SetActive(i == m_TargetIndex);
            }

            var tree = m_TreesStyles[m_TargetIndex].transform.GetChild(0);

            for (int i = 0; i < tree.childCount; i++)
            {
                var itemTree = tree.GetChild(i);
                itemTree.localScale = Vector3.one * SMALL_SCALE;
                itemTree.GetComponent<MeshRenderer>().material = mat;

                m_ListTransTrees.Add(itemTree);
            }
        }


        public override void AfterCheckConnect()
        {
            if (Game.isChangeGridStatus && Game.isPlaying)
            {
                SetTerrainMat();
            }

            if (!m_IsHaveWater)
            {
                m_IsHavingWater = false;
                IsGrowUpComplete = false;
            }
            SetColor();

            if (m_IsOldHaveWater == m_IsHaveWater)
            {
                if (IsDone)
                {
                    if (!IsGrowUpComplete)
                    {
                        OnWaterGrowUpDone();
                    }
                }
                else
                {
                    IsGrowUpComplete = false;
                    TweenScale(false);
                }
            }
            else
            {
                if (!m_IsHaveWater || !IsDone)
                {
                    IsGrowUpComplete = false;
                    TweenScale(false);
                }
                WaterPoint point = m_IsHaveWater ? m_StartWater : WaterPoint.CENTER;
                SetSetGrowingWater(m_IsHaveWater, point);

                if (m_IsHaveWater)
                {
                    EventController.RaiseEvent(EventList.GamePlay.PLAY_SOUND, new EventParam(SoundName.WATER_CONNECT));
                }
            }

        }

        protected override void OnWaterGrowUpDone()
        {
            if (IsDone)
            {
                m_PsWin.Play();
                TweenScale(true, () => {
                    IsGrowUpComplete = true;
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
            Sequence sequence = DOTween.Sequence();

            var ease = isGrowUp ? Ease.OutBounce : Ease.Unset;

            m_TweenScale = sequence.OnComplete(() => {
                callBack?.Invoke();
            });
            for (int i = 0; i < m_ListTransTrees.Count; i++)
            {
                sequence.Join(m_ListTransTrees[i].DOScale(isGrowUp ? Vector3.one * LARGE_SCALE : Vector3.one * SMALL_SCALE, .75f)
                .SetEase(ease));
            }

            sequence.Play();
        }

        public override void OnDestroyMe()
        {
            base.OnDestroyMe();
            m_TweenScale?.Kill(true);
            IsGrowUpComplete = false;
        }
    }
}

