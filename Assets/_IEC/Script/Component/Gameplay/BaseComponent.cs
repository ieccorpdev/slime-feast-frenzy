using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseComponent : _BaseBehaviour
    {
        [SerializeField]
        private TerrainComponent m_Terrain;

        [SerializeField]
        protected EffectGridAura m_HintEffect;

        [SerializeField]
        protected WaterComponent[] m_WaterComponent;

        [SerializeField]
        protected GameObject[] m_LineStyleObjs;

        [SerializeField]
        private Collider m_Collider;

        #region Get value
        public int ID => gameObject.GetInstanceID();
        protected Transform Transform => transform;
        public Vector3 CurrentWorldPos => transform.position;
        private Vector3 CurrentLocalPos => transform.localPosition;

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

        public bool IsHaveLineTop => CurrentLine[0, 1] == 1;
        public bool IsHaveLineBot => CurrentLine[2, 1] == 1;
        public bool IsHaveLineLeft => CurrentLine[1, 0] == 1;
        public bool IsHaveLineRight => CurrentLine[1, 2] == 1;

        #endregion

        public int[,] CurrentLine = new int[3, 3];
        protected int[,] DefaultLine = new int[3, 3];

        public int Row, Col, TrueRot, TrueColor, WaterColorIndex;

        public GridType GridType;

        // Dynamic data
        protected Color CurrentWaterColor
        {
            get
            {
                if (WaterColorIndex > 0)
                {
                    return ColorConfig.GetWaterColor(WaterColorIndex);
                }
                else
                {
                    return ColorConfig.GetWaterClassicColor(Game.isRandomColorClassicMode, Game.classicColor);
                }
            }
        }

        public virtual bool IsTrueRot() => m_CurrentRot == TrueRot;
        public bool IsAllowRotation => !m_IsRotating;

        protected bool m_IsInit, m_IsRotating, m_IsHaveWater, m_IsLock, 
            m_IsOldHaveWater, m_IsHavingWater, m_IsMoving;

        protected int m_CurrentRot, m_OldWaterIndexColor, m_WaterCount;

        private int m_DelayStart;

        protected StartPoint m_StartWater;
        public StartPoint Inline;

        public int m_InlineOrigin, m_InlineOffset;
        public bool IsOneWayBlock, IsInBigSquare;

        private bool IsHaveTerrain => m_Terrain != null;

        //Temp
        protected Tween m_Tween;
        protected List<int> m_ListWaterColor = new List<int>();
        private Vector3 m_StartPos1, m_EndPos1, m_StartPos2, m_EndPos2;


        public virtual void OnInit(GridType gridType , int row, int col, int trueRot = 0, int rot = 0, int color = 0, int style = 0)
        {
            OnResetAll();

            GridType = gridType;
            Row = row;
            Col = col;
            WaterColorIndex = color;
            TrueColor = color;
            m_CurrentRot = rot;
            TrueRot = trueRot;

            DefaultLine = GetDefaultLine(style);
            CurrentLine = DefaultLine;
            OnUpdateSetTerrain();
        }

        private void OnMouseDown()
        {
            OnHandleMouseDown();
        }

        protected virtual void OnHandleMouseDown()
        {

        }

        private void OnResetAll()
        {
            if (!m_IsInit)
            {
                m_IsInit = true;
                m_WaterCount = m_WaterComponent.Length;
            }

            if (m_WaterCount > 0)
            {
                for (int i = 0; i < m_WaterCount; i++)
                {
                    m_WaterComponent[i].OnInit(OnWaterGrowUpDone, OnGrowUping);
                }
            }

            m_Tween?.Kill(true);
            if (m_HintEffect != null)
            {
                m_HintEffect.OnDestroyMe();
            }

            IsOneWayBlock = false;
            IsInBigSquare = false;

            SetActiveLock(false);

            if (IsHaveTerrain)
            {
                m_Terrain.ResetAll();
            }

            OnResetData();
        }

        private void OnResetData()
        {
            m_IsRotating = false;
            m_IsHaveWater = false;
            m_IsOldHaveWater = false;
            m_OldWaterIndexColor = 0;
            m_DelayStart = 0;
            m_IsHavingWater = false;
            m_ListWaterColor.Clear();
            m_StartWater = StartPoint.CENTER;

        }

        public virtual void OnUpdate(float deltaTime)
        {
            if (m_WaterCount < 1) return;
            for (int i = 0; i < m_WaterCount; i++)
            {
                var water = m_WaterComponent[i];
                water.OnUpdate(deltaTime);
                water.UpdatePos(GetWaterPos(m_StartWater));
            }
        }

        public virtual void OnDestroyMe()
        {
            OnResetAll();
            PrefabHelper.Destroy(gameObject);
        }

        public void SetNewPos(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public virtual void SetLock()
        {
            SetActiveLock(true);
            if (IsHaveTerrain)
            {
                m_Terrain.SetActiveLock(true);
            }
        }

        public void PushToBigSquare()
        {
            SetActiveLock(true);
            IsInBigSquare = true;
            m_Terrain.gameObject.SetActive(false);
        }

        private void SetActiveLock(bool isActive)
        {
            m_IsLock = isActive;

            if (m_Collider)
            {
                m_Collider.enabled = !isActive;
            }
        }

        public virtual void OnUpdateSetTerrain()
        {
            var listMat = Game.isFlyingIsland ? (PlayerData.themeSelected == 0 ?
                ShopConfig.listTerrainIsLand1 : ShopConfig.listTerrainIsLand2) : Game.listTerrainType;
            var terrainMat = GridHelper.GetTerrainType(Row, Col, listMat);
            m_Terrain.SetupTerrain(terrainMat, Game.isBlockGrayColor, Game.isFlyingIsland, false);

            for (int i = 0; i < 4; i++)
            {
                if (m_LineStyleObjs[i] != null)
                {
                    m_LineStyleObjs[i].GetComponent<MeshRenderer>().material = 
                        Game.themeConfig.waterLineConfig.listWaterLineMat[i];
                }
            }
        }

        private void OnGrowUping(float fillAmount)
        {
            if (Game.isBlockGrayColor)
            {
                if (!m_IsLock)
                {
                    if (IsHaveTerrain)
                    {
                        m_Terrain.OnUpdate(fillAmount);
                    }
                }
            }
        }

        public virtual void AfterInit()
        {
            OnResetData();
        }

        public virtual void BeforeInit()
        {

        }

        protected virtual int[,] GetDefaultLine(int style)
        {
            return OriginLine;
        }

        public void PrepareCheckConnect()
        {
            if(GridType != GridType.GRID_SOURCE)
            {
                m_IsOldHaveWater = m_IsHaveWater;
                m_OldWaterIndexColor = WaterColorIndex;
                m_IsHaveWater = false;
                m_ListWaterColor.Clear();
            }
        }

        private void AddGridToListConnect( StartPoint point, bool isInit = false)
        {
            GamePlay.listGridConnectCache.Add(GamePlay.gridCache);
            GamePlay.gridCache.SetStartWater(point);

            if (isInit)
            {
                GamePlay.gridCache.SetupArrow(point);
            }
        }

        public List<BaseComponent> GetGridsCanConnect(int color, bool isInit)
        {
            GamePlay.listGridConnectCache.Clear();
            if (GridType == GridType.GRID_TARGET) return GamePlay.listGridConnectCache;

            if (IsHaveLineTop)
            {
                GamePlay.gridCache = Game.GetGrid(Row - 1, Col, color);
                if (GamePlay.gridCache != null)
                {
                    if (GamePlay.gridCache.IsHaveLineBot)
                    {
                        if (isInit)
                        {
                            AddGridToListConnect(StartPoint.BOT, true);
                        }
                        else
                        {
                            if(Game.isHaveOneWayBlock && GamePlay.gridCache.IsOneWayBlock)
                            {
                                if(GamePlay.gridCache.Inline == StartPoint.BOT)
                                {
                                    AddGridToListConnect(StartPoint.BOT);
                                }
                            }
                            else
                            {
                                AddGridToListConnect(StartPoint.BOT);
                            }
                        }
                    }
                }
            }

            if (IsHaveLineBot)
            {
                GamePlay.gridCache = Game.GetGrid(Row + 1, Col, color);
                if (GamePlay.gridCache != null)
                {
                    if (GamePlay.gridCache.IsHaveLineTop)
                    {
                        if (isInit)
                        {
                            AddGridToListConnect(StartPoint.TOP, true);
                        }
                        else
                        {
                            if (Game.isHaveOneWayBlock && GamePlay.gridCache.IsOneWayBlock)
                            {
                                if (GamePlay.gridCache.Inline == StartPoint.TOP)
                                {
                                    AddGridToListConnect(StartPoint.TOP);
                                }
                            }
                            else
                            {
                                AddGridToListConnect(StartPoint.TOP);
                            }
                        }
                    }
                }
            }

            if (IsHaveLineLeft)
            {
                GamePlay.gridCache = Game.GetGrid(Row, Col - 1, color);
                if (GamePlay.gridCache != null)
                {
                    if (GamePlay.gridCache.IsHaveLineRight)
                    {
                        if (isInit)
                        {
                            AddGridToListConnect(StartPoint.RIGHT, true);
                        }
                        else
                        {
                            if (Game.isHaveOneWayBlock && GamePlay.gridCache.IsOneWayBlock)
                            {
                                if (GamePlay.gridCache.Inline == StartPoint.RIGHT)
                                {
                                    AddGridToListConnect(StartPoint.RIGHT);
                                }
                            }
                            else
                            {
                                AddGridToListConnect(StartPoint.RIGHT);
                            }
                        }
                    }
                }
            }

            if (IsHaveLineRight)
            {
                GamePlay.gridCache = Game.GetGrid(Row, Col + 1, color);
                if (GamePlay.gridCache != null)
                {
                    if (GamePlay.gridCache.IsHaveLineLeft)
                    {
                        if (isInit)
                        {
                            AddGridToListConnect(StartPoint.LEFT, true);
                        }
                        else
                        {
                            if (Game.isHaveOneWayBlock && GamePlay.gridCache.IsOneWayBlock)
                            {
                                if (GamePlay.gridCache.Inline == StartPoint.LEFT)
                                {
                                    AddGridToListConnect(StartPoint.LEFT);
                                }
                            }
                            else
                            {
                                AddGridToListConnect(StartPoint.LEFT);
                            }
                        }
                    }
                }
            }

            if (GridType == GridType.WALL)
            {
                GamePlay.gridCache = GridHelper.GetOppositeWall(Row, Col, color, Game);
                if (GamePlay.gridCache != null)
                {
                    AddGridToListConnect(StartPoint.LEFT);
                }
            }

            return GamePlay.listGridConnectCache;
        }

        protected virtual void OnWaterGrowUpDone()
        {
            m_IsHavingWater = true;
        }

        public int DelayStart => m_IsHavingWater ? 0 : m_DelayStart + 1;

        public void SetStartWater(StartPoint point)
        {
            if (!Game.isPlaying) return;
            m_StartWater = point;
        }

        public void SetupArrow(StartPoint point)
        {
            if (!Game.isHaveOneWayBlock) return;
            if (GridType == GridType.GRID_STRAIGHT || GridType == GridType.GRID_CURVE)
            {
                OnSetUpArrow(point);
            }
        }

        private void OnSetUpArrow(StartPoint point)
        {
            var ratioDist = Game.isHaveIntroOutro ? .1f : 2f;
            m_InlineOrigin = (int)point;

            m_InlineOffset = LevelHelper.CalculatorMissingRot(m_InlineOrigin, TrueRot);

            var originStartPos = GetWaterPos(point);
            originStartPos.y = 0f;
            var dir1 = (CenterPos - originStartPos).normalized;

            m_StartPos1 = originStartPos + (dir1 * ratioDist);
            m_EndPos1 = CenterPos + (-dir1 * ratioDist);

            var originDestinationPos = GetDestination(point);
            originDestinationPos.y = 0f;
            var dir2 = (originDestinationPos - CenterPos).normalized;

            m_StartPos2 = CenterPos + (dir2 * ratioDist);
            m_EndPos2 = originDestinationPos + (-dir2 * ratioDist);

            
            m_Terrain.SetArrowPos(m_StartPos1, m_EndPos1, m_StartPos2, m_EndPos2);
        }

        private Vector3 GetDestination(StartPoint point)
        {
            if (IsHaveLineTop && point != StartPoint.TOP)
            {
                return TopPos;
            }

            if (IsHaveLineBot && point != StartPoint.BOT)
            {
                return BotPos;
            }

            if (IsHaveLineLeft && point != StartPoint.LEFT)
            {
                return LeftPos;
            }

            if (IsHaveLineRight && point != StartPoint.RIGHT)
            {
                return RightPos;
            }

            return CenterPos;
        }

        public void OnSetActiveArrow()
        {
            IsOneWayBlock = true;
            m_Terrain.SetActiveArrow();
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

        public virtual void AfterCheckConnect()
        {
            if (m_IsHaveWater)
            {
                SetColor();
            }

            m_IsHavingWater = m_IsHaveWater;
            if (m_IsOldHaveWater == m_IsHaveWater) return;
            SetGrowingWater(m_IsHaveWater, m_IsHaveWater ? m_StartWater : StartPoint.CENTER);

            if (m_IsHaveWater)
            {
                EventController.RaiseEvent(EventList.GamePlay.PLAY_SOUND, new EventParam(SoundName.WATER_CONNECT));
            }
        }

        protected void SetColor()
        {
            if (!m_IsHaveWater) return;
            WaterColorIndex = GridHelper.GetColorIndex(m_ListWaterColor);
            for (int i = 0; i < m_WaterCount; i++)
            {
                m_WaterComponent[i].SetColor(CurrentWaterColor, !m_IsOldHaveWater);
            }
        }

        protected void SetGrowingWater(bool isGrowUp, StartPoint waterPoint)
        {
            var waterPos = GetWaterPos(waterPoint);
            for (int i = 0; i < m_WaterCount; i++)
            {
                m_WaterComponent[i].SetGrowing(isGrowUp, waterPos, m_DelayStart);
            }

            if (Game.isBlockGrayColor && !m_IsLock && IsHaveTerrain)
            {
                m_Terrain.SetPos(waterPos);
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

        public void OnTweenSpawn(bool isTween)
        {
            m_Tween?.Kill(true);
            if (isTween)
            {
                if (IsInBigSquare)
                {
                    var tempPos = CurrentLocalPos;
                    tempPos.y = 0f;
                    transform.localPosition = tempPos;
                }
                else
                {
                    var moveTime = Config.startEffectMoveTime;
                    var delayTime = (GridType == GridType.WALL) ? 0f : moveTime + GamePlay.GetIndexSpawn(Row, Col) * .1f;

                    m_IsMoving = true;
                    m_Tween = transform.DOLocalMoveY(0f, moveTime).SetDelay(delayTime).OnComplete(() => {
                        m_IsMoving = false;
                    });
                }
            }
            else
            {
                var tempPos = CurrentLocalPos;
                tempPos.y = 20f;
                transform.localPosition = tempPos;
            }
        }

        private Vector3 GetWaterPos(StartPoint waterPoint)
        {
            Vector3 pos = Vector3.zero;
            switch (waterPoint)
            {
                case StartPoint.TOP:
                    pos = TopPos;
                    break;
                case StartPoint.BOT:
                    pos = BotPos;
                    break;
                case StartPoint.LEFT:
                    pos = LeftPos;
                    break;
                case StartPoint.RIGHT:
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

        protected virtual int GetTrueRotHint()
        {
            return TrueRot;
        }


        [NaughtyAttributes.Button]
        protected void CheckLine()
        {
            var str = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    str += CurrentLine[i, j];
                }
                str += "\n";
            }
            Debug.LogError("\n" + str);

            Debug.LogError("IsHaveLineTop: " + IsHaveLineTop + " | IsHaveLineBot: " + IsHaveLineBot
                + " | IsHaveLineLeft: " + IsHaveLineLeft + " | IsHaveLineRight: " + IsHaveLineRight);
        }
    }

}
