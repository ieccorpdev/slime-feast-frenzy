using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class BaseBehaviour : MonoBehaviour
{
    private static readonly Dictionary<Type, object> FindDictionary = new Dictionary<Type, object>();
    private readonly Dictionary<Type, object> GetDictionary = new Dictionary<Type, object>();
    private readonly Dictionary<Type, object> GetChildDictionary = new Dictionary<Type, object>();
    private readonly Dictionary<string, object> CreateIfNotExistDictionary = new Dictionary<string, object>();

    protected static T SingleFind<T>() where T : Object
    {
        var type = typeof(T);
        if (!FindDictionary.ContainsKey(type)) FindDictionary[type] = FindObjectOfType<T>();
        return (T) FindDictionary[type];
    }

    protected T SingleGet<T>() where T : Object
    {
        var type = typeof(T);
        if (!GetDictionary.ContainsKey(type)) GetDictionary[type] = GetComponent<T>();
        return (T) GetDictionary[type];
    }

    protected T SingleGetChild<T>(bool includeInactive = true) where T : Object
    {
        var type = typeof(T);
        if (!GetChildDictionary.ContainsKey(type))
            GetChildDictionary[type] = GetComponentInChildren<T>(includeInactive);
        return (T) GetChildDictionary[type];
    }

    protected T SingleCreateIfNotExist<T>(string customName, Func<T> createFunc)
    {
        return SingleCreateIfNotExist(createFunc, customName);
    }

    protected T SingleCreateIfNotExist<T>(Func<T> createFunc, string customName = null)
    {
        var dictKey = typeof(T).ToString();
        if (!string.IsNullOrEmpty(customName)) dictKey = customName;
        if (!CreateIfNotExistDictionary.ContainsKey(dictKey)) CreateIfNotExistDictionary[dictKey] = createFunc();
        return (T) CreateIfNotExistDictionary[dictKey];
    }

    public void SetSingleCreateIfNotExist<T>(T obj, string customName = null)
    {
        var dictKey = typeof(T).ToString();
        if (!string.IsNullOrEmpty(customName)) dictKey = customName;
        CreateIfNotExistDictionary[dictKey] = obj;
    }

    public T GetSingleCreate<T>(string customName = null) where T : Object
    {
        var dictKey = typeof(T).ToString();
        if (!string.IsNullOrEmpty(customName)) dictKey = customName;
        if (CreateIfNotExistDictionary.TryGetValue(dictKey, out var obj))
        {
            return (T)obj;
        }
        return null;
    }

    protected void SingleClear<T>()
    {
        var type = typeof(T);
        var typeStr = type.ToString();
        if (CreateIfNotExistDictionary.ContainsKey(typeStr))
        {
            CreateIfNotExistDictionary.Remove(typeStr);
        }
        if (FindDictionary.ContainsKey(type))
        {
            FindDictionary.Remove(type);
        }
    }

    protected void SingleClear(string customName)
    {
        if (CreateIfNotExistDictionary.ContainsKey(customName))
        {
            CreateIfNotExistDictionary.Remove(customName);
        }
    }

    protected void SingleClearCreateIfNotExist()
    {
        CreateIfNotExistDictionary.Clear();
    }

    protected void SingleClearFind()
    {
        FindDictionary.Clear();
    }
}