using System;
using OgurySdk.Internal.Message;
using UnityEngine;
// ReSharper disable UseNullPropagation

namespace OgurySdk
{
    public class OguryCallbacks : MonoBehaviour
    {
        #region Ogury Ads callbacks

        public event InternalEventHandler OnAdAvailable;

        public event InternalEventHandler OnAdNotAvailable;

        public event InternalEventHandler OnAdLoaded;

        public event InternalEventHandler OnAdNotLoaded;

        public event InternalEventHandler OnAdDisplayed;
        
        public event InternalEventHandler OnAdClicked;

        public event InternalRewardedEventHandler OnAdRewarded;

        public event InternalEventHandler OnAdClosed;

        public event InternalOguryErrorEventHandler OnAdError;
        
        public delegate void InternalEventHandler(int instanceId);

        public delegate void InternalRewardedEventHandler(int instanceId, OguryRewardItem rewardItem);

        #endregion

        #region Ogury Choice Manager callbacks

        public event InternalCompleteEventHandler OnAskComplete;

        public event InternalOguryErrorEventHandler OnAskError;

        public event InternalCompleteEventHandler OnEditComplete;

        public event InternalOguryErrorEventHandler OnEditError;
        
        public delegate void InternalCompleteEventHandler(OguryChoiceManager.Answer answer);

        #endregion
        
        #region Ogury Consent Manager callbacks
        
        public event InternalLegacyCompleteEventHandler LegacyOnAskComplete;

        public event InternalOguryErrorEventHandler LegacyOnAskError;

        public event InternalLegacyCompleteEventHandler LegacyOnEditComplete;

        public event InternalOguryErrorEventHandler LegacyOnEditError;
        
        public delegate void InternalLegacyCompleteEventHandler();
        
        #endregion
        
        public delegate void InternalOguryErrorEventHandler(int instanceId, OguryError oguryError);

