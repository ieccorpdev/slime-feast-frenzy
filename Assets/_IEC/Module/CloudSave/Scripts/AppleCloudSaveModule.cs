#if UNITY_IOS && ENABLE_ICLOUD_SAVE
using System;
using System.Collections.Generic;
using FancyHole.Features.CloudSave;
using FancyHole.SkipRename;
using UnityEngine;

namespace FancyHole
{
    public class AppleCloudSaveModule : ICloudSaveModule
    {
        private static AppleCloudSaveWrapper.iCloudSyncCallback _cachedNativeCallback;
        private static Action _onSyncedFromUser;


        public string UserId => "0"; // Apple does not provide a user ID for iCloud

        public void Authenticate()
        {
            AppleCloudSaveWrapper.Synchronize();
        }

        public void Authenticate(Action callback)
        {
            _onSyncedFromUser = callback;

            // Register native-compatible static callback only once
            if (_cachedNativeCallback == null)
            {
                _cachedNativeCallback = OnNativeCloudSynced;
                AppleCloudSaveWrapper.RegisteriCloudSyncCallback(_cachedNativeCallback);
            }
            
            AppleCloudSaveWrapper.Synchronize();
        }

        public bool IsAvailable()
        {
            return AppleCloudSaveWrapper.iCloudAvailable();
        }

        public void SaveData(CloudSaveData save)
        {
            AppleCloudSaveWrapper.SaveToiCloud(CloudSaveController.CLOUD_SAVE_KEY,CloudSaveData.ConvertDataToJson(save));
        }

        public void LoadData(Action<string> onLoadCompleted)
        {
            var json = AppleCloudSaveWrapper.LoadFromiCloud(CloudSaveController.CLOUD_SAVE_KEY);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("LOAD CLOUD DATA FAILED");
                onLoadCompleted?.Invoke(null);
            }
            else
            {
                onLoadCompleted?.Invoke(json);
            }
        }

        public void ClearData(List<string> keys)
        {
            if (keys == null || keys.Count == 0) return;
            foreach (var key in keys)
            {
                AppleCloudSaveWrapper.RemoveFromiCloud(key);
            }
        }

        public void OnSyncComplete(Action callback)
        {
            _onSyncedFromUser = callback;

            // Register native-compatible static callback only once
            if (_cachedNativeCallback == null)
            {
                _cachedNativeCallback = OnNativeCloudSynced;
                AppleCloudSaveWrapper.RegisteriCloudSyncCallback(_cachedNativeCallback);
            }
        }
        
        [AOT.MonoPInvokeCallback(typeof(AppleCloudSaveWrapper.iCloudSyncCallback))]
        private static void OnNativeCloudSynced()
        {
            Debug.LogWarning("[iCloud] Native sync callback triggered.");
            _onSyncedFromUser?.Invoke();
        }
    }
}
#endif