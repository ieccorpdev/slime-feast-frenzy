using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using BestHTTP.Decompression.Zlib;
using TMPro;
using UnityEngine.Serialization;

namespace IEC
{
    public class EffectRewardClaim : _BaseBehaviour
    {
        public int maxSpawn = 30;
        public RectTransform baseObjectToSpawn;

        private Dictionary<ResourceType, List<RectTransform>> m_ListEffectByType = new();
        public List<ParticleSystem> listStartExplosion;
        private float allTimeSpawn = 2f;
        public RuntimeAnimatorController coinAnimator;//may be switch to prefab after this...
        public bool AnimCompleted { get; private set; }

        public void SpawnEffect(RewardClaimData effectData)
        {
            SpawnEffect(effectData.rewardType, effectData.rewardSprite, effectData.posSpawn, effectData.posEnd, effectData.rewardAmount,effectData.startSize ,effectData.onComplete, effectData.onEndMove, effectData.delay, effectData.animTime);
        }

        public void SpawnEffect(ResourceType rewardType,Sprite rewardSprite, Vector2 posSpawn, Vector2 posEnd, int addAmount,Vector2 startSize, Action onComplete = null,Action onEndMove = null, float delay = 0f, float animTime = 1f)
        {
            if (addAmount <= 0)
            {
                onComplete?.Invoke();
                return; 
            }

            AnimCompleted = false;

            if (!m_ListEffectByType.ContainsKey(rewardType))
            {
                m_ListEffectByType.Add(rewardType, new());
            }

            List<RectTransform> effectCurrency = m_ListEffectByType[rewardType];
            

            for (int i = 0; i < effectCurrency.Count; i++)
            {
                effectCurrency[i].gameObject.SetActive(false);
            }

            // int currencyCount = effectCurrency.Count;
            // currencyCount = Mathf.Max(currencyCount, addAmount);
            int currencyCount = Mathf.Min(addAmount, maxSpawn);
            float ratioTime = animTime / currencyCount;

            Sequence sequence = DOTween.Sequence();
            //EffectCurrency.gameObject.SetActive(true);
            sequence.SetId(rewardType);
            for (int i = 0; i < currencyCount; i++)
            {
                RectTransform currencyObj;

                if (i >= effectCurrency.Count)
                {
                    effectCurrency.Add(ObjectPool.Spawn(baseObjectToSpawn.gameObject, transform).GetComponent<RectTransform>());
                }
                var posStart = (Vector3)UnityEngine.Random.insideUnitCircle * 0.8f *200 ;
                currencyObj = effectCurrency[i];
                currencyObj.transform.localScale = Vector3.one;
                currencyObj.anchoredPosition = posSpawn + (rewardType == ResourceType.Gold? posStart : Vector2.zero );
                var img = currencyObj.GetComponent<Image>();
                //Animation
                if (rewardType == ResourceType.Gold)
                {
                    var animator = currencyObj.GetComponent<Animator>();
                    if (animator == null)
                    {
                        animator = currencyObj.gameObject.AddComponent<Animator>();
                    }

                    animator.enabled = true;
                    animator.runtimeAnimatorController = coinAnimator;
                }
                else
                {
                    var animator = currencyObj.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.enabled = false;
                    }
                }
                //
                img.sprite = rewardSprite;
                
                img.rectTransform.sizeDelta = startSize;
                currencyObj.gameObject.SetActive(true);

                var index = i;
                float delayTime = i * ratioTime /2f;
                float delayTimeToStart = i * ratioTime;
                if (rewardType == ResourceType.Gold)
                {
                    EventController.RaiseEvent(EventList.Audio.COIN_APPEAR);
                }
                else
                {
                    EventController.RaiseEvent(EventList.Audio.ITEM_APPEAR);
                }

                float timeEffect = 0.5f + delayTimeToStart * ratioTime;
                sequence
                    //.Insert(0f, currencyObj.DOAnchorPos(posStart, timeToPostart + delayTime).SetEase(Ease.OutQuad))
                    .Insert(0f,img.rectTransform.DOSizeDelta(img.rectTransform.sizeDelta*0.5f,timeEffect).SetEase(Ease.OutQuad))
                    .Insert(delayTime, currencyObj.DOAnchorPos(posEnd, timeEffect).SetEase(Ease.InBack)
                        .OnComplete(() =>
                        {
                            onEndMove?.Invoke();
                            currencyObj.gameObject.SetActive(false);
                            if (rewardType == ResourceType.Gold)
                            {
                                EventController.RaiseEvent(EventList.Audio.COIN_GET);
                            }
                            else
                            {
                                EventController.RaiseEvent(EventList.Audio.ITEM_GET);
                            }

                            ActiveVFXStartExplosion(posEnd);
                        }));
            }

