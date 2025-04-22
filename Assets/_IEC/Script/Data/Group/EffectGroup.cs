using System;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IEC
{
    public enum EffectType
    {
        Gold
    }
    
    public class EffectGroup : _BaseBehaviour, IGroup
    {
        public Transform parent;
        public List<GameObject> effectPrefabs;
        public float spawnRadius = 0.8f;
        public float timeFly = 0.5f;
        
        private readonly Dictionary<EffectType, List<Transform>> m_ListEffect = new Dictionary<EffectType, List<Transform>>();
        private readonly Dictionary<EffectType, string> m_TweenIDs = new Dictionary<EffectType, string>();

        public void LocateGroup()
        {
            Pool.Set(this);
        }

        private void Start()
        {
            for (var i = 0; i < effectPrefabs.Count; i++)
            {
                var type = (EffectType)i;
                m_ListEffect[type] = new List<Transform>();
                m_TweenIDs[type] = $"Effect{type}" + gameObject.GetInstanceID();
            }
        }

        public void ShowEffect(EffectType type, int amount, Vector3 posFrom, Vector3 posTo, Action onFinish = null)
        {
            ShowEffect(type, amount, posFrom, posTo, currency =>
            {
                switch (type)
                {
                    case EffectType.Gold:
                        PlayerData.AddCurrency(ResourceType.Gold,amount);
                        break;
                }
                
            }, () =>
            {
                OnFinishEffect(type);
                onFinish?.Invoke();
            });
        }
        
        public void ShowEffectOnly(EffectType type, Vector3 posFrom, Vector3 posTo, Action onFinish = null)
        {
            ShowEffect(type, 0, posFrom, posTo, null, () =>
            {
                OnFinishEffect(type);
                onFinish?.Invoke();
            });
        }

        private void OnFinishEffect(EffectType type)
        {
            switch (type)
            {
                case EffectType.Gold:
                    Observer.Notify(EventList.GamePlay.GOLD_CHANGE);
                    break;
            }
        }

        public void ShowEffect(EffectType type, int amount, Vector3 posFrom, Vector3 posTo, Action<int> actionAddResource, Action onFinish, int resourcesDisplay = 10)
        {
            DOTween.Kill(m_TweenIDs[type]);
            
            var effectPrefab = effectPrefabs[(int)type];
            var effectList = m_ListEffect[type];
            
            actionAddResource?.Invoke(amount);
            EventController.RaiseEvent(EventList.SAVE_DATA);
            
            for (int i = effectList.Count; i < resourcesDisplay; i++)
            {
                var obj = PrefabHelper.Clone(effectPrefab, parent);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.one;
                effectList.Add(obj.transform);
            }

            foreach (var effect in effectList)
            {
                effect.position = posFrom;
                effect.gameObject.SetActive(false);
            }

            var listEffect = effectList.Take(resourcesDisplay).ToList();
            
            TriggerEffectSequences(type, listEffect, posFrom, posTo, onFinish);
        }
        
        private void TriggerEffectSequences(EffectType type, List<Transform> effects, Vector3 posFrom, Vector3 posTo, Action onFinish)
        {
            var s = DOTween.Sequence();
            var seqInit = DOTween.Sequence();
            var seqMove = DOTween.Sequence();

            foreach (var effect in effects)
            {
                var randomPos = (Vector3)Random.insideUnitCircle * spawnRadius;
                effect.gameObject.SetActive(true);

                seqInit.Join(
                    effect.DOMove(posFrom + randomPos, timeFly * 0.3f)
                        .SetEase(Ease.InQuart)
                );
            }

            var delay = 0f;
            foreach (var effect in effects)
            {
                delay += 0.02f;
                var tweenMove = effect
                    .DOMove(posTo, timeFly)
                    .SetEase(Ease.InQuart)
                    .OnComplete(() => effect.gameObject.SetActive(false));

                seqMove.Insert(delay, tweenMove);
            }

            seqMove.InsertCallback(timeFly + 0.02f, onFinish.Invoke);

            s.Append(seqInit);
            s.Append(seqMove);

            s.SetId(m_TweenIDs[type]);
        }
    }
}