using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseComponent : _BaseBehaviour
    {
        [SerializeField]
        protected WaterComponent[] m_WaterComponent;

        [SerializeField]
        private TerrainComponent m_Terrain;

        [SerializeField]
        protected EffectGridAura m_HintEffect;

        [SerializeField]
        private Transform m_TransWater;

        public int ID => gameObject.GetInstanceID();
        protected Transform Transform => transform;

        public Vector3 CurrentWorldPos => Transform.position;
        private Vector3 CurrentLocalPos => Transform.localPosition;

        // Use set water pos
        private Vector3 CenterPos => Vector3.zero;
        private Vector3 TopPos => Transform.InverseTransformPoint(CurrentWorldPos + Vector3.forward * .5f);
        private Vector3 BotPos => Transform.InverseTransformPoint(CurrentWorldPos + Vector3.back * .5f);
        private Vector3 LeftPos => Transform.InverseTransformPoint(CurrentWorldPos + Vector3.left * .5f);
        private Vector3 RightPos => Transform.InverseTransformPoint(CurrentWorldPos + Vector3.right * .5f);

        //Angle
        protected static int[] TotalRots = new int[] { 0, 90, 180, 270 };

        //Use line checker
        protected virtual int[,] OriginLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 0, 1, 0},
            { 0, 0, 0}
        };

        protected int[,] CurrentLine = new int[3, 3];
        protected int[,] DefaultLine = new int[3, 3];

        public bool IsHaveLineTop => CurrentLine[0, 1] == 1;
        public bool IsHaveLineBot => CurrentLine[2, 1] == 1;
        public bool IsHaveLineLeft => CurrentLine[1, 0] == 1;
        public bool IsHaveLineRight => CurrentLine[1, 2] == 1;

        // Grid static data
        [HideInInspector]
        public int Row, Col, TrueRot, TrueColor;

        public GridType GridType;

        [HideInInspector]
        public int WaterColor;


        // Dynamic data
        protected Color CurrentWaterColor
        {
            get {
                if (Game.isMultiColor)
                {
                    return Config.GetWaterColor(WaterColor);
                }
                else
                {
                    return Config.GetWaterClassicColor(Game.isRandomColorClassicMode, Game.classicColor);
                }
            }
        }

        public bool IsTrueRot => m_CurrentRot == TrueRot;

        protected bool m_IsRotating, m_IsHaveWater, m_IsOldHaveWater;

        protected int m_CurrentRot, m_OldWaterColor, m_WaterCount;

        //Temp
        protected Tween m_Tween;
        private List<int> m_ListWaterColor = new List<int>();
        private BaseComponent m_TempGrid;
        private List<BaseComponent> m_ListGridConnect = new List<BaseComponent>();


        public virtual void OnInit(GridType gridType , int row, int col, int trueRot = 0, int rot = 0, int color = 0, int style = 0)
        {
            GridType = gridType;
            Row = row;
            Col = col;
            WaterColor = color;
            m_CurrentRot = rot;
            TrueRot = trueRot;

            DefaultLine = GetDefaultLine(style);
            CurrentLine = DefaultLine;

            Transform.localRotation = Quaternion.Euler(0f, TotalRots[m_CurrentRot], 0f);

            m_WaterCount = m_WaterComponent.Length;

            if(m_WaterCount > 0)
            {
                for (int i = 0; i < m_WaterCount; i++)
                {
                    m_WaterComponent[i].OnInit();
                }
            }

            if (GridType != GridType.WALL)
            {
                m_Terrain.SetupTerrain(GridHelper.GetTerrainType(Row, Col, Game.listTerrainType));
            }

            if (Game.isStartLevelEffect)
            {
                OnTweenMove(false);
            }
        }

        private void OnMouseDown()
        {
            OnHandleMouseDown();
        }

        protected virtual void OnHandleMouseDown()
        {

        }

        public virtual void OnUpdate(float deltaTime)
        {
            if (m_WaterCount < 1) return;
            for (int i = 0; i < m_WaterCount; i++)
            {
                if (m_WaterComponent[i].gameObject.activeInHierarchy)
                {
                    m_WaterComponent[i].OnUpdate(deltaTime);
                }
            }
        }

        public virtual void OnDestroyMe()
        {
            if (m_HintEffect != null)
            {
                m_HintEffect.OnDestroyMe();
            }

            m_IsRotating = false;
            m_IsHaveWater = false;
            m_IsOldHaveWater = false;
            m_OldWaterColor = 0;

            PrefabHelper.Destroy(gameObject);
        }

        public virtual void AfterInit(bool isMultiColor)
        {
            m_IsRotating = false;
            m_IsHaveWater = (GridType == GridType.GRID_SOURCE);
            m_IsOldHaveWater = false;
            m_OldWaterColor = 0;
        }

        protected virtual int[,] GetDefaultLine(int style)
        {
            return OriginLine;
        }

        public virtual void SetTempRot(int tempRot)
        {

        }



        protected void SetActiveWater(bool isActive)
        {
            for (int i = 0; i < m_WaterCount; i++)
            {
                m_WaterComponent[i].SetGrowing(isActive, TopPos);
            }
        }

        public virtual void PrepareCheckConnect()
        {
            if (GridType == GridType.GRID_SOURCE) return;
            m_IsOldHaveWater = m_IsHaveWater;
            m_OldWaterColor = WaterColor;
            m_IsHaveWater = false;

            m_ListWaterColor.Clear();
        }

        public void SetConnect(int color)
        {
            m_IsHaveWater = true;
            if (!m_ListWaterColor.Contains(color))
            {
                m_ListWaterColor.Add(color);
            }
        }

        private void SetColor()
        {
            if (!m_IsHaveWater) return;
            WaterColor = GridHelper.GetColor(m_ListWaterColor);
            for (int i = 0; i < m_WaterComponent.Length; i++)
            {
                m_WaterComponent[i].SetColor(CurrentWaterColor, !m_IsOldHaveWater);
            }
        }

        public virtual void AfterCheckConnect()
        {
            if (GridType == GridType.GRID_SOURCE) return;
            SetColor();
            if (m_IsOldHaveWater == m_IsHaveWater && m_OldWaterColor == WaterColor) return;
            if (m_IsHaveWater)
            {
                Game.isHaveWaterConnection = true;
            }
            SetActiveWater(m_IsHaveWater);
        }

        public bool IsAllowConnect(int color)
        {
            return (GridType != GridType.GRID_SOURCE && !m_ListWaterColor.Contains(color));
        }

        public List<BaseComponent> GetGridNearCanConnect(int color)
        {
            m_ListGridConnect.Clear();

            if (GridType == GridType.GRID_TARGET) return m_ListGridConnect;

            if (IsHaveLineTop)
            {
                m_TempGrid = Game.GetGrid(Row - 1, Col, color);
                if (m_TempGrid != null)
                {
                    if (m_TempGrid.IsHaveLineBot)
                    {  
                        m_ListGridConnect.Add(m_TempGrid);
                    }
                }
            }

            if (IsHaveLineBot)
            {
                m_TempGrid = Game.GetGrid(Row + 1, Col, color);
                if (m_TempGrid != null)
                {
                    if (m_TempGrid.IsHaveLineTop)
                    {
                        m_ListGridConnect.Add(m_TempGrid);
                    }
                }
            }

            if (IsHaveLineLeft)
            {
                m_TempGrid = Game.GetGrid(Row, Col - 1, color);
                if (m_TempGrid != null)
                {
                    if (m_TempGrid.IsHaveLineRight)
                    {
                        m_ListGridConnect.Add(m_TempGrid);
                    }
                }
            }

            if (IsHaveLineRight)
            {
                m_TempGrid = Game.GetGrid(Row, Col + 1, color);
                if (m_TempGrid != null)
                {
                    if (m_TempGrid.IsHaveLineLeft)
                    {
                        m_ListGridConnect.Add(m_TempGrid);
                    }
                }
            }

            if(GridType == GridType.WALL)
            {
                var m_TempGrid = GridHelper.GetOppositeWall(Row, Col, color, Game);
                if (m_TempGrid != null)
                {
                    m_ListGridConnect.Add(m_TempGrid);
                }
            }

            return m_ListGridConnect;
        }

        public virtual void OnHint()
        {

        }

        public void OnEnableHintEffect(bool isEnable)
        {
            if (isEnable)
            {
                m_HintEffect.StartEffect(CurrentWaterColor);
            }
            else
            {
                m_HintEffect.DisableEffect();
            }
        }

        private void OnTweenMove(bool isDestroy)
        {
            m_Tween?.Kill(true);
            var moveTime = Random.Range(.5f, .75f);
            var delayTime = Random.Range(.25f, .5f);

            if (isDestroy)
            {
                m_Tween = transform.DOLocalMoveY(15f, moveTime).SetEase(Ease.InCirc)
                    .SetDelay(delayTime).OnComplete(() =>
                    {
                        PrefabHelper.Destroy(gameObject);
                    });
            }
            else
            {
                var tempPos = CurrentLocalPos;
                tempPos.y = 15f;
                transform.localPosition = tempPos;

                m_Tween = transform.DOLocalMoveY(0f, moveTime).SetDelay(delayTime);
            }
        }

        private void SetWaterPoint(Vector3 pos)
        {

        }

        private Vector3 GetWaterPos(WaterPoint waterPoint)
        {
            Vector3 pos = Vector3.zero;
            switch (waterPoint)
            {
                case WaterPoint.Top:
                    pos = TopPos;
                    break;
                case WaterPoint.Bot:
                    pos = BotPos;
                    break;
                case WaterPoint.Left:
                    pos = LeftPos;
                    break;
                case WaterPoint.Right:
                    pos = RightPos;
                    break;
                default:
                    pos = CenterPos;
                    break;
            }
            return pos;
        }



        [NaughtyAttributes.Button]
        protected void CheckLine()
        {
            for (int i = 0; i < 3; i++)
            {
                var str = "";
                for (int j = 0; j < 3; j++)
                {
                    str += CurrentLine[i, j];
                }
                Debug.LogError(str);
            }

            Debug.LogError("IsHaveLineTop: " + IsHaveLineTop + " | IsHaveLineBot: " + IsHaveLineBot
                + " | IsHaveLineLeft: " + IsHaveLineLeft + " | IsHaveLineRight: " + IsHaveLineRight);
        }
    }

    public enum WaterPoint
    {
        Center,
        Top,
        Bot,
        Left,
        Right
    }

}