            sequence.OnComplete(() =>
            {
                //EffectCurrency.SetActive(false); 
                onComplete?.Invoke();
                AnimCompleted = true;
            });
            sequence.Play();
                            
            //EventController.RaiseEvent(EventList.GamePlay.ON_CLAIM_GOLD);
        }

        public void ActiveVFXStartExplosion(Vector2 anchorPos)
        {
            var vfx = listStartExplosion.Find(x => !x.isPlaying);
            if (vfx != null)
            {
                vfx.GetComponent<RectTransform>().anchoredPosition = anchorPos;
                vfx.Play();
            }
        }
        private Vector3 PosRandomSpawnRuby(Vector3 posSpawn)
        {
            posSpawn.x += UnityEngine.Random.Range(-300f, 300f);
            posSpawn.y += UnityEngine.Random.Range(-150f, 150f);
            return posSpawn;
        }
        
        // public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
        // {
        //     // Vector2 localPoint;
        //     // Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin);
        //     // Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
        //     // screenP += fromPivotDerivedOffset;
        //     // RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
        //     // Vector2 pivotDerivedOffset = new Vector2(to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin);
        //     // return to.anchoredPosition + localPoint - pivotDerivedOffset;
        //     
        //     // Convert world position of 'from' to screen space
        //     Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(GamePlay.uiCamera, from.position);
        //
        //     // Convert screen point to local point in 'to' RectTransform
        //     RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenPoint, GamePlay.uiCamera, out Vector2 localPoint);
        //
        //     return localPoint;
        // }
        
        public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
        {
            Vector2 localPoint;
            Canvas fromCanvas = from.GetComponentInParent<Canvas>();
            Canvas toCanvas = to.GetComponentInParent<Canvas>();
        
            Camera fromCamera = fromCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : fromCanvas.worldCamera;
            Camera toCamera = toCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : toCanvas.worldCamera;
        
            // Convert world position to screen position
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(fromCamera, from.position);
        
            // Convert screen position to local position in the target rect
            RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, toCamera, out localPoint);
        
            Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin);
            Vector2 pivotDerivedOffset = new Vector2(to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin);
        
            return to.anchoredPosition + localPoint - pivotDerivedOffset;
        }
    }

    public class EffectRewardData
    {
        public ResourceType rewardType;
        public int rewardAmount;
        public Vector3 startPos;
        public Vector2 startSize;
        public EffectRewardData()   
        {
        }
        public EffectRewardData(ResourceType rewardType, int addAmount, Vector3 startPos,Vector2 startSize)
        {
            this.rewardType = rewardType;
            this.rewardAmount = addAmount;
            this.startPos = startPos;
            this.startSize = startSize;
        }
    }

    public class RewardClaimData
    {
        public ResourceType rewardType;
        public Sprite rewardSprite;
        public int rewardAmount;
        public Vector2 posSpawn;
        public Vector2 posEnd;
        public Action onComplete;
        public Action onEndMove;
        public float delay;
        public float animTime;
        //Custom
        public Vector2 startSize;
        public RewardClaimData(ResourceType rewardType,Sprite rewardSprite, Vector2 posSpawn, Vector2 posEnd, int addAmount, Action onComplete = null,Action onEndMove = null, float delay = 0f, float animTime = 2f)
        {
            this.rewardType = rewardType;
            this.rewardSprite = rewardSprite;
            this.rewardAmount = addAmount;
            this.posSpawn = posSpawn;
            this.posEnd = posEnd;
            this.onComplete = onComplete;
            this.onEndMove = onEndMove;
            this.delay = delay;
            this.animTime = animTime;
        }

        public void SetStartSize(Vector2 startSize)
        {
            this.startSize = startSize;
        }
    }
    
}