using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _IECModules
{
    [CreateAssetMenu(menuName = "Services/PercentileEventConfig")]
    public class PercentileEventConfig : ScriptableObject
    {
        [Serializable]
        public class Param
        {
            public string eventName;
            public string country;
            public float revAndroid;
            public float revIOS;
        }

        public string url;
        public List<Param> listParam = new List<Param>();

        [NaughtyAttributes.Button]
        public void LoadData()
        {
            listParam.Clear();
            GoogleSheetUtility.DownLoadCsvEditor(url,true, list =>
            {
                foreach (var row in list)
                {
                    var param = new Param
                    {
                        eventName = row[0],
                        country = row[1]
                    };
                    float.TryParse(row[2], out param.revAndroid);
                    float.TryParse(row[3], out param.revIOS);
                    listParam.Add(param);
                }
            }, Debug.LogError);
        }

        public List<string> GetListEventPush(int day, float totalRev)
        {
            return listParam.Where(x =>
            {
                var eventDayStr =
                    x.eventName.GetBetweenString($"_percentile_{x.country.ToLower()}_d", string.Empty, false);
                int.TryParse(eventDayStr, out var eventDay);
                var isEnoughRev =
                    totalRev >= ((Application.platform == RuntimePlatform.Android) ? x.revAndroid : x.revIOS);
                return eventDay == day && isEnoughRev;
            }).Select(x => x.eventName).ToList();
        }
    }
}
