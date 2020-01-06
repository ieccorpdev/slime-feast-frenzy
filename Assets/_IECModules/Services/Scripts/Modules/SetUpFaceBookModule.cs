#if (PluginFacebook)

using System;
using UnityEngine;
using Facebook.Unity;

namespace Root
{
    public class SetUpFacebookModule : ISetUpSdkService
    {
        SdkState m_State = SdkState.NotInit;

        public void Init(Action<bool> callback)
        {
            if (m_State == SdkState.Initializing)
            {
                Debug.Log("Call Init Facebook while initializing");
                return;
            }

            if (m_State == SdkState.Initialized)
            {
                Debug.Log("Call Init Facebook but initialized");
                return;
            }

            if (FB.IsInitialized)
            {
                m_State = SdkState.Initialized;
                FB.ActivateApp();
                Debug.Log("Facebook init done");

                UnityToolbag.Dispatcher.InvokeAsync(() =>
                {
                    if (callback != null)
                    {
                        callback.Invoke(true);
                    }
                });
            }
            else
            {
                m_State = SdkState.Initializing;
                FB.Init(() =>
                {
                    if (FB.IsInitialized)
                    {
                        m_State = SdkState.Initialized;
                        FB.ActivateApp();
                        Debug.Log("Facebook init done");
                    }
                    else
                    {
                        m_State = SdkState.NotInit;
                    }
                    
                    UnityToolbag.Dispatcher.InvokeAsync(() =>
                    {
                        if (callback != null)
                        {
                            callback.Invoke(FB.IsInitialized);
                        }
                    });
                });
            }
        }

        public SdkState GetState()
        {
            return m_State;
        }

        public bool IsReady()
        {
            return FB.IsInitialized;
        }
    }
}

#endif