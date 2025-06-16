using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    public class EffectCoinGroup : _BaseBehaviour, IGroup
    {
        public GameObject coinPrefab;
        public float spawnRadius = 0.8f;
        public float timeFly = 0.5f;

        private List<Transform> m_ListCoin = new List<Transform>();

        public Transform coinMoveFromT;
        public Transform coinMoveToT;

        public string TWEEN_ID => "EffectCoin" + gameObject.GetInstanceID();

        public System.Action<int> actionAddGold;

        public void LocateGroup()
        {
            Pool.Set(this);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        public void ShowTest()
        {
            ShowEffect(0, 10);
        }

        public void ShowEffect(int addNumber, int coinNumber = 10)
        {
            var posFrom = coinMoveFromT.position;
            var posTo = coinMoveToT.position;

            ShowEffect(addNumber, coinNumber, posFrom, posTo);
        }

        public void ShowEffect(int addNumber, int coinNumber, Vector3 posFrom, Vector3 posTo)
        {
            DOTween.Kill(TWEEN_ID);

            for (int i = m_ListCoin.Count; i < coinNumber; i++)
            {
                var obj = PrefabHelper.Clone(coinPrefab, coinPrefab.transform.parent);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.one;
                m_ListCoin.Add(obj.transform);
            }

            foreach (var coin in m_ListCoin)
            {
                coin.position = posFrom;
                coin.gameObject.SetActive(false);
            }

            var listCoin = m_ListCoin.Take(coinNumber).ToList();

            var s = DOTween.Sequence();
            var seqInit = DOTween.Sequence();
            var seqMove = DOTween.Sequence();

            foreach (var coin in listCoin)
            {
                var randomPos = (Vector3)Random.insideUnitCircle * spawnRadius;
                coin.gameObject.SetActive(true);

                seqInit.Join(
                    coin.DOMove(posFrom + randomPos, timeFly * 0.3f)
                        .SetEase(Ease.InQuart)
                );
            }

            var delay = 0f;
            foreach (var coin in listCoin)
            {
                delay += 0.02f;
                var tweenMove = coin
                    .DOMove(posTo, timeFly)
                    .SetEase(Ease.InQuart)
                    .OnComplete(() =>
                    {
                        coin.gameObject.SetActive(false);
                    });

                seqMove.Insert(delay, tweenMove);
            }

            seqMove.InsertCallback(timeFly + 0.02f, () =>
            {
                actionAddGold?.Invoke(addNumber);
            });

            s.Append(seqInit);
            s.Append(seqMove);

            s.AppendCallback(() =>
            {
                EventController.RaiseEvent(EventList.SAVE_DATA);
            });

            s.SetId(TWEEN_ID);
        }
    }
}