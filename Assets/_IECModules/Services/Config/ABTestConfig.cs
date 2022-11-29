using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

namespace IEC
{
    [CreateAssetMenu(fileName = "ABTestConfig", menuName = "Services/ABTestConfig")]
    public class ABTestConfig : ScriptableObject
    {
        public const string RESULT_PREFIX = "ResultAb";
        public const string UPDATE_PREFIX = "UpdateAb";

        public string KeyName;
        public bool IsActive;
        public bool IsOnlyNewInstall;
        public bool IsOverride;
        public string[] ListEventsAB;
        public int[] ListRandomRatio;
        public int DefaultValue;
        public bool IsForceValue;
        public int ForceValue;

        [Header("Custom setup")]
        public string platform;
        public string customResultAction;
        public string customUpdateAction;

        public int Date => int.Parse(KeyName.Split('_').Last());

        [Button]
        public void SetDefaultRatioAndAB()
        {
            ListRandomRatio = new int[ListEventsAB.Length];
            for (int i = 0; i < ListRandomRatio.Length; i++)
            {
                ListRandomRatio[i] = 1;
            }

            var firstEventAB = ListEventsAB.First();
            var version = int.Parse(firstEventAB.Split(new[] { "_v" }, StringSplitOptions.None)[1]);

            for (var i = 1; i < ListEventsAB.Length; i++)
            {
                ListEventsAB[i] = $"{KeyName}_v{version + i}";
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public string GetLastAbName()
        {
            return ListEventsAB.LastOrDefault();
        }

        public bool IsPlatform(string platform)
        {
            if (string.IsNullOrWhiteSpace(platform)) return true;
            if (string.IsNullOrWhiteSpace(this.platform)) return true;
            if (this.platform.ToLower() == platform.ToLower()) return true;
            return false;
        }

        public string GetAbTestAction(string prefix)
        {
            if (prefix == UPDATE_PREFIX)
            {
                return GetUpdateAbTestAction();
            }
            return GetResultAbTestAction();
        }

        public string GetResultAbTestAction()
        {
            if (string.IsNullOrWhiteSpace(customUpdateAction))
            {
                return RESULT_PREFIX + Date;
            }
            return customUpdateAction;
        }

        public string GetUpdateAbTestAction()
        {
            if (string.IsNullOrWhiteSpace(customUpdateAction))
            {
                return UPDATE_PREFIX + Date;
            }
            return customUpdateAction;
        }

        public int GetVersion(int abValue)
        {
            return int.Parse(ListEventsAB[abValue].Split('v').Last());
        }

        public ABTestDate GetAbTestDate(int abValue)
        {
            return new ABTestDate()
            {
                config = this,
                date = Date,
                version = GetVersion(abValue)
            };
        }
    }
}