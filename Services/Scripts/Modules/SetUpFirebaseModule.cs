#if (PluginFirebase)

using System;
using UnityEngine;
using Firebase;
using Firebase.Extensions;

namespace IEC
{
    public class SetUpFirebaseModule : ISetUpSdkService
    {
        SdkState m_State = SdkState.NotInit;
        Action<bool> m_CallBack;

        public void Init(Action<bool> callback)
        {
            m_CallBack = callback;
            if (m_State == SdkState.Initializing)
            {
                Debug.LogError("Call Init Firebase while initializing");
                return;
            }

            if (m_State == SdkState.Initialized)
            {
                Debug.LogError("Call Init Firebase but initialized");
                if (callback != null)
                {
                    callback.Invoke(true);
                }
                return;
            }

            if (m_State == SdkState.FailInit)
            {
                Debug.LogError("Call Init Firebase but already fail initialized");
                if (callback != null)
                {
                    callback.Invoke(false);
                }
                return;
            }

            m_State = SdkState.Initializing;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    m_State = SdkState.Initialized;
                    if (m_CallBack != null)
                    {
                        m_CallBack.Invoke(true);
                    }
                }
                else
                {
                    m_State = SdkState.FailInit;
                    Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    if (m_CallBack != null)
                    {
                        m_CallBack.Invoke(false);
                    }
                }
            });
        }

        public SdkState GetState()
        {
            return m_State;
        }

        public bool IsReady()
        {
            return m_State == SdkState.Initialized;
        }
    }
}

#endif