using System;
using System.Collections.Generic;

namespace IEC.CloudSave
{
    public interface ICloudSaveModule
    {
        Action<bool> OnAuthenticated { get; set; }
        string UserId { get; }
        void Init();
        void Authenticate(Action callback, bool isManual);
        bool IsAvailable();
        void SaveData(CloudSaveData save);
        void LoadData(Action<string> onLoadCompleted);
        void ClearData(List<string> keys);
        void OnSyncComplete(Action callback);
    }
}