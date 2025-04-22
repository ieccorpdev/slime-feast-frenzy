using System;
using System.Collections.Generic;
using UnityEngine;

namespace IEC.Common.Event
{
    public static class EventRx
    {
        public static void OnEvent(string eventName, GameObject listener, Action callback)
        {
            EventDispatcher.AddListener(eventName, callback, listener);
        }

        public static void OnEvent(string eventName, GameObject listener, Action<string> callback)
        {
            EventDispatcher.AddListener<string>(eventName, callback, listener);
        }

        public static void SendEvent(string eventName)
        {
            EventDispatcher.Send(eventName);
        }

        public static void SendEvent<T>(string eventName, T data)
        {
            EventDispatcher.Send(eventName, data);
        }
    }

    public static class EventDispatcher
    {
        private static Dictionary<string, List<ListenerWrapper>> listeners = new();

        public static void AddListener(string eventName, Action callback, GameObject owner)
        {
            AddListener<object>(eventName, _ => callback?.Invoke(), owner);
        }

        public static void AddListener<T>(string eventName, Action<T> callback, GameObject owner)
        {
            if (!listeners.ContainsKey(eventName))
                listeners[eventName] = new List<ListenerWrapper>();

            listeners[eventName].Add(new ListenerWrapper
            {
                Owner = owner,
                Action = obj => callback?.Invoke((T)obj)
            });
        }

        public static void Send(string eventName)
        {
            Send<object>(eventName, null);
        }

        public static void Send<T>(string eventName, T data)
        {
            if (!listeners.TryGetValue(eventName, out var list)) return;

            list.RemoveAll(l => l.Owner == null); // Clean up destroyed listeners
            foreach (var listener in list)
            {
                listener.Action?.Invoke(data);
            }
        }
        

        private class ListenerWrapper
        {
            public GameObject Owner;
            public Action<object> Action;
        }
    }

    public static class EventRxExtension
    {
        public static void OnEvent(this GameObject listener, string eventName, Action callback)
        {
            EventRx.OnEvent(eventName, listener, callback);
        }

        public static void OnEvent<T>(this GameObject listener, string eventName, Action<T> callback)
        {
            EventDispatcher.AddListener<T>(eventName, callback, listener);
        }

        // Placeholder extensions for compatibility (no UniRx features like Take, Merge etc.)
        public static void MergeWarning() =>
            Debug.LogWarning("Observable-like methods like Merge/Take/etc are not available without UniRx.");
    }
}
