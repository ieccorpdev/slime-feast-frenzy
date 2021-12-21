using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IECModules;
using IEC.Linq;
using UnityEngine;
using NaughtyAttributes;
using TMPro;

namespace IEC
{
    public class AutoTestGroup : _BaseBehaviour, IGroup
    {
        const string SERVICE_DATA_KEY = "ServiceData";
        const string SERVICE_FIRST_TIME = "ServiceFirstTime";
        const string PLAYER_DATA_KEY = "PlayerData";

        public bool IsRecording { get; private set; }
        public float TimeStart { get; private set; }
        
        public bool IsEnableAltUnity { get; private set; }
        public DataAutoTestRecord DataRecord { get; private set; }

        public TMP_Text txtAb;

        public CheatCode.PositionActivate positionActivate = CheatCode.PositionActivate.TopLeft;

        public float cornerSize = 0.05f;

        private AltUnityRunner m_AltUnity => SingleGetChild<AltUnityRunner>();
        private int m_TestIndex;
        public void LocateGroup()
        {
            Pool.Set(this);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (IsRecording)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    DataRecord.listTap.Add(new DataTapRecord()
                    {
                        x = Input.mousePosition.x,
                        y = Input.mousePosition.y,
                        time = Time.time - TimeStart
                    });
                }
            }
            else if(!IsEnableAltUnity)
            {
                if (!Input.GetMouseButtonUp(0)) return;
                var isClickActivate = false;
                var isClickTop = Input.mousePosition.y > Screen.height * (1 - cornerSize);
                var isClickBot = Input.mousePosition.y < Screen.height * cornerSize;
                var isClickRight = Input.mousePosition.x > Screen.width * (1 - cornerSize);
                var isClickLeft = Input.mousePosition.x < Screen.width * cornerSize;

                isClickActivate |= ((positionActivate & CheatCode.PositionActivate.TopLeft) != 0 && isClickTop && isClickLeft);
                isClickActivate |= ((positionActivate & CheatCode.PositionActivate.TopRight) != 0 && isClickTop && isClickRight);
                isClickActivate |= ((positionActivate & CheatCode.PositionActivate.BotLeft) != 0 && isClickBot && isClickLeft);
                isClickActivate |= ((positionActivate & CheatCode.PositionActivate.BotRight) != 0 && isClickBot && isClickRight);

                if (isClickActivate)
                {
                    m_TestIndex++;
                    if (m_TestIndex != 8) return;
                    m_TestIndex = 0;
                    IsEnableAltUnity = true;
                    Debug.LogWarning("Enable AltUnity");
                    m_AltUnity.showPopUp = true;
                    m_AltUnity.gameObject.SetActive(true);
                    if (LastAbConfig() == null) txtAb.text = "Last AB null";
                    else
                    {
                        var abKey = LastAbConfig().KeyName;
                        var abValue = Services.ABTest().GetValue(abKey, 0);
                        txtAb.text = LastAbConfig().ListEventsAB[abValue];
                    }
                }
                else
                {
                    m_TestIndex = 0;
                }
            }
        }

        [Button]
        public void SetReady()
        {
            var readyFlag = transform.GetChild(0).gameObject;
            readyFlag.SetActive(true);
        }

        [Button]
        public void StartRecordAction()
        {
            var jsonModule = new JsonDotNetModule();

            var listAbTestConfig = Services.config.abTestConfigs.ToList();
            var abIndex = listAbTestConfig.FindLastIndex(x => Services.ABTest().HasValue(x.KeyName));

            var forceSetAb = "ForceSetAB";
            var forceSetAbName = Services.DataSecure().GetString(forceSetAb, "No Force Set");
            var forceSetAbIndex = listAbTestConfig.FindLastIndex(x => x.KeyName == forceSetAbName);
            abIndex = (forceSetAbIndex >= 0) ? forceSetAbIndex : abIndex;

            var abConfig = listAbTestConfig.ElementAtOrDefault(abIndex);
            var abValue = Services.ABTest().GetValue(abConfig.KeyName, 0);

            Services.ActiveNoAds(true);

            DataRecord = new DataAutoTestRecord()
            {
                abName = abConfig.KeyName,
                abValue = abValue,
                screenWidth = Screen.width,
                screenHeight = Screen.height,
                playerData = jsonModule.FromJson<PlayerSecret>(jsonModule.ToJson(Pool.Get<PlayerSecret>())),
                serviceData = jsonModule.FromJson<ServiceData>(jsonModule.ToJson(Pool.Get<ServiceData>())),
                listTap = new List<DataTapRecord>(),
            };

            IsRecording = true;
            TimeStart = Time.time;

            Debug.LogWarning("StartRecordAction: " + DataRecord.AbVersion);
        }

        [Button]
        public void StopRecordAction()
        {
            IsRecording = false;
            var jsonModule = new JsonDotNetModule();
            var json = jsonModule.ToJson(DataRecord);

            var fileName = "DataRecord-" + DataRecord.ScreenSize + "-" + DataRecord.AbVersion + ".json";
            var filePath = Application.persistentDataPath + "/" + fileName;
            var fileWriter = System.IO.File.CreateText(filePath);
            fileWriter.Write(json);
            fileWriter.Close();

            Debug.LogWarning("StopRecordAction: " + filePath);
        }

        public void SetRecordState(string jsonData)
        {
            var jsonModule = new JsonDotNetModule();
            var dataRecord = jsonModule.FromJson<DataAutoTestRecord>(jsonData);

            Game.isDebugReset = true;
            PlayerPrefs.DeleteAll();

            Services.DataNormal().SetBool(SERVICE_FIRST_TIME, false);
            Services.DataNormal().SetInt(dataRecord.abName, dataRecord.abValue);

            Services.DataSecure().SetObject(PLAYER_DATA_KEY, dataRecord.playerData);
            Services.DataSecure().SetObject(SERVICE_DATA_KEY, dataRecord.serviceData);

            Services.DataNormal().Save();
            Services.DataSecure().Save();

            // wait appium close app
        }

        public int GetCountLastAb()
        {
            if (LastAbConfig() == null) return -1;
            return LastAbConfig().ListEventsAB.Length;
        }

        public void SetLastAbValue(int abValue)
        {
            if (LastAbConfig() == null) return;
            var key = LastAbConfig().KeyName;
            Services.DataNormal().SetInt(key, abValue);
            txtAb.text = $"Set AB:{LastAbConfig().ListEventsAB[abValue]}";
        }

        private ABTestConfig LastAbConfig()
        {
            if (Services == null 
                || Services.config == null 
                || Services.config.abTestConfigs == null
                || Services.config.abTestConfigs.Length == 0) return null;

            return Services.config.abTestConfigs.Last();
        }
    }
}