using System;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseConfig : ScriptableObject
    {
        private readonly Dictionary<string, object> CreateIfNotExistDictionary = new Dictionary<string, object>();

        protected T SingleCreateIfNotExist<T>(string customName, Func<T> createFunc)
        {
            return SingleCreateIfNotExist(createFunc, customName);
        }

        protected T SingleCreateIfNotExist<T>(Func<T> createFunc, string customName = null)
        {
            var dictKey = typeof(T).ToString();
            if (!string.IsNullOrEmpty(customName)) dictKey = customName;
            if (!CreateIfNotExistDictionary.ContainsKey(dictKey)) CreateIfNotExistDictionary[dictKey] = createFunc();
            return (T)CreateIfNotExistDictionary[dictKey];
        }

        protected void SetSingleCreateIfNotExist<T>(T obj, string customName = null)
        {
            var dictKey = typeof(T).ToString();
            if (!string.IsNullOrEmpty(customName)) dictKey = customName;
            CreateIfNotExistDictionary[dictKey] = obj;
        }

        protected void SingleClear<T>()
        {
            var type = typeof(T);
            var typeStr = type.ToString();
            if (CreateIfNotExistDictionary.ContainsKey(typeStr))
            {
                CreateIfNotExistDictionary.Remove(typeStr);
            }
        }

        protected void SingleClear(string customName)
        {
            var typeStr = customName;
            if (CreateIfNotExistDictionary.ContainsKey(typeStr))
            {
                CreateIfNotExistDictionary.Remove(typeStr);
            }
        }

        public void SetDirtyAndSave()
        {
            UnityEditorHelper.SetDirtyAndSave(this);
        }
    }
}