using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(fileName = "RemoteConfig", menuName = "Services/RemoteConfig")]
    public class RemoteConfigData : ScriptableObject
    {
        public string keyName;
        public string remoteKeyAndroid;
        public string remoteKeyIOS;
        public string testValue;
    }
}