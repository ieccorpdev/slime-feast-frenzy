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

        [SerializeField]
        private MeshRenderer m_RootTreeMesh, m_TreeBaseMesh;

        [SerializeField]
        private GameObject m_TreeBaseOldObj, m_TreeBaseNewObj;

        private Tween m_TweenScale;

        private int m_TargetIndex;

        private List<Transform> m_ListTransTrees = new List<Transform>();

        public List<Renderer> m_ListMeshTrees = new List<Renderer>();

        public Renderer m_TrunkMesh;

        public bool IsDone => m_IsHaveWater && (TrueColor == WaterIndexColor);

        private bool IsGrowUpComplete;

        public bool IsComplete => IsDone && IsGrowUpComplete;

        private static float SMALL_SCALE = .4f;
        private static float LARGE_SCALE = 1.5f;

        private static int Shader_FillAmount = Shader.PropertyToID("_FillAmount");
        private static int Shader_Color = Shader.PropertyToID("_Color");
        private static int Shader_Pos = Shader.PropertyToID("_GLOBALMaskPosition");

        private MaterialPropertyBlock m_TreeMat, m_TrunkMat;

        public float m_FillAmount;

        private Material CurrentMat
        {
            get
            {
                if (TrueColor > 0)
                {
                    return Config.GetWaterMat(Game.isTreeColorV1 || Game.isTreeColorV2, TrueColor);
                }
                else
                {
                    return Config.GetWaterClassicMat(Game.isRandomColorClassicMode, Game.isTreeColorV1 || Game.isTreeColorV2, Game.classicColor);
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

        private int m_LineType;

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

            for (int i = 0; i < m_WaterCount - 1; i++)
            {
                m_WaterComponent[i].gameObject.SetActive(i == lineType);
            }

            bool isNewBaseTree = Game.isRootTreeChangeColorByWater;

            m_WaterComponent[m_WaterCount - 1].gameObject.SetActive(isNewBaseTree);
            m_WaterComponent[m_WaterCount - 1].SetMainWater(false);

            m_TreeBaseMesh.material = ShopConfig.treebaseMats[lineType];

            m_TreeBaseOldObj.SetActive(!isNewBaseTree);
            m_TreeBaseNewObj.SetActive(isNewBaseTree);

            var mat = CurrentMat;
            var colorEffect = mat.color;

            for (int i = 0; i < m_PsWins.Length; i++)
            {
                m_PsWins[i].startColor = colorEffect;
            }

            var minIndex = Game.isHaveNewTrees ? 4 : 0;
            var maxIndex = Game.isHaveNewTrees ? 15 : 5;

            m_ListTransTrees.Clear();
            m_TargetIndex = RandomHelper.RangeWithSeed(Game.levelSeed, minIndex, maxIndex);
            Game.levelSeed += 1;

            for (int i = 0; i < m_TreesStyles.Length; i++)
            {
                m_TreesStyles[i].SetActive(i == m_TargetIndex);
            }

            var tree = m_TreesStyles[m_TargetIndex].transform.GetChild(0);
            m_TrunkMesh = m_TreesStyles[m_TargetIndex].transform.GetChild(1).GetComponent<Renderer>();

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
                m_TrunkMesh.material = Config.trunkGrayMat;
                m_ListMeshTrees.ForEach(x => x.material = Config.treeGrayMat);

                SetupColor(CurrentWaterColor);
            }
            else
            {
                m_ListMeshTrees.ForEach(x => x.material = mat);
            }

            if (Game.isRootTreeOnlyGreenColor)
            {
                m_RootTreeMesh.material = Config.targetDefaultMat;
            }
            else
            {
                m_RootTreeMesh.material = mat;
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (m_IsScaling)
            {
                var ratio = (m_ListTransTrees[0].lossyScale.x - .4f) / 1.1f;
                m_FillAmount = ratio * 1.5f;
                OnUpdatePropertyBlock();
            }
        }

        private void SetupColor(Color color)
        {
            m_TreeMat.SetColor(Shader_Color, color);

            var pos = CurrentWorldPos;
            pos.y = -.11f;
            m_TreeMat.SetVector(Shader_Pos, pos);
            m_TrunkMat.SetVector(Shader_Pos, pos);

            ApplyData();
        }

        private void OnUpdatePropertyBlock()
        {
            m_TreeMat.SetFloat(Shader_FillAmount, m_FillAmount);
            m_TrunkMat.SetFloat(Shader_FillAmount, m_FillAmount);
            ApplyData();
        }

        private void ApplyData()
        {
            m_ListMeshTrees.ForEach(x => x.SetPropertyBlock(m_TreeMat));
            m_TrunkMesh.SetPropertyBlock(m_TrunkMat);
        }


        public override void AfterCheckConnect()
        {
            if ((Game.isChangeGridStatus || Game.isGreenifyHydrated) && Game.isPlaying)
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
            IsGrowUpComplete = false;
            m_ListTransTrees.Clear();
            base.OnDestroyMe();
        }
    }
}