        // Singleton
        public static OguryCallbacks Instance { get; private set; }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // When we navigate between scene, a new game object with this script can be created.
                // We destroy it immediately to ensure we only have one instance.
                // Otherwise we will have cases where we will trigger events multiple times.
                Destroy(this);
            }
        }

        public void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public static Boolean CheckIfPresent()
        {
            if (Instance == null)
            {
                Debug.LogWarning(
                    "No component of type OguryCallbacks available. No event will be triggered by the Ogury SDK. "
                    + "Make sure to create a game object in your first scene with the OguryCallbacks script attached to it."
                );
                return false;
            }

            return true;
        }

        #region Ogury Ads callbacks from native layer
        
        public void ForwardOnAdAvailable(string payload)
        {
            SimpleMessage msg = JsonUtility.FromJson<SimpleMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("OnAdAvailable: " + msg.instanceId);
            }
            if (OnAdAvailable != null)
            {
                OnAdAvailable.Invoke(msg.instanceId);
            }
        }
        
        public void ForwardOnAdNotAvailable(string payload)
        {
            SimpleMessage msg = JsonUtility.FromJson<SimpleMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("OnAdNotAvailable: " + msg.instanceId);
            }
            if (OnAdNotAvailable != null)
            {
                OnAdNotAvailable.Invoke(msg.instanceId);
            }
        }
        
        public void ForwardOnAdLoaded(string payload)
        {
            SimpleMessage msg = JsonUtility.FromJson<SimpleMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("OnAdLoaded: " + msg.instanceId);
            }
            if (OnAdLoaded != null)
            {
                OnAdLoaded.Invoke(msg.instanceId);
            }
        }
        
        public void ForwardOnAdNotLoaded(string payload)
        {
            SimpleMessage msg = JsonUtility.FromJson<SimpleMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("OnAdNotLoaded: " + msg.instanceId);
            }
            if (OnAdNotLoaded != null)
            {
                OnAdNotLoaded.Invoke(msg.instanceId);
            } 
        }
        
        public void ForwardOnAdDisplayed(string payload)
        {
            SimpleMessage msg = JsonUtility.FromJson<SimpleMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("OnAdDisplayed: " + msg.instanceId);
            }
            if (OnAdDisplayed != null)
            {
                OnAdDisplayed.Invoke(msg.instanceId);
            }
        }
        
        public void ForwardOnAdClicked(string payload)
        {
            SimpleMessage msg = JsonUtility.FromJson<SimpleMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("OnAdClicked: " + msg.instanceId);
            }
            if (OnAdClicked != null)
            {
                OnAdClicked.Invoke(msg.instanceId);
            }
        }

        public void ForwardOnAdRewarded(string payload)
        {
            RewardItemMessage msg = JsonUtility.FromJson<RewardItemMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("OnAdRewarded: " + msg.instanceId);
            }
            if (OnAdRewarded != null)
            {
                OnAdRewarded.Invoke(msg.instanceId, new OguryRewardItem(msg.name, msg.value));
            }
        }
        
        public void ForwardOnAdClosed(string payload)
        {
            SimpleMessage msg = JsonUtility.FromJson<SimpleMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("OnAdClosed: " + msg.instanceId);
            }
            if (OnAdClosed != null)
            {
                OnAdClosed.Invoke(msg.instanceId);
            }
        }
        
        public void ForwardOnAdError(string payload)
        {
            OguryErrorMessage msg = JsonUtility.FromJson<OguryErrorMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("OnAdError: " + msg.instanceId + " - Error Code: " + msg.errorCode);
            }
            OguryError error = new OguryError(msg.errorCode, msg.description);
            if (OnAdError != null)
            {
                OnAdError.Invoke(msg.instanceId, error);
            }
        }
        
        #endregion
        
        #region Ogury Consent Manager callbacks from native layer
        
        public void ForwardLegacyOnAskCompleted(string payload)
        {
            SimpleMessage msg = JsonUtility.FromJson<SimpleMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("LegacyOnAskCompleted");
            }
            if (LegacyOnAskComplete != null)
            {
                LegacyOnAskComplete.Invoke();
            }
        }
        
        public void ForwardLegacyOnAskFailed(string payload)
        {
            OguryErrorMessage msg = JsonUtility.FromJson<OguryErrorMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("LegacyOnAskFailed - Error Code: " + msg.errorCode);
            }
            OguryError error = new OguryError(msg.errorCode, msg.description);
            if (LegacyOnAskError != null)
            {
                LegacyOnAskError.Invoke(msg.instanceId, error);
            }
        }
        
        public void ForwardLegacyOnEditCompleted(string payload)
        {
            SimpleMessage msg = JsonUtility.FromJson<SimpleMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("LegacyOnEditCompleted");
            }
            if (LegacyOnEditComplete != null)
            {
                LegacyOnEditComplete.Invoke();
            }
        }
        
        public void ForwardLegacyOnEditFailed(string payload)
        {
            OguryErrorMessage msg = JsonUtility.FromJson<OguryErrorMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("LegacyOnEditFailed - Error Code: " + msg.errorCode);
            }
            OguryError error = new OguryError(msg.errorCode, msg.description);
            if (LegacyOnEditError != null)
            {
                LegacyOnEditError.Invoke(msg.instanceId, error);
            }
        }

        #endregion

        #region Ogury Choice Manager callbacks from native layer
        
        public void ForwardOnAskCompleted(string payload)
        {
            OguryChoiceManagerAnswerMessage msg = JsonUtility.FromJson<OguryChoiceManagerAnswerMessage>(payload);
            OguryChoiceManager.Answer answer = (OguryChoiceManager.Answer) Enum.Parse(typeof(OguryChoiceManager.Answer), msg.answer);
            if (msg.debug)
            {
                Debug.Log("OnAskCompleted - Answer: " + answer);
            }
            if (OnAskComplete != null)
            {
                OnAskComplete.Invoke(answer);
            }
        }
        
        public void ForwardOnAskFailed(string payload)
        {
            OguryErrorMessage msg = JsonUtility.FromJson<OguryErrorMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("OnAskFailed - Error Code: " + msg.errorCode);
            }
            OguryError error = new OguryError(msg.errorCode, msg.description);
            if (OnAskError != null)
            {
                OnAskError.Invoke(msg.instanceId, error);
            }
        }
        
        public void ForwardOnEditCompleted(string payload)
        {
            OguryChoiceManagerAnswerMessage msg = JsonUtility.FromJson<OguryChoiceManagerAnswerMessage>(payload);
            OguryChoiceManager.Answer answer = (OguryChoiceManager.Answer) Enum.Parse(typeof(OguryChoiceManager.Answer), msg.answer);
            if (msg.debug)
            {
                Debug.Log("OnEditCompleted - Answer: " + answer);
            }
            if (OnEditComplete != null)
            {
                OnEditComplete.Invoke(answer);
            }
        }
        
        public void ForwardOnEditFailed(string payload)
        {
            OguryErrorMessage msg = JsonUtility.FromJson<OguryErrorMessage>(payload);
            if (msg.debug)
            {
                Debug.Log("OnEditFailed - Error Code: " + msg.errorCode);
            }
            OguryError error = new OguryError(msg.errorCode, msg.description);
            if (OnEditError != null)
            {
                OnEditError.Invoke(msg.instanceId, error);
            }
        }

        #endregion
    }
}