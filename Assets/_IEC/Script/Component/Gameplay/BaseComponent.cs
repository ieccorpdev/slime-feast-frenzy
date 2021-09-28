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

        public int Row, Col, TrueRot, TrueColor, WaterIndexColor;

        public GridType GridType;

        // Dynamic data
        protected Color CurrentWaterColor
        {
            get
            {
                if (WaterIndexColor > 0)
                {
                    return Config.GetWaterColor(WaterIndexColor);
                }
                else
                {
                    return Config.GetWaterClassicColor(Game.isRandomColorClassicMode, Game.classicColor);
                }
            }
        }

        public virtual bool IsTrueRot() => m_CurrentRot == TrueRot;
        public bool IsAllowRotation => !m_IsRotating;

        protected bool m_IsInit, m_IsRotating, m_IsHaveWater, m_IsLock, 
            m_IsOldHaveWater, m_IsHavingWater, m_IsMoving;

        protected int m_CurrentRot, m_OldWaterIndexColor, m_WaterCount;

        private int m_DelayStart;

        protected WaterPoint m_StartWater;
        public WaterPoint Inline;

        public int m_InlineOrigin, m_InlineOffset;
        public bool IsOneWayBlock, IsInBigSquare;


        //Temp
        protected Tween m_Tween;
        protected List<int> m_ListWaterColor = new List<int>();
        private BaseComponent m_TempGrid;
        private List<BaseComponent> m_ListGridConnect = new List<BaseComponent>();

        private Vector3 m_StartPos1, m_EndPos1, m_StartPos2, m_EndPos2;


        public virtual void OnInit(GridType gridType , int row, int col, int trueRot = 0, int rot = 0, int color = 0, int style = 0)
        {
            if (!m_IsInit)
            { 
                m_IsInit = true;
                m_WaterCount = m_WaterComponent.Length;

                if (m_WaterCount > 0)
                {
                    for (int i = 0; i < m_WaterCount; i++)
                    {
                        m_WaterComponent[i].OnInit(OnWaterGrowUpDone, OnGrowUping);
                    }
                }
            }

            OnResetAll();

            GridType = gridType;
            Row = row;
            Col = col;
            WaterIndexColor = color;
            TrueColor = color;
            m_CurrentRot = rot;
            TrueRot = trueRot;

            DefaultLine = GetDefaultLine(style);
            CurrentLine = DefaultLine;
            OnUpdateSetTerrain();
        }

        private void OnResetAll()
        {
            m_IsLock = false;
            IsOneWayBlock = false;
            IsInBigSquare = false;

            if (m_Collider)
            {
                m_Collider.enabled = true;
            }

            if (m_Terrain)
            {
                m_Terrain.ResetAll();
            }

            OnResetDataPlaying();
        }

        public void SetNewPos(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public virtual void SetLock(bool isChangeTerrain)
        {
            m_IsLock = true;
            m_Collider.enabled = false;
            if (isChangeTerrain)
            {
                if (m_Terrain != null)
                {
                    m_Terrain.SetActiveLock(true);
                }
            }
            else
            {
                m_Terrain.gameObject.SetActive(false);

                if (Game.isHaveBigSquare)
                {
                    IsInBigSquare = true;
                }
            }
        }

        protected void SetLockHint()
        {
            m_IsLock = true;
            if (m_Terrain != null)
            {
                m_Terrain.SetActiveLock(true);
            }
        }

        public virtual void OnUpdateSetTerrain()
        {
            if (GridType != GridType.WALL)
            {
                if (Game.isChangeGridStatus || Game.isGreenifyHydrated)
                {
                    SetTerrainMat();
                }
                else
                {
                    var listMat = Game.isFlyingIsland ? (PlayerData.themeSelected == 0 ? 
                        ShopConfig.listTerrainIsLand1 : ShopConfig.listTerrainIsLand2) 
                        : Game.listTerrainType;
                    m_Terrain.SetupTerrain(GridHelper.GetTerrainType(Row, Col, listMat), 
                        Game.isBlockGrayColor, Game.isFlyingIsland);
                }


                var waterLineMats = (PlayerData.themeSelected == 4) ? ShopConfig.waterLineMat2 : ShopConfig.waterLineMat1;
                for (int i = 0; i < 5; i++)
                {
                    if(m_LineStyleObjs[i] != null)
                    {
                        m_LineStyleObjs[i].GetComponent<MeshRenderer>().material = waterLineMats[i];
                    }
                }
            }
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

        protected virtual void OnGrowUping(float fillAmount)
        {
            if (Game.isBlockGrayColor)
            {
                if (!m_IsLock)
                {
                    if (m_Terrain != null)
                    {
                        m_Terrain.OnUpdate(fillAmount);
                    }
                }
            }
        }

        public virtual void OnUpdate(float deltaTime)
        {
            if (m_WaterCount < 1) return;
            for (int i = 0; i < m_WaterCount; i++)
            {
                if (m_WaterComponent[i].gameObject.activeInHierarchy)
                {
                    m_WaterComponent[i].OnUpdate(deltaTime);

                    m_WaterComponent[i].UpdatePos(GetWaterPos(m_StartWater));
                }
            }
        }

        public virtual void OnDestroyMe()
        {
            m_Tween?.Kill(true);
            if (m_HintEffect != null)
            {
                m_HintEffect.OnDestroyMe();
            }

            if (m_Terrain != null)
            {
                m_Terrain.ResetAll();
            }

            OnResetDataPlaying();

            PrefabHelper.Destroy(gameObject);
        }

        private void OnResetDataPlaying()
        {
            m_IsRotating = false;
            m_IsHaveWater = false;
            m_IsOldHaveWater = false;
            m_OldWaterIndexColor = 0;
            m_DelayStart = 0;
            m_IsHavingWater = false;
            m_ListWaterColor.Clear();
            m_StartWater = WaterPoint.CENTER;

        }

        public virtual void BeforeInit()
        {

        }

        public virtual void AfterInit()
        {
            if (GridType != GridType.GRID_SOURCE)
            {
                OnResetDataPlaying();
            }
        }

        protected virtual int[,] GetDefaultLine(int style)
        {
            return OriginLine;
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

        public List<BaseComponent> GetGridsCanConnect(int color, bool isInit)
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
                        if (isInit)
                        {
                            m_TempGrid.SetupArrow(WaterPoint.BOT);
                            m_ListGridConnect.Add(m_TempGrid);
                        }
                        else
                        {
                            if(Game.isHaveOneWayBlock && m_TempGrid.IsOneWayBlock)
                            {
                                if(m_TempGrid.Inline == WaterPoint.BOT)
                                {
                                    m_ListGridConnect.Add(m_TempGrid);
                                    m_TempGrid.SetStartWater(WaterPoint.BOT);
                                }
                            }
                            else
                            {
                                m_ListGridConnect.Add(m_TempGrid);
                                m_TempGrid.SetStartWater(WaterPoint.BOT);
                            }
                        }
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
                        if (isInit)
                        {
                            m_TempGrid.SetupArrow(WaterPoint.TOP);
                            m_ListGridConnect.Add(m_TempGrid);
                        }
                        else
                        {
                            if (Game.isHaveOneWayBlock && m_TempGrid.IsOneWayBlock)
                            {
                                if (m_TempGrid.Inline == WaterPoint.TOP)
                                {
                                    m_ListGridConnect.Add(m_TempGrid);
                                    m_TempGrid.SetStartWater(WaterPoint.TOP);
                                }
                            }
                            else
                            {
                                m_ListGridConnect.Add(m_TempGrid);
                                m_TempGrid.SetStartWater(WaterPoint.TOP);
                            }
                        }
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
                        if (isInit)
                        {
                            m_TempGrid.SetupArrow(WaterPoint.RIGHT);
                            m_ListGridConnect.Add(m_TempGrid);
                        }
                        else
                        {
                            if (Game.isHaveOneWayBlock && m_TempGrid.IsOneWayBlock)
                            {
                                if (m_TempGrid.Inline == WaterPoint.RIGHT)
                                {
                                    m_ListGridConnect.Add(m_TempGrid);
                                    m_TempGrid.SetStartWater(WaterPoint.RIGHT);
                                }
                            }
                            else
                            {
                                m_ListGridConnect.Add(m_TempGrid);
                                m_TempGrid.SetStartWater(WaterPoint.RIGHT);
                            }
                        }
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
                        if (isInit)
                        {
                            m_TempGrid.SetupArrow(WaterPoint.LEFT);
                            m_ListGridConnect.Add(m_TempGrid);
                        }
                        else
                        {
                            if (Game.isHaveOneWayBlock && m_TempGrid.IsOneWayBlock)
                            {
                                if (m_TempGrid.Inline == WaterPoint.LEFT)
                                {
                                    m_ListGridConnect.Add(m_TempGrid);
                                    m_TempGrid.SetStartWater(WaterPoint.LEFT);
                                }
                            }
                            else
                            {
                                m_ListGridConnect.Add(m_TempGrid);
                                m_TempGrid.SetStartWater(WaterPoint.LEFT);
                            }
                        }
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
            m_IsHavingWater = true;
        }

        public int DelayStart => m_IsHavingWater ? 0 : m_DelayStart + 1;

        public void SetStartWater(WaterPoint point)
        {
            if (!Game.isPlaying) return;
            m_StartWater = point;
        }

        public void OnSetActiveArrow()
        {
            IsOneWayBlock = true;
            m_Terrain.SetActiveArrow();
        }

        public void SetupArrow(WaterPoint point)
        {
            if (GridType == GridType.GRID_STRAIGHT || GridType == GridType.GRID_CURVE)
            {
                OnSetUpArrow(point);
            }
        }

        private void OnSetUpArrow(WaterPoint point)
        {
            var ratioDist = Game.isHaveIntroOutro ? .1f : 2f;
            if (!Game.isHaveOneWayBlock) return;
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

        private Vector3 GetDestination(WaterPoint point)
        {
            if (IsHaveLineTop)
            {
                if(point != WaterPoint.TOP)
                {
                    return TopPos;
                }
            }

            if (IsHaveLineBot)
            {
                if (point != WaterPoint.BOT)
                {
                    return BotPos;
                }
            }

            if (IsHaveLineLeft)
            {
                if (point != WaterPoint.LEFT)
                {
                    return LeftPos;
                }
            }

            if (IsHaveLineRight)
            {
                if (point != WaterPoint.RIGHT)
                {
                    return RightPos;
                }
            }

            return CenterPos;
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
            if ((Game.isChangeGridStatus || Game.isGreenifyHydrated) && Game.isPlaying)
            {
                SetTerrainMat();
            }

            if (!m_IsHaveWater)
            {
                m_IsHavingWater = false;
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
            var waterPos = GetWaterPos(waterPoint);
            for (int i = 0; i < m_WaterCount; i++)
            {
                m_WaterComponent[i].SetGrowing(isGrowUp, waterPos, m_DelayStart);
            }

            if (Game.isBlockGrayColor)
            {
                if (!m_IsLock)
                {
                    if (m_Terrain != null)
                    {
                        m_Terrain.SetPos(waterPos);
                    }
                }
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

        public void OnTweenMove(bool isTween)
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
                    var delayTime = (GridType == GridType.WALL) ? 0f : moveTime + GetIndexSpawn() * .1f;

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

        private int GetIndexSpawn()
        {
            if (Game.introType == 0)
            {
                if (Game.isCamRotated)
                {
                    return Row - 1;
                }
                else
                {
                    return Col - 1;
                }
            }
            else
            {
                if (Game.isCamRotated)
                {
                    return (Game.size - Row - 2);
                }
                else
                {
                    return (Game.size - Col - 2);
                }
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

        public virtual void SetTerrainMat()
        {
            if (m_Terrain == null) return;
            var mat = m_IsHaveWater ? Game.listTerrainType[0] : Game.listTerrainType[1];
            m_Terrain.SetupTerrain(mat, Game.isBlockGrayColor, Game.isFlyingIsland);
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
