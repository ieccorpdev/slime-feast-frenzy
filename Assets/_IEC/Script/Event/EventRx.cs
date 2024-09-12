using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace IEC.Common.Event
{
    public static class EventRx
    {
        public static void OnEvent(string eventName, GameObject listener, Action callback)
        {
            EventRx<string>.GetEvent(eventName)
                .Subscribe(s =>
                {
                    callback?.Invoke();
                })
                .AddTo(listener);
        }

        public static void OnEvent(string eventName, GameObject listener, Action<string> callback)
        {
            EventRx<string>.GetEvent(eventName)
                .Subscribe(s =>
                {
                    callback?.Invoke(s);
                })
                .AddTo(listener);
        }

        public static void SendEvent(string eventName)
        {
            EventRx<string>.SendEvent(eventName, "");
        }

        public static void SendEvent<T>(string eventName, T eventData)
        {
            // Debug.LogError("SendEvent:" + eventName);
            EventRx<T>.SendEvent(eventName, eventData);
        }
    }

    public static class EventRx<T>
    {
        static Dictionary<string, Subject<T>> s_DictEvents = new Dictionary<string, Subject<T>>();

        public static IObservable<T> GetEvent(string eventName)
        {
            if (!s_DictEvents.ContainsKey(eventName))
            {
                s_DictEvents.Add(eventName, new Subject<T>());
            }
            // Debug.LogError(eventName);
            return s_DictEvents[eventName];
        }

        public static void SendEvent(string eventName, T value)
        {
            if (s_DictEvents.TryGetValue(eventName, out var eventRx))
            {
                eventRx.OnNext(value);
            }
        }
    }

    public static class EventRxExtension
    {
        public static ObservableYieldInstruction<string> WaitForEvent(this GameObject listener, string eventName)
        {
            var yielInstruction = EventRx<string>.GetEvent(eventName)
                .Take(1)
                .ToYieldInstruction();
            yielInstruction.AddTo(listener);
            return yielInstruction;
        }

        public static ObservableYieldInstruction<T> WaitForEvent<T>(this GameObject listener, string eventName)
        {
            var yielInstruction = EventRx<T>.GetEvent(eventName)
                .Take(1)
                .ToYieldInstruction();
            yielInstruction.AddTo(listener);
            return yielInstruction;
        }

        public static void OnEvent(this GameObject listener, string eventName, Action callback)
        {
            EventRx<EventParam>.GetEvent(eventName)
                .Subscribe(param =>
                {
                    callback?.Invoke();
                })
                .AddTo(listener);
        }

        public static void OnEvent<T>(this GameObject listener, string eventName, Action<T> callback)
        {
            EventRx<T>.GetEvent(eventName)
                .Subscribe(input =>
                {
                    callback?.Invoke(input);
                })
                .AddTo(listener);
        }

        public static IObservable<T> Concat<T>(this IObservable<T> first, params IObservable<T>[] seconds)
        {
            return Observable.Concat(first, seconds);
        }
        
        public static IObservable<T> Merge<T>(this IObservable<T> first, params IObservable<T>[] seconds)
        {
            return Observable.Merge(first, seconds);
        }
        
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> observerAction)
        {
            return ObservableExtensions.Subscribe(source, observerAction);
        }

        public static IDisposable AddTo(this IDisposable source, GameObject gameObject)
        {
            return DisposableExtensions.AddTo(source, gameObject);
        }

        public static IObservable<T> Take<T>(this IObservable<T> source, int count)
        {
            return Observable.Take(source, count);
        }

        public static IObservable<T> TakeUntilDestroy<T>(this IObservable<T> source, GameObject gameObject)
        {
            return Observable.TakeUntilDestroy(source, gameObject);
        }

        public static IObservable<T> TakeUntilDisable<T>(this IObservable<T> source, GameObject gameObject)
        {
            return Observable.TakeUntilDisable(source, gameObject);
        }

        public static IObservable<T> TakeWhile<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            return Observable.TakeWhile(source, predicate);
        }

        public static IObservable<T> TakeUntil<T, TOther>(this IObservable<T> source, IObservable<TOther> other)
        {
            return Observable.TakeUntil(source, other);
        }
    }
}