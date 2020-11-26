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
                    EventController.RaiseEvent(EventList.GamePlay.CHECK_GAME_OVER);
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

            Game.maxHintIndex = Mathf.Clamp(Game.maxHintIndex + addIndex, 0, Game.listHints.Count);
            var startIndexEffect = Game.maxHintIndex - addIndex;
            for (int i = 0; i < Game.maxHintIndex; i++)
            {
                var grid = Game.listHints[i];
                if(i >= startIndexEffect)
                {
                    m_ListTempHint.Add(grid);
                }
                if (!grid.IsTrueRot)
                {
                    grid.OnHint();
                }
            }

            EventController.RaiseEventDelay(.3f, EventList.GamePlay.GAME_AFTER_HINT, new EventParam(this, m_ListTempHint));
            Services.PushEvent(new IEC.TrackingEvent.HintLevel());
        }

        private List<BaseComponent> m_ListTempGridSolution = new List<BaseComponent>();
        public void CheckSoulution()
        {
            Game.listGrids.ForEach(x => x.PrepareCheckConnect());

            var sourceCount = Game.listSource.Count;
            for (int i = 0; i < sourceCount; i++)
            {
                var source = Game.listSource[i];

                m_ListTempGridSolution.Clear();
                m_ListTempGridSolution.Add(source);

                var totalGrid = Game.size * Game.size;
                bool isChanged = true;
                var totalGridHaveWaterCount = m_ListTempGridSolution.Count;

                while (totalGridHaveWaterCount < totalGrid && isChanged)
                {
                    var count = m_ListTempGridSolution.Count;
                    for (int j = 0; j < count; j++)
                    {
                        var grid = m_ListTempGridSolution[j];
                        var listGridNear = grid.GetGridNearCanConnect(source.WaterColor);

                        var listGridNearCount = listGridNear.Count;
                        if (listGridNear.Count > 0)
                        {
                            listGridNear.ForEach(x => x.SetConnect(source.WaterColor));
                            m_ListTempGridSolution.AddRange(listGridNear);
                        }
                    }

                    totalGridHaveWaterCount = m_ListTempGridSolution.Count;

                    if (totalGridHaveWaterCount == count)
                    {
                        isChanged = false;
                        var gridSolutionCount = m_ListTempGridSolution.Count;
                        for (int x = 0; x < gridSolutionCount; x++)
                        {
                            var gridNear = m_ListTempGridSolution[x];
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