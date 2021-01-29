using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using TMPro;

namespace IEC
{
    public class EffectCoinClaim : MonoBehaviour
    {
        public RectTransform[] effectCoins;
        public GameObject EffectCoins;

        public float velocityCoinMove = 10f;
        public float allTimeSpawn = 0.3f;

        public void SpawnEffect(TMP_Text txtMainMoney, Vector2 posEnd, int moneyAdd, int endMoney, Action callback = null)
        {
            if(moneyAdd <= 0)
            {
                callback?.Invoke();
                return;
            }

            for (int i = 0; i < effectCoins.Length; i++)
            {
                effectCoins[i].gameObject.SetActive(false);
            }

            var posSpawn = new Vector2(0f, 0f);
            float distance = Vector2.Distance(posSpawn, posEnd);

            int coinCount = effectCoins.Length;
            coinCount = Mathf.Min(coinCount, moneyAdd);
            var moneyStart = endMoney - moneyAdd;
            var rubyAddPerFly = moneyAdd / coinCount;
            float ratioTime = allTimeSpawn / coinCount;

            Sequence sequence = DOTween.Sequence();
            EffectCoins.gameObject.SetActive(true);

            var timeToPostart = 0.2f;
            var posYEnd = posSpawn.y + 1.5f;
            txtMainMoney.text = StringHelper.GetNumber(moneyStart);

            for (int i = 0; i < coinCount; i++)
            {
                var coinObj = effectCoins[i];
                coinObj.gameObject.SetActive(true);
                coinObj.transform.localScale = Vector3.one;
                coinObj.anchoredPosition = posSpawn;
                var posStart = PosRandomSpawnRuby(posSpawn);
                var index = i;
                float delayTime = i * ratioTime;
                float delayTimeToStart = i * 0.05f;
                sequence
                    .Insert(0f, coinObj.DOAnchorPos(posStart, timeToPostart + delayTime * 2).SetEase(Ease.OutQuad))
                    .Insert(timeToPostart + delayTime * 2, coinObj.DOMove(posEnd, 0.1f + delayTimeToStart * 0.5f).SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            moneyStart += rubyAddPerFly;
                            txtMainMoney.text = StringHelper.GetNumber(moneyStart);
                            coinObj.gameObject.SetActive(false);
                        }));
            }

            sequence.OnComplete(() => {
                txtMainMoney.text = StringHelper.GetNumber(endMoney);
                EffectCoins.SetActive(false); callback?.Invoke();
            });
            sequence.Play();
        }

        private Vector3 PosRandomSpawnRuby(Vector3 posSpawn)
        {
            posSpawn.x += UnityEngine.Random.Range(-400f, 400f);
            posSpawn.y += UnityEngine.Random.Range(-300f, 300f);
            return posSpawn;
        }
    }
}