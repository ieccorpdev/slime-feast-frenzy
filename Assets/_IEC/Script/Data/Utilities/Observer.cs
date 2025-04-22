using System.Collections.Generic;

namespace IEC
{
    public static class Observer
    {
        public delegate void CallbackObserverParam(params object[] data);

        public delegate void CallbackObserverVoid();

        private static readonly Dictionary<string, HashSet<CallbackObserverParam>> CallbackPara = new Dictionary<string, HashSet<CallbackObserverParam>>();

        private static readonly Dictionary<string, HashSet<CallbackObserverVoid>> CallbackVoid = new Dictionary<string, HashSet<CallbackObserverVoid>>();

        public static void AddObserver(string eventName, CallbackObserverVoid callback)
        {
            CreateListObserverForEventVoid(eventName).Add(callback);
        }

        public static void AddObserver(string eventName, CallbackObserverParam callbackObserverParam)
        {
            CreateListObserverForEventParam(eventName).Add(callbackObserverParam);
        }

        public static void RemoveObserver(string eventName, CallbackObserverVoid callback)
        {
            var listObserver = CreateListObserverForEventVoid(eventName);
            listObserver.Remove(callback);
        }

        public static void RemoveObserver(string eventName, CallbackObserverParam callbackObserverParam)
        {
            var listObserver = CreateListObserverForEventParam(eventName);
            listObserver.Remove(callbackObserverParam);
        }

        public static void RemoveAllObserver()
        {
            CallbackPara.Clear();
            CallbackVoid.Clear();
        }

        public static void Notify(string eventName, params object[] data)
        {
            foreach (var observer in CreateListObserverForEventParam(eventName))
                observer(data);
            foreach (var observer in CreateListObserverForEventVoid(eventName))
                observer();
        }

        public static void Notify(string eventName)
        {
            foreach (var observer in CreateListObserverForEventParam(eventName))
                observer();
            foreach (var observer in CreateListObserverForEventVoid(eventName))
                observer();
        }

        private static HashSet<CallbackObserverParam> CreateListObserverForEventParam(string eventName)
        {
            if (!CallbackPara.ContainsKey(eventName))
                CallbackPara.Add(eventName, new HashSet<CallbackObserverParam>());
            return CallbackPara[eventName];
        }

        private static HashSet<CallbackObserverVoid> CreateListObserverForEventVoid(string eventName)
        {
            if (!CallbackVoid.ContainsKey(eventName))
                CallbackVoid.Add(eventName, new HashSet<CallbackObserverVoid>());
            return CallbackVoid[eventName];
        }
    }
}