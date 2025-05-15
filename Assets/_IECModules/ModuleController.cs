using UnityEngine;

namespace _IECModules
{
    public class ModuleController : MonoBehaviour
    {
        public const string LOG_ENABLED_KEY = "unityLogger.logEnabled";

        public static ModuleController instance;

        public static CheatCode cheatCode;
        
        public static GoogleSheetToCSV googleSheetToCsv;

        public static NotificationBehaviour notificationBehaviour;

        private void Awake()
        {
            if (instance)
            {
                // Prevent Create Multiple Instance On Load Scene
                DestroyImmediate(gameObject, true);
                return;
            }

            instance = this;

            DontDestroyOnLoad(this);

            cheatCode = GetComponentInChildren<CheatCode>(true);
            
            googleSheetToCsv = GetComponentInChildren<GoogleSheetToCSV>(true);

            notificationBehaviour = GetComponentInChildren<NotificationBehaviour>(true);

            InitConstParam();
        }

        private void InitConstParam()
        {
            Application.targetFrameRate = 60;

            Debug.unityLogger.filterLogType = ((PlayerPrefs.HasKey(LOG_ENABLED_KEY) &&
                                            PlayerPrefs.GetInt(LOG_ENABLED_KEY) == 1)
                                           || Application.isEditor) ? LogType.Log : LogType.Warning;
        }
    }
}