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

        public int ID => gameObject.GetInstanceID();

        public Vector3 CurrentPos => transform.position;

        public Transform Transform => transform;

        public GridType GridType;

        protected bool IsRotating, IsHaveWater;

        protected bool m_IsOldStatusWater;

        [HideInInspector]
        public int Row, Col, WaterColor;

        protected int m_CurrentRot, m_OldWaterColor, m_TrueRot;

        protected Tween m_Tween;

        protected static int[] TotalRots = new int[] { 0, 90, 180, 270 };

        public bool IsTrueRot => m_CurrentRot == m_TrueRot;

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

        public List<int> m_ListWaterColor = new List<int>();

        public virtual void OnInit(GridType gridType , int row, int col, int trueRot = 0, int rot = 0, int color = 0, int style = 0)
        {
            GridType = gridType;
            Row = row;
            Col = col;
            WaterColor = color;
            m_CurrentRot = rot;
            m_TrueRot = trueRot;

            DefaultLine = GetDefaultLine(style);
            CurrentLine = DefaultLine;

            transform.localRotation = Quaternion.Euler(0f, TotalRots[m_CurrentRot], 0f);

            for (int i = 0; i < m_WaterComponent.Length; i++)
            {
                m_WaterComponent[i].OnInit(Config.GetWaterColor(WaterColor, Game.isMultiColor), GridType == GridType.GRID_SOURCE);
            }

            if (GridType != GridType.WALL)
            {
                m_Terrain.SetupTerrain(GetTerrainType(Row, Col));
            }
        }

        protected virtual void OnMouseDown()
        {

        }

        protected void OnDOShakePosition()
        {
            m_Tween?.Kill(true);
            Vector3 shakeVector = new Vector3(0f, 0.05f, 0f);
            m_Tween = transform.DOShakePosition(.5f, shakeVector, 20, 1f, false, true);
            EventController.RaiseEvent(EventList.GamePlay.GAME_WALL_CLICK, new EventParam(this, CurrentPos));
        }

        public virtual void AfterInit(bool isMultiColor)
        {
            IsRotating = false;
            IsHaveWater = (GridType == GridType.GRID_SOURCE);
            m_IsOldStatusWater = false;
            m_OldWaterColor = 0;
        }

        public virtual void OnUpdate(float deltaTime)
        {
            for (int i = 0; i < m_WaterComponent.Length; i++)
            {
                if (m_WaterComponent[i].gameObject.activeSelf)
                {
                    m_WaterComponent[i].OnUpdate(deltaTime);
                }
            }
        }

        protected virtual int[,] GetDefaultLine(int style)
        {
            return OriginLine;
        }

        private int GetTerrainType(int row, int col)
        {
            if(row % 2 == 0)
            {
                if(col % 2 == 0)
                {
                    return Game.listTerrainType[0];
                }
                else
                {
                    return Game.listTerrainType[1];
                }
            }
            else
            {
                if (col % 2 == 0)
                {
                    return Game.listTerrainType[1];
                }
                else
                {
                    return Game.listTerrainType[0];
                }
            }
        }

        protected void SetActiveWater(bool isActive)
        {
            for (int i = 0; i < m_WaterComponent.Length; i++)
            {
                m_WaterComponent[i].SetFading(isActive);
            }
        }

        public virtual void PrepareCheckConnect()
        {
            if (GridType == GridType.GRID_SOURCE) return;
            m_IsOldStatusWater = IsHaveWater;
            m_OldWaterColor = WaterColor;
            IsHaveWater = false;

            m_ListWaterColor.Clear();
        }

        public void SetConnect(int color)
        {
            IsHaveWater = true;
            if (!m_ListWaterColor.Contains(color))
            {
                m_ListWaterColor.Add(color);
            }
        }

        private void SetColor()
        {
            if (!IsHaveWater) return;
            WaterColor = GetColor();
            for (int i = 0; i < m_WaterComponent.Length; i++)
            {
                m_WaterComponent[i].SetColor(Config.GetWaterColor(WaterColor, Game.isMultiColor), !m_IsOldStatusWater);
            }
        }

        private int GetColor()
        {
            var resultColor = m_ListWaterColor[0];
            if(m_ListWaterColor.Count == 2)
            {
                resultColor = GetMix2Colors();
            }
            else if(m_ListWaterColor.Count == 3)
            {
                if (m_ListWaterColor.Contains(1) && m_ListWaterColor.Contains(2) && m_ListWaterColor.Contains(3))
                {
                    resultColor = 7;
                }
                else 
                {
                    resultColor = GetMix2Colors();
                }
            }

            return resultColor;
        }

        private int GetMix2Colors()
        {
            var resultColor = 0;

            if (m_ListWaterColor.Contains(1) && m_ListWaterColor.Contains(2))
            {
                resultColor = 4;
            }
            else if (m_ListWaterColor.Contains(2) && m_ListWaterColor.Contains(3))
            {
                resultColor = 5;
            }
            else if (m_ListWaterColor.Contains(1) && m_ListWaterColor.Contains(3))
            {
                resultColor = 6;
            }
            return resultColor;
        }

        public virtual void AfterCheckConnect()
        {
            if (GridType == GridType.GRID_SOURCE) return;
            SetColor();
            if (m_IsOldStatusWater == IsHaveWater && m_OldWaterColor == WaterColor) return;
            if (IsHaveWater)
            {
                Game.isHaveWaterConnection = true;
            }
            SetActiveWater(IsHaveWater);
        }

        public bool IsAllowConnect(int color)
        {
            return (GridType != GridType.GRID_SOURCE && !m_ListWaterColor.Contains(color));
        }

        private BaseComponent m_TempGrid;
        private List<BaseComponent> m_ListGridConnect = new List<BaseComponent>();

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
                var m_TempGrid = GetOppositeWall(color);
                if (m_TempGrid != null)
                {
                    m_ListGridConnect.Add(m_TempGrid);
                }
            }

            return m_ListGridConnect;
        }

        private BaseComponent GetOppositeWall(int color)
        {
            var maxIndex = Game.size - 1;
            var row = Row;
            var col = Col;

            if (Row == 0)
            {
                row = maxIndex;
            }

            if (Row == maxIndex)
            {
                row = 0;
            }

            if (Col == 0)
            {
                col = maxIndex;
            }

            if (Col == maxIndex)
            {
                col = 0;
            }

            return Game.GetGrid(row, col, color);

        }

        public virtual void OnHint()
        {

        }

        public virtual void OnDestroyMe()
        {
            m_Tween?.Kill(true);
            IsRotating = false;
            IsHaveWater = false;
            m_IsOldStatusWater = false;
            m_OldWaterColor = 0;
            PrefabHelper.Destroy(gameObject);
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
