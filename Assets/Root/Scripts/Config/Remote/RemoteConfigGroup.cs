using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Root
{
    [CreateAssetMenu(fileName = "Remote Configs", menuName = "Remote/Remote Config Group")]
    public class RemoteConfigGroup : ScriptableObject
    {
        public bool isTest;

        [Header("Android")]
        public RemoteConfig difficulty;

        List<RemoteConfig> listRemote = new List<RemoteConfig>();
        bool isInit = false;

        public void Init()
        {
            if (isInit)
            {
                return;
            }
            isInit = true;
            listRemote.Clear();
            Debug.Log("Init remote configs");
            var fields = GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].FieldType == (typeof(RemoteConfig)))
                {
                    listRemote.Add((RemoteConfig)fields[i].GetValue(this));
                }
            }
        }

        public List<RemoteConfig> GetListRemote()
        {
            Init();
            return listRemote;
        }
    }
}
