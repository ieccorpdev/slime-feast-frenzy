using DG.Tweening;
using IEC.Common.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace IEC
{
    public static class SceneName
    {
        public const string GAME = "Game";
    }

    public class SceneController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        public string CurrentScene { get; private set; }

        Dictionary<string, string> m_DictEventNeedLoadScene = new Dictionary<string, string>()
        {
            { EventList.GamePlay.NEW_GAME, SceneName.GAME },
        };

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
        }

        public void SendEvent(float delayTime, string eventName, EventParam param = new EventParam())
        {
            DOVirtual.DelayedCall(delayTime, () => SendEvent(eventName, param));
        }

        public void SendEvent(string eventName, EventParam param = new EventParam(), Action onLoadSceneDone = null)
        {
            if (m_DictEventNeedLoadScene.TryGetValue(eventName, out string sceneName))
            {
                RunWithScene(sceneName, () =>
                {
                    onLoadSceneDone?.Invoke();
                    EventRx.SendEvent(eventName, param);
                });
                return;
            }
            onLoadSceneDone?.Invoke();
            EventRx.SendEvent(eventName, param);
        }

        public void RunWithScene(string sceneName, System.Action action)
        {
            if (sceneName == CurrentScene)
            {
                action?.Invoke();
                return;
            }
            StartCoroutine(PrepareSceneForAction(sceneName, action));
        }

        IEnumerator PrepareSceneForAction(string sceneName, System.Action action)
        {
            // if (!string.IsNullOrEmpty(CurrentScene))
            //     yield return SceneManager.UnloadSceneAsync(CurrentScene);
            // yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            // SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            // CurrentScene = sceneName;
            var activeScene = SceneManager.GetActiveScene();
            RemoveDuplicateEventSystem(activeScene);
            LocateGroupFromScene(activeScene);
            //UI.LocateAdditiveGroup();
            // SendEvent(EventList.GamePlay.SETUP_NEW_SCENE);
            EventController.RaiseEvent(EventList.GamePlay.SETUP_NEW_SCENE);
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }

        void LocateGroupFromScene(Scene scene)
        {
            var rootObjs = scene.GetRootGameObjects();
            foreach (var obj in rootObjs)
            {
                var groups = obj.GetComponentsInChildren<IGroup>();
                foreach (var group in groups)
                {
                    group.LocateGroup();
                }
            }
        }

        void RemoveDuplicateEventSystem(Scene scene)
        {
            var rootObjs = scene.GetRootGameObjects();
            foreach (var obj in rootObjs)
            {
                var childs = obj.GetComponentsInChildren<EventSystem>();
                foreach (var c in childs)
                {
                    Destroy(c.gameObject);
                }
            }
        }
    }
}