using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class EffectController : _BaseBehaviour, IController, IEventListener, IEventUpdate, IEventAwake
    {
        private float m_TimeCountAfterHint;
        private bool m_IsCounting;
        private List<BaseComponent> m_ListHintEffect = new List<BaseComponent>();

        [SerializeField]
        private List<AnimalComponent> m_ListAnimal;

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
            PrefabHelper.SetUpPrefab(Config.effectLeavesObj);

            m_ListAnimal.ForEach(x => x.OnInit());
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

                case EventList.GamePlay.GAME_LEAVES_EFFECT:
                    var pos = (Vector3)param.p0;
                    pos.y = .35f;
                    StartCoroutine(IEEffectLeaves(pos));
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
            m_ListAnimal.ForEach(x => x.OnUpdate(deltaTime));

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

        private IEnumerator IEEffectLeaves(Vector3 posSpawn)
        {
            var psObj = PrefabHelper.Clone(Config.effectLeavesObj);
            psObj.transform.SetParent(GamePlay.effectParent);
            psObj.transform.position = posSpawn;
            var ps = psObj.GetComponent<ParticleSystem>();
            var psMain = ps.main;
            psMain.startColor = new ParticleSystem.MinMaxGradient(Game.themeConfig.leaveEffectColorMin, Game.themeConfig.leaveEffectColorMax);
            ps.Play();
            yield return new WaitForSeconds(1f);

            PrefabHelper.Destroy(psObj);
        }
    }
}
