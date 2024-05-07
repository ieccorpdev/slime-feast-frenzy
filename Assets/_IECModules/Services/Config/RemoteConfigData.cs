using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(fileName = "RemoteConfig", menuName = "Services/RemoteConfig")]
    public class RemoteConfigData : ScriptableObject
    {
        public string keyName;
        public string testValue;
        public string defaultValue;
        //public RemoteDataType type;

        public T GetDefault<T>()
        {
            try
            {
                return (T)Convert.ChangeType(defaultValue, typeof(T));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default;
            }
        }
    }
}

public enum RemoteDataType
{
    String,
    Int,
    Bool
}