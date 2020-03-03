using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace IEC
{
    public class TestController : MonoBehaviour, IController, IEventListener, IEventAwake, IEventStart, IEventUpdate
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
                case EventList.NONE:
                    Debug.Log("event none");
                    break;
            }
        }

        public void BehaviourAwake()
        {
        }

        public void BehaviourStart()
        {
            var textHello = Pool.Get<UIGroup>().textHello;
            Debug.Log("textHello", textHello.gameObject);
            var rectT = textHello.gameObject.GetComponent<RectTransform>();
            var curPosY = rectT.anchoredPosition.y;
            rectT.DOAnchorPosY(curPosY - 200, 2f).SetLoops(-1, LoopType.Yoyo);
        }

        public void BehaviourUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Reload Scene");
                DOTween.Clear();
                SceneManager.LoadScene(0);
            }
        }
    }
}