//#define FIREBASE_REMOTE
#if FIREBASE_REMOTE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
namespace Root
{
    public class FirebaseRemoteModule : IRemoteServer
    {
        bool isInit = false;
        public RemoteConfigs remoteConfigs;
        IDataService data;

        public FirebaseRemoteModule(IDataService dataService, RemoteConfigs configs)
        {
            if (isInit)
            {
                return;
            }

            data = dataService;
            remoteConfigs = configs;
            remoteConfigs.Init();
            FirebaseCommon.Init(null, () =>
            {
                var listRemote = configs.GetListRemote();
                for (int i = 0; i < listRemote.Count; i++)
                {
                    switch (listRemote[i].type)
                    {
                        case RemoteType.Bool:
                            data.SetBool(listRemote[i].key, FirebaseRemoteConfig.GetValue(listRemote[i].key).BooleanValue);
                            break;
                        case RemoteType.Int:
                            data.SetInt(listRemote[i].key, (int)FirebaseRemoteConfig.GetValue(listRemote[i].key).LongValue);
                            break;
                        case RemoteType.String:
                            data.SetString(listRemote[i].key, FirebaseRemoteConfig.GetValue(listRemote[i].key).StringValue);
                            break;
                    }
                }
            });
        }

        public bool GetBoolValue(RemoteConfig config)
        {
            if (remoteConfigs.isTest)
            {
                return config.boolTest;
            }
            if (config.type == RemoteType.Bool)
            {
                if (FirebaseCommon.IsRemoteDone())
                {
                    return FirebaseRemoteConfig.GetValue(config.key).BooleanValue;
                }
                else
                {
                    return data.GetBool(config.key, config.boolDefault);
                }
            }
            else
            {
                Debug.Log("Wrong type config");
                return config.boolDefault;
            }
        }

        public int GetIntValue(RemoteConfig config)
        {
            if (remoteConfigs.isTest)
            {
                return config.intTest;
            }
            if (config.type == RemoteType.Int)
            {
                if (FirebaseCommon.IsRemoteDone())
                {
                    return (int)FirebaseRemoteConfig.GetValue(config.key).LongValue;
                }
                else
                {
                    return data.GetInt(config.key, config.intDefault);
                }
            }
            else
            {
                Debug.Log("Wrong type config");
                return config.intDefault;
            }
        }

        public string GetStringValue(RemoteConfig config)
        {
            if (remoteConfigs.isTest)
            {
                return config.stringTest;
            }
            if (config.type == RemoteType.String)
            {
                if (FirebaseCommon.IsRemoteDone())
                {
                    return FirebaseRemoteConfig.GetValue(config.key).StringValue;
                }
                else
                {
                    return data.GetString(config.key, config.stringDefault);
                }
            }
            else
            {
                Debug.Log("Wrong type config");
                return config.stringDefault;
            }
        }
    }
}
#endif