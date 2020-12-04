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
        protected MeshRenderer[] m_LineMeshs;

        public int ID => gameObject.GetInstanceID();
        protected Transform Transform => transform;

        public Vector3 CurrentWorldPos => Transform.position;
        private Vector3 CurrentLocalPos => Transform.localPosition;

        // Use set water pos
        private Vector3 CenterPos => CurrentWorldPos;
        private Vector3 TopPos => CurrentWorldPos + Vector3.forward * .5f;
        private Vector3 BotPos => CurrentWorldPos + Vector3.back * .5f;
        private Vector3 LeftPos => CurrentWorldPos + Vector3.left * .5f;
        private Vector3 RightPos => CurrentWorldPos + Vector3.right * .5f;

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
        //[HideInInspector]
        public int Row, Col, TrueRot, TrueColor;

        public GridType GridType;

        //[HideInInspector]
        public int WaterIndexColor;

        // Dynamic data
        protected Color CurrentWaterColor
        {
            get {
                if (Game.isMultiColor)
                {
                    return Config.GetWaterColor(WaterIndexColor);
                }
                else
                {
                    return Config.GetWaterClassicColor(Game.isRandomColorClassicMode, Game.classicColor);
                }
            }
        }

        public bool IsTrueRot => m_CurrentRot == TrueRot;
        public bool IsAllowRotation => !m_IsRotating;

        protected bool m_IsRotating, m_IsHaveWater;

        public bool m_IsOldHaveWater, isHavingWater;

        public int m_CurrentRot, m_OldWaterIndexColor, m_WaterCount;

        private int m_DelayStart;

        protected WaterPoint m_StartWater;


        //Temp
        protected Tween m_Tween;
        public List<int> m_ListWaterColor = new List<int>();
        private BaseComponent m_TempGrid;
        private List<BaseComponent> m_ListGridConnect = new List<BaseComponent>();

        protected Material[] m_LineMats;


        public virtual void OnInit(GridType gridType , int row, int col, int trueRot = 0, int rot = 0, int color = 0, int style = 0)
        {
            GridType = gridType;
            Row = row;
            Col = col;
            WaterIndexColor = color;
            TrueColor = color;
            m_CurrentRot = rot;
            TrueRot = trueRot;

            DefaultLine = GetDefaultLine(style);
            CurrentLine = DefaultLine;

            m_WaterCount = m_WaterComponent.Length;

            if(m_WaterCount > 0)
            {
                Material waterMat = Game.isHaveNewWaterEffect ? Config.newWaterMat : Config.waterMat;

                for (int i = 0; i < m_WaterCount; i++)
                {
                    m_WaterComponent[i].OnInit(waterMat, OnWaterGrowUpDone);
                }
            }

            if (GridType != GridType.WALL)
            {
                m_Terrain.SetupTerrain(GridHelper.GetTerrainType(Row, Col, Game.listTerrainType));
                transform.localRotation = Quaternion.Euler(0f, TotalRots[m_CurrentRot], 0f);
            }

            if (Game.isStartLevelEffect)
            {
                OnTweenMove(false);
            }

            m_LineMats = Game.abTheme > 0 ? Config.lineNewMats : Config.lineMatDefaultMats;
        }

        private void OnMouseDown()
        {
            OnHandleMouseDown();
        }

        protected virtual void OnHandleMouseDown()
        { 

        }
        protected void OnDOShakePosition()
        {
            if (!Game.isPlaying) return;
            m_Tween?.Kill(true);
            Vector3 shakeVector = new Vector3(0f, 0.05f, 0f);
            m_Tween = transform.DOShakePosition(.5f, shakeVector, 20, 1f, false, true);
            EventController.RaiseEvent(EventList.GamePlay.GAME_LEAVES_EFFECT, new EventParam(this, CurrentWorldPos));
        }

        public virtual void FixLevelData(int newCurrentRot)
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

            OnResetData();

            PrefabHelper.Destroy(gameObject);
        }

        private void OnResetData()
        {
            m_IsRotating = false;
            m_IsHaveWater = false;
            m_IsOldHaveWater = false;
            m_OldWaterIndexColor = 0;
            m_DelayStart = 0;
            isHavingWater = false;
            m_ListWaterColor.Clear();
            m_StartWater = WaterPoint.CENTER;
        }

        public virtual void AfterInit()
        {
            if (GridType != GridType.GRID_SOURCE)
            {
                OnResetData();
            }
        }

        protected virtual int[,] GetDefaultLine(int style)
        {
            return OriginLine;
        }

        public virtual void SetTempRot(int tempRot)
        {

        }

        public virtual void PrepareCheckConnect()
        {
            if(GridType != GridType.GRID_SOURCE)
            {
                m_IsOldHaveWater = m_IsHaveWater;
                m_OldWaterIndexColor = WaterIndexColor;
                m_IsHaveWater = false;
                m_ListWaterColor.Clear();
            }
        }

        public List<BaseComponent> GetGridsCanConnect(int color)
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
                        m_TempGrid.SetStartWater(WaterPoint.BOT);
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
                        m_TempGrid.SetStartWater(WaterPoint.TOP);
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
                        m_TempGrid.SetStartWater(WaterPoint.RIGHT);
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
                        m_TempGrid.SetStartWater(WaterPoint.LEFT);
                        m_ListGridConnect.Add(m_TempGrid);
                    }
                }
            }

            if (GridType == GridType.WALL)
            {
                var m_TempGrid = GridHelper.GetOppositeWall(Row, Col, color, Game);
                if (m_TempGrid != null)
                {
                    m_TempGrid.SetStartWater(WaterPoint.LEFT);
                    m_ListGridConnect.Add(m_TempGrid);
                }
            }

            return m_ListGridConnect;
        }

        protected virtual void OnWaterGrowUpDone()
        {
            isHavingWater = true;
        }

        public int DelayStart => isHavingWater ? 0 : m_DelayStart + 1;

        public void SetStartWater(WaterPoint point)
        {
            if (!Game.isPlaying) return;
            m_StartWater = point;
        }

        public bool IsAllowConnect(int color)
        {
            return (GridType != GridType.GRID_SOURCE && !m_ListWaterColor.Contains(color));
        }

        public void SetConnect(int color, int delayStart)
        {
            m_DelayStart = delayStart;
            m_IsHaveWater = true;
            if (!m_ListWaterColor.Contains(color))
            {
                m_ListWaterColor.Add(color);
            }
        }

        public void SetTotalColor(List<int> color)
        {
            if(GridType != GridType.GRID_SOURCE)
            {
                m_ListWaterColor.AddRange(color);
            }
        }

        public virtual void AfterCheckConnect()
        {
            if (!m_IsHaveWater)
            {
                isHavingWater = false;
            }
            SetColor();
            //if (m_IsOldHaveWater == m_IsHaveWater && m_OldWaterIndexColor == WaterIndexColor) return;
            if (m_IsOldHaveWater == m_IsHaveWater) return;
            WaterPoint point = m_IsHaveWater ? m_StartWater : WaterPoint.CENTER;
            SetSetGrowingWater(m_IsHaveWater, point);

            if (m_IsHaveWater)
            {
                EventController.RaiseEvent(EventList.GamePlay.PLAY_SOUND, new EventParam(SoundName.WATER_CONNECT));
            }
        }

        protected void SetColor()
        {
            if (!m_IsHaveWater) return;
            WaterIndexColor = GridHelper.GetColor(m_ListWaterColor);
            for (int i = 0; i < m_WaterComponent.Length; i++)
            {
                m_WaterComponent[i].SetColor(CurrentWaterColor, !m_IsOldHaveWater);
            }
        }

        protected void SetSetGrowingWater(bool isGrowUp, WaterPoint waterPoint)
        {
            for (int i = 0; i < m_WaterCount; i++)
            {
                m_WaterComponent[i].SetGrowing(isGrowUp, GetWaterPos(waterPoint), m_DelayStart);
            }
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

        private Vector3 GetWaterPos(WaterPoint waterPoint)
        {
            Vector3 pos = Vector3.zero;
            switch (waterPoint)
            {
                case WaterPoint.TOP:
                    pos = TopPos;
                    break;
                case WaterPoint.BOT:
                    pos = BotPos;
                    break;
                case WaterPoint.LEFT:
                    pos = LeftPos;
                    break;
                case WaterPoint.RIGHT:
                    pos = RightPos;
                    break;
                default:
                    pos = CenterPos;
                    break;
            }
            pos.y = .2f;
            return pos;
        }

        public virtual void OnUndo()
        {

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

}
