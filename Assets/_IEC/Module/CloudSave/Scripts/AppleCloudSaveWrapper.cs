using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FancyHole.SkipRename
{
    public class AppleCloudSaveWrapper
    {
        public delegate void iCloudSyncCallback();
        
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void Synchronize();

        [DllImport("__Internal")]
        public static extern void SaveToiCloud(string key, string value);

        [DllImport("__Internal")]
        public static extern void RemoveFromiCloud(string key);

        [DllImport("__Internal")]
        public static extern string LoadFromiCloud(string key);

        [DllImport("__Internal")]
        public static extern bool iCloudAvailable();

        [DllImport("__Internal")]
        public static extern void RegisteriCloudSyncCallback(iCloudSyncCallback  callback);
#else
        public static void Synchronize()
        {
            Debug.LogError("Synchronize");
        }
        
        public static void SaveToiCloud(string key, string value)
        {
            Debug.LogError($"SaveToiCloud: {key} = {value}");
        }
        
        public static void RemoveFromiCloud(string key)
        {
            Debug.LogError($"RemoveFromiCloud: {key}");
        }

        public static string LoadFromiCloud(string key)
        {
            Debug.LogError($"LoadFromiCloud: {key}");   
            return null;
        }

        public static bool iCloudAvailable()
        {
            return false;
        }
        
        public static void RegisteriCloudSyncCallback(iCloudSyncCallback callback)
        {
            Debug.LogError("RegisteriCloudSyncCallback");
        }
#endif
    }
}