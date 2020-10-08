using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GameController : _BaseBehaviour, IController, IEventStart, IEventListener
    {
        [SerializeField]
        private ParticleSystem m_PsWin;

        [SerializeField]
        private List<GridComponent> m_Grids;

        public void LocateController()
        {
            Controllers.Add(this);
        }
        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourStart()
        {
            OnStartLevel();
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.START_LEVEL:
                    OnStartLevel();
                    break;

                case EventList.GamePlay.CHECK_GAME_OVER:
                    CheckGameOver();
                    break;
            }
        }

        private void OnStartLevel()
        {
            m_Grids.ForEach(x => x.OnInit());
            Game.isPlaying = true;
        }

        private void CheckGameOver()
        {
            if (IsWin())
            {
                Game.isPlaying = false;
                m_PsWin.Play();
                EventController.RaiseEventDelay(3f, EventList.GamePlay.START_LEVEL);
            }
        }

        private bool IsWin()
        {
            for (int i = 0; i < m_Grids.Count; i++)
            {
                if (!m_Grids[i].IsTrueRot()) return false;
            }
            return true;
        }
    }
}
