using System;
using System.Collections;
using UnityEditor;

namespace IEC.ModuleManager
{
    public class EditorCoroutine
    {
        private readonly Func<bool> _waitUntil;
        private readonly Action _callback;

        private EditorCoroutine(Func<bool> waitUntil, Action callback)
        {
            _waitUntil = waitUntil;
            _callback = callback;
        }

        public static EditorCoroutine StartCoroutine(float waitForSeconds, Action callback)
        {
            var startTime = DateTime.Now;
            return StartCoroutine(() => (DateTime.Now - startTime).TotalSeconds >= waitForSeconds, callback);
        }
        public static EditorCoroutine StartCoroutine(Func<bool> waitUntil, Action callback)
        {
            var coroutine = new EditorCoroutine(waitUntil, callback);
            coroutine.Start();
            return coroutine;
        }

        private void Start()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        /// <summary>
        /// Stops the coroutine.
        /// </summary>
        public void Stop()
        {
            if (EditorApplication.update == null) return;

            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            // Coroutine has ended, stop updating.
            if (!_waitUntil.Invoke()) return;
            _callback.Invoke();
            Stop();
        }
    }
}