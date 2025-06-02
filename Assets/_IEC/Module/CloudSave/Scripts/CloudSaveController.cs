using System;
using System.Collections.Generic;
using System.Linq;
using _IECModules;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IEC.CloudSave
{
    public class CloudSaveController : _BaseBehaviour, IController, IEventAwake, IEventPauseApp,
        IEventQuitApp, IEventListener, IEventLateUpdate
    {
        public const string CLOUD_SAVE_KEY = "CLOUD_SAVE_KEY";
        
        public CloudSaveData cloudSaveData;
        public CloudSaveInstallData cloudSaveInstallData;

        private ICloudSaveModule _cloudSaveModule;
        private bool _isLoadedFromCloud;
        private bool _isNeedSaveData;
        private bool _isAuthenticatedDone;
        private bool _isStartTimeoutCountdown;
        private bool _isLevelLoadedFirstTime;
        private bool _isNeedShowPopupCloudSave;
        private float _timeFromStart = 0f;
        
        public string UserId => _cloudSaveModule.UserId;
        public bool IsUserSignedIn => UserId != "0" && UserId != "UnknownID" && UserId != "Unavailable Player Identification";

        private DataController DataController => Controllers.Get<DataController>();

        private void Update()
        {
            UpdateTimeout();
        }
        
        private void UpdateTimeout()
        {
            if (!_isStartTimeoutCountdown) return;
            _timeFromStart += Time.deltaTime;
        }

        public bool IsCloudSaveAvailableAndBelongToUser()
        {
            return IsCloudSaveAvailable() && IsCloudSaveBelongToUser();
        }

        private bool IsCloudSaveAvailable()
        {
            return _cloudSaveModule.IsAvailable();
        }
        
        public bool IsCloudSaveBelongToUser()
        {
            if (!IsUserSignedIn) return false; // if user not signed it, return false
            if (string.IsNullOrEmpty(PlayerData.cloudSaveUserId)) return true;
            if (!IsUserSignedIn) return false; // if user not signed it, return false
            return UserId == PlayerData.cloudSaveUserId;
        }

        public bool IsCloudSaveBelongToUser(PlayerSecret cloudData)
        {
            if (cloudData == null) return true;
            if (!IsUserSignedIn) return false; // if user not signed it, return false
            if (string.IsNullOrEmpty(cloudData.cloudSaveUserId)) return true;
            return UserId == cloudData.cloudSaveUserId;
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.SAVE_DATA:
                    _isNeedSaveData = true;
                    Debug.LogWarning("[CLOUD SAVE]: Raise Save Data");
                    break;
                case EventList.GamePlay.HOME:
                    OnScreenInGameOrScreenHomeShowed(null);
                    break;
            }
        }

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public void BehaviourAwake()
        {
            this.RegisterListener(EventList.GamePlay.SCREEN_CHANGE, OnScreenInGameOrScreenHomeShowed);
#if UNITY_ANDROID
            _cloudSaveModule = new GooglePlayCloudSaveModule();
#elif UNITY_IOS
            _cloudSaveModule = new AppleGameCenterCloudSaveModule();
#endif
            _cloudSaveModule.Init();
            _cloudSaveModule.OnAuthenticated += OnAuthenticatedFixed;
            _cloudSaveModule.Authenticate(OnAuthenticated, false);
            
            _isStartTimeoutCountdown = true;

            cloudSaveInstallData = DataController.LoadData<CloudSaveInstallData>(CLOUD_SAVE_KEY);

            ModuleController.cheatCode.AddCheatCodeButton("Clear Cloud Data",
                (param) =>
                {
                    var listKeys = new List<string>();
                    listKeys.Add(CLOUD_SAVE_KEY);
                    _cloudSaveModule.ClearData(listKeys);
                    cloudSaveInstallData.isCloudSaveOn = false;
                    SaveInstallData();
                });

            ModuleController.cheatCode.AddCheatCodeButton("Log Cloud Data",
                (param) =>
                {
                    _cloudSaveModule.LoadData((data) =>
                    {
                        Debug.LogError($"CLOUD DATA: {data}");
                    });
                });
            
            ModuleController.cheatCode.AddCheatCodeButton("Log User ID",
                (param) =>
                {
                    Debug.LogError($"Cloud User ID: {UserId}");
                    Debug.LogError($"Local User ID: {PlayerData.cloudSaveUserId}");
                });
        }

        // This will always be called after authenticate, while OnAuthenticated can be overriden
        private void OnAuthenticatedFixed(bool result)
        {
            if (cloudSaveInstallData.isCloudSaveOn && result && IsUserSignedIn && IsCloudSaveBelongToUser())
            {
                PlayerData.cloudSaveUserId = UserId;
            }
            _isAuthenticatedDone = result;
            this.RaiseEvent(EventList.GamePlay.ON_USER_AUTHENTICATE_DONE);
        }

        private void OnAuthenticated()
        {
            _cloudSaveModule.LoadData(LoadData);
            UI.PopUpHomeSettings.CheckCloudSaveBelongToUser();
        }

        private void OnScreenInGameOrScreenHomeShowed(object obj)
        {
            if (obj == null) return;
            var screen = ((ScreenChangeData)obj).screenUI;
            if (!screen) return;
            if (screen != UI.ScreenHome && screen != UI.ScreenInGame) return;
            
            if (_isLevelLoadedFirstTime) return;
            _isLevelLoadedFirstTime = true;
            if (!_isNeedShowPopupCloudSave) return;
            ShowPopupCloudSave();
        }

        public void BehaviourLateUpdate(float deltaTime)
        {
            if (_isNeedSaveData)
            {
                SaveData();
                _isNeedSaveData = false;
            }
        }

        public void BehaviourPauseApp(bool isPause)
        {
            if (isPause)
                SaveData();
        }

        public void BehaviourQuitApp()
        {
            SaveData();
        }

        [Button]
        public void SaveData()
        {
            SaveInstallData();
            SaveToCloud();
        }

        private void SaveToCloud()
        {
            if (!cloudSaveInstallData.isCloudSaveOn) return;
            if (!_isAuthenticatedDone) return;
            if (cloudSaveData == null)
            {
                cloudSaveData = new CloudSaveData();
            }

            if (!IsCloudSaveBelongToUser())
            {
                Debug.LogError("[CLOUD SAVE]: User ID is different");
                return;
            }
            
            Debug.LogWarning("[CLOUD SAVE] Save Data");
            _cloudSaveModule.SaveData(DataController.ConvertToCloudSaveData(cloudSaveData));
        }

        public void SaveInstallData()
        {
            if (cloudSaveInstallData == null)
            {
                cloudSaveInstallData = new CloudSaveInstallData();
            }

            Services.DataSecure().SetObject(CLOUD_SAVE_KEY, cloudSaveInstallData);
        }

        private void LoadData(string data)
        {
            if (_isLoadedFromCloud) return;
            // if (_isTimeout) return;

            Debug.LogWarning("[CLOUD SAVE] Load Data In Callback");

            if (data == null)
            {
                // Probably for first time load
                Debug.LogError("[CLOUD SAVE]: No Cloud Data Loaded, Will Create New Data From Local Data");
                cloudSaveData = new CloudSaveData();
                cloudSaveData = DataController.ConvertToCloudSaveData(cloudSaveData);
                _isLoadedFromCloud = true;
                return;
            }

            cloudSaveData = CloudSaveData.ConvertJsonToData(data);
            Debug.LogWarning($"[CLOUD SAVE] Cloud Data Loaded: {data}");
            _isLoadedFromCloud = true;

            ShowDifferenceFirstTime();
        }
        
        public void ApplyCloudSaveData()
        {
            if (!_isLoadedFromCloud) return;
            if (cloudSaveData == null) return;
            
            Debug.LogWarning("[CLOUD SAVE] Apply Cloud Save Data");
            DataController.ApplyCloudSave(cloudSaveData);
        }

        public void ApplyLocalSaveData()
        {
            cloudSaveData = DataController.ConvertToCloudSaveData(cloudSaveData);
            SaveData();
        }

        private void ShowDifferenceFirstTime()
        {
            if (cloudSaveInstallData.isOldUserFirstTime && !cloudSaveInstallData.isCloudSaveOn) return;
            SaveInstallData();
            ShowDifference();
            cloudSaveInstallData.isOldUserFirstTime = true;
        }
        
        public void ShowDifference()
        {
            if (cloudSaveData == null)
            {
                Debug.LogError("[CLOUD SAVE] Cloud Data Is Null");
                return;
            }

            var localPlayerData = Pool.Get<PlayerSecret>();
            var cloudPlayerData = GetCloudPlayerData();

            var isDataSimilar = CompareCoreData(localPlayerData, cloudPlayerData);
            var isDataBelongToUser = IsCloudSaveBelongToUser(cloudPlayerData);
            Debug.LogWarning($"[CLOUD SAVE] Is Data Similar: {isDataSimilar}");
            Debug.LogWarning($"[CLOUD SAVE] Is Data Belong To User: {isDataBelongToUser}");
            if (!isDataBelongToUser)
            {
                Debug.LogError("[CLOUD SAVE] Cloud Data Belong To Other User");
                return;
            }
            
            if (!isDataSimilar)
            {
                if (!_isLevelLoadedFirstTime)
                {
                    _isNeedShowPopupCloudSave = true;
                    return;
                }

                ShowPopupCloudSave();
            }
        }

        [Button]
        void ShowPopupCloudSave()
        {
            UI.PopupCloudSave.SetupFirstTime(!cloudSaveInstallData.isCloudSaveOn);
            UI.ShowScreen(UI.PopupCloudSave);
            _isNeedShowPopupCloudSave = false;
        }

        // return true if data is similar, false if different
        private bool CompareCoreData(PlayerSecret localData, PlayerSecret cloudData)
        {
            if (localData == null || cloudData == null) return true;

            if (localData.level != cloudData.level) return false;
            if (localData.gold != cloudData.gold) return false;

            return true;
        }

        public PlayerSecret GetCloudPlayerData()
        {
            if (cloudSaveData == null) return null;
            if (cloudSaveData.dictDatas.TryGetValue(DataController.PLAYER_DATA_KEY, out var data))
            {
                Debug.LogWarning($"[CLOUD SAVE] Cloud Data: {data}");
                return JsonConvert.DeserializeObject<PlayerSecret>(data);
            }

            Debug.LogError("[CLOUD SAVE] PlayerData Not Found In Cloud Save");
            return null;
        }
        
        public void Authenticate(Action callback)
        {
            callback += OnAuthenticated;
            _cloudSaveModule.Authenticate(callback, true);
        }
    }
}