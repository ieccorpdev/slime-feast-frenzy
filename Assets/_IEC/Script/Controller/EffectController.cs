using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class EffectController : _BaseBehaviour, IController, IEventListener, IEventUpdate
    {
        public float m_TimeCountAfterHint;
        public bool m_IsCounting;

        private List<BaseComponent> m_ListHintEffect = new List<BaseComponent>();

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
                case EventList.GamePlay.GAME_EFFECT_HINT:
                    OnDisableAll();
                    var listPos = (List<BaseComponent>)param.p0;
                    SpawnEffectHint(listPos);
                    break;
                case EventList.GamePlay.GAME_GRID_CLICK:
                    OnDisableAll();
                    break;
            }
        }

        private void OnDisableAll()
        {
            m_IsCounting = false;
            m_ListHintEffect.ForEach(x => x.OnEnableHintEffect(false));
            m_ListHintEffect.Clear();
        }

        public void BehaviourUpdate(float deltaTime)
        {
            if (m_IsCounting)
            {
                m_TimeCountAfterHint += deltaTime;
                if(m_TimeCountAfterHint > 5f)
                {
                    OnDisableAll();
                }
            }
        }

        private void SpawnEffectHint(List<BaseComponent> listHintEffect)
        {
            listHintEffect.ForEach(x => x.OnEnableHintEffect(true));

            m_ListHintEffect.Clear();
            m_ListHintEffect.AddRange(listHintEffect);

            m_TimeCountAfterHint = 0f;
            m_IsCounting = true;
        }
    }

}
