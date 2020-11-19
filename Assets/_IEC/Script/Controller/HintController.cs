using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace IEC
{
    public class HintController : _BaseBehaviour, IController, IEventListener
    {
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
                    StartHint();
                    break;
                case EventList.GamePlay.GAME_AFTER_HINT:
                    Game.isAllowHint = true;
                    EventController.RaiseEvent(EventList.GamePlay.CHECK_GAME_OVER);
                    break;
            }
        }

        public void StartHint()
        {
            if (!Game.isPlaying) return;
            if (!Game.isAllowHint) return;
            Game.isAllowHint = false;
            var addIndex = Game.size - 2;
            if (Game.isReduceHintGrid)
            {
                addIndex = Mathf.RoundToInt(addIndex * .7f);
            }

            Game.maxHintIndex = Mathf.Clamp(Game.maxHintIndex + addIndex, 0, Game.listHints.Count);
            for (int i = 0; i < Game.maxHintIndex; i++)
            {
                var grid = Game.listHints[i];
                if (!grid.IsTrueRot)
                {
                    grid.OnHint();
                }
            }

            EventController.RaiseEventDelay(.3f, EventList.GamePlay.GAME_AFTER_HINT);
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