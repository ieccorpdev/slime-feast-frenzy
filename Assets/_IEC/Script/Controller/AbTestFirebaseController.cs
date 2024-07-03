#if AbTestFirebase
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    public class AbTestFirebaseController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        public ScreenLoading screenLoading;
        private RemoteConfigResource s_RemoteResource => Services.config.remoteConfigResource;
        private bool m_IsRemoteLoadDoneOrTimeout = false;
        private bool m_IsFirebaseLoaded = false;
        private bool m_IsOverrideAb = false;

        private const string OLD_USER_NO_REMOTE = "AbTestFirebaseController.OLD_USER_NO_REMOTE";

        private IDataService RemoteData => Services.DataRemote();
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
                    break;

                case EventList.Service.REMOTE_CONFIG_LOADED:
                    if(m_IsRemoteLoadDoneOrTimeout) break;
                    m_IsRemoteLoadDoneOrTimeout = true;
                    m_IsFirebaseLoaded = (bool) param.p0;
                    screenLoading.Hide();
                    break;
            }
        }
        
        private void ApplyRemoteConfig()
        {
            var isOldUser = Services.DataNormal().GetBool(OLD_USER_NO_REMOTE, false);
            if (isOldUser || (!m_IsOverrideAb && !Services.IsNewInstall))
            {
                var reason = isOldUser ? "old user" : "overrideAB";
                Debug.LogWarning($"[ABRCT] AB OVERRIDE - reason: {reason}");
                ((FirebaseRemoteConfigModule)RemoteData).DefaultOverrideRemote();
            }
            
            
            //Setup feature
            SetupFeature();
        }

        void SetupFeature()
        {
            /* Example
            Game.levelDifficultyVersion = RemoteData.GetInt(s_RemoteResource.levelDifficult.keyName, s_RemoteResource.levelDifficult.GetDefault<int>());
            Game.sagaBackHomeLevel = RemoteData.GetInt(s_RemoteResource.homeActivation.keyName, s_RemoteResource.homeActivation.GetDefault<int>());
            Game.shopActivationLevel = RemoteData.GetInt(s_RemoteResource.shopActivation.keyName, s_RemoteResource.shopActivation.GetDefault<int>());
            */
        }

        public void BehaviourAwake()
        {
            screenLoading.hideAction = new Action(() =>
            {
                m_IsRemoteLoadDoneOrTimeout = true;
                ProcessAbVersionEvent(m_IsFirebaseLoaded);
                ApplyRemoteConfig();
                EventController.RaiseEvent(EventList.Service.CLOSE_LOADING_SCREEN);
            });
        }

        public void BehaviourStart()
        {
        }


        private void ProcessAbVersionEvent(bool isSuccess)
        {
            var abConfig = s_RemoteResource.abConfig;
            var str = "Processing Ab Version Event";
            var serviceData = Pool.Get<ServiceData>();

            var abConfigStr = RemoteData.GetString(abConfig.keyName, abConfig.defaultValue);
            var abConfigValue = Services.Json().FromJson<FirebaseAbConfig>(abConfigStr);
            var abVersion = abConfigValue.version;
            m_IsOverrideAb = abConfigValue.isOverride;
            if (serviceData.listSavedAbVersion == null) serviceData.listSavedAbVersion = new List<string>();
            var count = serviceData.listSavedAbVersion.Count;
            var list = serviceData.listSavedAbVersion;

            var log = list.Aggregate("[ABRCT]", (current, ab) => current + $"{ab}\n");

            Debug.LogWarning(log);

            var split = abVersion.Split('_');
            Debug.LogWarning("Remote Ab Version: " + abVersion);

            //str += $"\nabVersion: {abVersion}";
            if (Services.IsNewInstall)
            {
                list.Add(abVersion);
                if(isSuccess)
                {
                    var versionStr = split[2].Remove(0, 1);
                    var isNumber = int.TryParse(versionStr, out int version);
                    var eventValue = (isNumber) ? version : 1;
                    if (!isNumber) Debug.LogError("Version Ab is not a number");
                    Debug.LogWarning("AbTest event value:" + eventValue);
                    Services.PushEvent($"AB_{split[1]}", eventValue, new Dictionary<string, object>
                    {
                        ["version"] = abVersion
                    });
                    Services.SetUserProperty("AB_Version", abVersion);
                    m_IsOverrideAb = true;
                }
                Debug.LogWarning($"{str} Result: NEW_USER");
                RemoteData.SetString(abConfig.keyName, abConfigStr);
                RemoteData.Save();
                return;
            }
            
            if(!isSuccess) return;
            if (!list.Contains(abVersion))
            {
                if (list.Count == 0)
                {
                    Services.DataNormal().SetBool(OLD_USER_NO_REMOTE, true);
                    Services.DataNormal().Save();
                    var defaultVersion = Services.Json().FromJson<FirebaseAbConfig>(abConfig.defaultValue).version;
                    list.Add(defaultVersion);
                }
                
                if(!m_IsOverrideAb) return;
                var lastAb = list.Last();
                var lastAbPrefix = lastAb.Remove(lastAb.Length - 1, 1);
                Debug.LogWarning("Last AB:" + lastAb + "," + lastAbPrefix);
                var abPrefix = abVersion.Remove(abVersion.Length - 1, 1);
                Debug.LogWarning("AB Remote:" + abVersion + "," + abPrefix);
                if (lastAbPrefix == abPrefix)
                {
                    Debug.LogWarning($"{str} Result: AB failed user");
                    return;
                }
                
                Services.DataNormal().SetBool(OLD_USER_NO_REMOTE, false);
                Services.DataNormal().Save();
                
                list.Add(abVersion);
                var abShortVersion = $"{split[0]}_{split[1]}_v{split[2].Last()}";
                var listVersion = list.Skip(Mathf.Max(0, list.Count - 3)).ToList();
                var param = new Dictionary<string, object>
                {
                    ["new_version"] = abShortVersion,
                    ["old_version"] = lastAb,
                    ["list_version"] = listVersion
                };
                Services.PushEvent($"Override_AB_{split[1]}", 1, param);
                Debug.LogWarning($"{str} Result: OVERRIDE_USER");
                RemoteData.SetString(abConfig.keyName, abConfigStr);
                RemoteData.Save();
                return;
            }
            
            Debug.LogWarning($"{str} Result: OLD USER");
        }
    }
}
#endif

public class FirebaseAbConfig
{
    public string version;
    public bool isOverride;
}