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
        private ParticleSystem[] m_PsWins;

        [SerializeField]
        private GameObject[] m_TreeType;

        [SerializeField]
        private MeshRenderer m_GroundMesh, m_LineAroundMesh;

        private Material CurrentMat
        {
            get
            {
                if (TrueColor > 0)
                {
                    return ColorConfig.GetTargetMat(Game.colorVersion, TrueColor);
                }
                else
                {
                    return ColorConfig.GetTargetClassicMat(Game.colorVersion, Game.classicColor);
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

        public bool IsDone => m_IsHaveWater && (TrueColor == WaterColorIndex);
        public bool IsComplete => IsDone && m_IsGrowUpComplete;

        private static float SMALL_SCALE = .4f;
        private static float LARGE_SCALE = 1.5f;

        private Tween m_TweenScale;
        private int m_TargetIndex, m_LineType;
        private List<Transform> m_ListTransTrees = new List<Transform>();
        public List<Renderer> m_ListMeshTrees = new List<Renderer>();
        public Renderer m_TrunkMesh;

        private bool m_IsGrowUpComplete;

        private MaterialPropertyBlock m_TreeMat, m_TrunkMat;
        public float m_FillAmount;

        protected override int[,] GetDefaultLine(int lineType)
        {
            if (lineType == 1)
            {
                return StraightLine;
            }
            else if (lineType == 2)
            {
                return CurveLine;
            }
            else if (lineType == 3)
            {
                return CrossLine;
            }
            else
            {
                return EndLine;
            }
        }

        public override bool IsTrueRot()
        {
            if (m_LineType == 1)
            {
                if (IsOneWayBlock)
                {
                    return m_CurrentRot == TrueRot;
                }
                else if (TrueRot == 0 || TrueRot == 2)
                {
                    return (m_CurrentRot == 0 || m_CurrentRot == 2);
                }
                else
                {
                    return (m_CurrentRot == 1 || m_CurrentRot == 3);
                }
            }
            else
            {
                return m_CurrentRot == TrueRot;
            }
        }

        protected override int GetTrueRotHint()
        {
            if (m_LineType == 1)
            {
                if (IsOneWayBlock)
                {
                    return TrueRot;
                }
                return (m_CurrentRot + 1) % 4;
            }
            else
            {
                return TrueRot;
            }

        }

        public override void OnInit(GridType gridType, int row, int col, int rotTrue = 0, int rot = 0, int color = 0, int lineType = 0)
        {
            base.OnInit(gridType, row, col, rotTrue, rot, color, lineType);

            if(m_TreeMat == null)
            {
                m_TreeMat = new MaterialPropertyBlock();
            }

            if(m_TrunkMat == null)
            {
                m_TrunkMat = new MaterialPropertyBlock();
            }

            m_LineType = lineType;
            // Line
            for (int i = 0; i < 4; i++)
            {
                m_LineStyleObjs[i].SetActive(i == lineType);
            }

            for (int i = 0; i < 4; i++)
            {
                m_WaterComponent[i].gameObject.SetActive(i == lineType);
            }

            m_LineAroundMesh.material = Game.themeConfig.waterLineConfig.listRootTreeMat[lineType];

            var mat = CurrentMat;
            var colorEffect = mat.color;

            for (int i = 0; i < m_PsWins.Length; i++)
            {
                m_PsWins[i].startColor = colorEffect;
            }

            var minIndex = 10;
            var maxIndex = 13;

            m_ListTransTrees.Clear();
            m_TargetIndex = RandomHelper.RangeWithSeed(Game.levelSeed, minIndex, maxIndex);
            Game.levelSeed += 1;

            if (Game.isRandomFlower)
            {
                m_TargetIndex = Random.Range(minIndex, maxIndex);
            }

            for (int i = 0; i < m_TreeType.Length; i++)
            {
                m_TreeType[i].SetActive(i == m_TargetIndex);
            }

            var tree = m_TreeType[m_TargetIndex].transform.GetChild(0);
            m_TrunkMesh = m_TreeType[m_TargetIndex].transform.GetChild(1).GetComponent<Renderer>();

            m_ListMeshTrees.Clear();
            for (int i = 0; i < tree.childCount; i++)
            {
                var itemTree = tree.GetChild(i);
                itemTree.localScale = Vector3.one * SMALL_SCALE;
                var mesh = itemTree.GetComponent<Renderer>();
                m_ListMeshTrees.Add(mesh);
                m_ListTransTrees.Add(itemTree);
            }

            if (Game.isTreeGrayColor)
            {
                m_TrunkMesh.material = ColorConfig.trunkGrayMat;
                m_ListMeshTrees.ForEach(x => x.material = ColorConfig.treeGrayMat);

                m_FillAmount = 0f;
                OnUpdateFillAmount(m_FillAmount);
                SetupColor(CurrentWaterColor);
            }
            else
            {
                if (!Game.isMultiColor)
                {
                    if (m_TargetIndex > 10)
                    {
                        var newMat = ColorConfig.targetDefaultMatNew;

                        m_ListMeshTrees.ForEach(x => x.material = newMat);
                    }
                }
                else
                {
                    m_ListMeshTrees.ForEach(x => x.material = mat);
                }
                
            }

            if (Game.isRootTreeOnlyGreenColor)
            {
                m_GroundMesh.material = ColorConfig.targetDefaultMat;
            }
            else
            {
                m_GroundMesh.material = mat;
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (m_IsScaling)
            {
                var ratio = (m_ListTransTrees[0].lossyScale.x - .4f) / 1.1f;
                m_FillAmount = ratio * 1.5f;
                OnUpdateFillAmount(m_FillAmount);
            }

            if (m_IsMoving)
            {
                UpdateColorPos(true);
            }
        }

        private void SetupColor(Color color)
        {
            m_TreeMat.SetColor(GameShaderValue.SHADER_COLOR, color);
        }

        private void UpdateColorPos(bool isTweenMoving)
        {
            var pos = CurrentWorldPos;
            if (!isTweenMoving)
            {
                pos.y = -.11f;
            }
            
            m_TreeMat.SetVector(GameShaderValue.SHADER_POS, pos);
            m_TrunkMat.SetVector(GameShaderValue.SHADER_POS, pos);

            ApplyData();
        }

        private void OnUpdateFillAmount(float fillAmount)
        {
            m_TreeMat.SetFloat(GameShaderValue.SHADER_FILL_AMOUNT, fillAmount);
            m_TrunkMat.SetFloat(GameShaderValue.SHADER_FILL_AMOUNT, fillAmount);
            ApplyData();
        }

        private void ApplyData()
        {
            m_ListMeshTrees.ForEach(x => x.SetPropertyBlock(m_TreeMat));
            m_TrunkMesh.SetPropertyBlock(m_TrunkMat);
        }


        public override void AfterCheckConnect()
        {
            if (!m_IsHaveWater)
            {
                m_IsHavingWater = false;
                m_IsGrowUpComplete = false;
            }
            SetColor();

            if (m_IsOldHaveWater == m_IsHaveWater)
            {
                if (IsDone)
                {
                    if (!m_IsGrowUpComplete)
                    {
                        OnWaterGrowUpDone();
                    }
                }
                else
                {
                    m_IsGrowUpComplete = false;
                    TweenScale(false);
                }
            }
            else
            {
                if (!m_IsHaveWater || !IsDone)
                {
                    m_IsGrowUpComplete = false;
                    TweenScale(false);
                }
                StartPoint point = m_IsHaveWater ? m_StartWater : StartPoint.CENTER;
                SetGrowingWater(m_IsHaveWater, point);

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
                m_PsWins[0].Play();
                TweenScale(true, () => {
                    m_IsGrowUpComplete = true;
                    if (PlayerData.isVibrateOn)
                    {
                        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                    }
                    EventController.RaiseEvent(EventList.GamePlay.PLAY_SOUND, new EventParam(SoundName.TARGET_DONE));
                    EventController.RaiseEvent(EventList.GamePlay.GAME_CHECK_COMPLETE);
                });
            }
        }

        public bool m_IsScaling;
        private void TweenScale(bool isGrowUp, System.Action callBack = null)
        {
            m_TweenScale?.Kill(false);
            Sequence sequence = DOTween.Sequence();

            var ease = isGrowUp ? Ease.OutBounce : Ease.Unset;
            m_IsScaling = true;

            var timeScale = Game.isTreeGrayColor ? 1f : .75f;

            m_TweenScale = sequence.OnComplete(() => {
                callBack?.Invoke();
                m_IsScaling = false;
            });

            for (int i = 0; i < m_ListTransTrees.Count; i++)
            {
                sequence.Join(m_ListTransTrees[i].DOScale(isGrowUp ? Vector3.one * LARGE_SCALE : Vector3.one * SMALL_SCALE, timeScale)
                .SetEase(ease));
            }

            sequence.Play();
        }

        public override void OnDestroyMe()
        {
            m_TweenScale?.Kill(true);
            m_IsGrowUpComplete = false;
            m_ListTransTrees.Clear();
            base.OnDestroyMe();
        }
    }
}

