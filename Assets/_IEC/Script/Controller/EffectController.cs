using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class EffectController : _BaseBehaviour, IController, IEventAwake, IEventListener, IEventUpdate
    {
        private float m_TimeCountAfterHint;
        private bool m_IsCounting;

        private List<EffectGridAura> m_ListEffect = new List<EffectGridAura>();

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourAwake()
        {
            PrefabHelper.SetUpPrefab(Config.effectGridAuraObj);
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.GAME_EFFECT_HINT:
                    var listPos = (List<HintEffect>)param.p0;
                    SpawnEffectHint(listPos);
                    break;
            }
        }

        private void OnRemoveAll()
        {
            m_ListEffect.ForEach(x => x.DestroyEffect());
        }

        public void BehaviourUpdate(float deltaTime)
        {
            if (m_IsCounting)
            {

            }
        }

        private void SpawnEffectHint(List<HintEffect> listHintEffect)
        {
            for (int i = 0; i < listHintEffect.Count; i++)
            {
                var effectObj = PrefabHelper.Clone(Config.effectGridAuraObj);
                effectObj.transform.position = listHintEffect[i].pos;

                var effect = effectObj.GetComponent<EffectGridAura>();
                effect.StartEffect(Config.GetWaterColor(listHintEffect[i].color, Game.isMultiColor));
            }
        }
    }

    public struct HintEffect
    {
        public Vector3 pos;
        public int color;

        public HintEffect(Vector3 pos, int color)
        {
            this.pos = pos;
            this.color = color;
        }
    }
}
