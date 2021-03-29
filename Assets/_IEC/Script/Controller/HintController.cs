using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace IEC
{
    public class HintController : _BaseBehaviour, IController, IEventListener
    {
        private List<BaseComponent> m_ListTempHint = new List<BaseComponent>();

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.GAME_START_HINT:
                    Game.isWaitAfterRV = false;
                    StartHint();
                    break;
                case EventList.GamePlay.GAME_AFTER_HINT:
                    Game.isAllowHint = true;
                    EventController.RaiseEvent(EventList.GamePlay.GAME_CHECK_CONNECT);
                    EventController.RaiseEventDelay(.1f, EventList.GamePlay.GAME_EFFECT_HINT, new EventParam(this, param.p0));
                    break;
            }
        }

        public void StartHint()
        {
            if (!Game.isPlaying) return;
            if (!Game.isAllowHint) return;
            m_ListTempHint.Clear();
            Game.isAllowHint = false;
            var addIndex = Game.size - 2;
            if (Game.isFixedHintGrid)
            {
                addIndex = 3;
            }else if (Game.isReduceHintGrid)
            {
                addIndex = Mathf.RoundToInt(addIndex * .7f);
            }

            if (Game.isHintStyleV2 || Game.isHintStyleBSP)
            {
                if (Game.isHintStyleBSP)
                {
                    addIndex = 1;
                }

                var moveCount = 0;
                var listHintsCount = Game.listHints.Count;
                for (int i = 0; i < listHintsCount; i++)
                {
                    var grid = Game.listHints[i];
                    if (!grid.IsTrueRot())
                    {
                        grid.OnHint();
                        m_ListTempHint.Add(grid);
                        moveCount += 1;
                    }

                    if(moveCount >= addIndex)
                    {
                        break;
                    }
                }
            }
            else
            {
                Game.maxHintIndex = Mathf.Clamp(Game.maxHintIndex + addIndex, 0, Game.listHints.Count);
                var startIndexEffect = Game.maxHintIndex - addIndex;
                for (int i = 0; i < Game.maxHintIndex; i++)
                {
                    var grid = Game.listHints[i];
                    if (i >= startIndexEffect)
                    {
                        m_ListTempHint.Add(grid);
                    }
                    if (!grid.IsTrueRot())
                    {
                        grid.OnHint();
                    }
                }
            }

            EventController.RaiseEventDelay(.3f, EventList.GamePlay.GAME_AFTER_HINT, new EventParam(this, m_ListTempHint));
        }

        private List<BaseComponent> m_ListGridSolution = new List<BaseComponent>();
        public void CheckSoulution()
        {
            Game.listGrids.ForEach(x => x.PrepareCheckConnect());

            var sourceCount = Game.listSource.Count;
            var totalGrid = Game.size * Game.size;

            for (int i = 0; i < sourceCount; i++)
            {
                var source = Game.listSource[i];

                m_ListGridSolution.Clear();
                m_ListGridSolution.Add(source);

                
                bool isChanged = true;
                var totalGridHaveWaterCount = m_ListGridSolution.Count;

                var startIndex = 0;

                while (totalGridHaveWaterCount < totalGrid && isChanged)
                {
                    var count = m_ListGridSolution.Count;
                    for (int j = startIndex; j < count; j++)
                    {
                        var grid = m_ListGridSolution[j];
                        var listGrids = grid.GetGridsCanConnect(source.WaterIndexColor, true);
                        startIndex += 1;
                        var listGridNearCount = listGrids.Count;
                        if (listGridNearCount > 0)
                        {
                            for (int x = 0; x < listGridNearCount; x++)
                            {
                                var gridNearConnect = listGrids[x];

                                if (!m_ListGridSolution.Contains(gridNearConnect))
                                {
                                    m_ListGridSolution.Add(gridNearConnect);
                                    gridNearConnect.SetConnect(source.WaterIndexColor, 0);
                                }
                            }
                        }
                    }

                    totalGridHaveWaterCount = m_ListGridSolution.Count;

                    if (totalGridHaveWaterCount == count)
                    {
                        isChanged = false;
                        var gridSolutionCount = m_ListGridSolution.Count;
                        for (int x = 0; x < gridSolutionCount; x++)
                        {
                            var gridNear = m_ListGridSolution[x];
                            if (gridNear.GridType != GridType.WALL)
                            {
                                Game.listHints.Add(gridNear);
                            }
                        }
                    }
                }
            }
        }

    }
}