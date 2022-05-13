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

            private int m_Day = -1;
            public int Day
            {
                get
                {
                    if (m_Day != -1) return m_Day;
                    var convert = int.TryParse(
                        eventName.GetBetweenString($"percentile_{country.ToLower()}_d", string.Empty,false), out m_Day);
                    if (convert) return m_Day;
                    Debug.LogError("Error get day retention rev");
                    m_Day = -2;
                    return m_Day;
                }
            }

            public float Rev => Application.platform == RuntimePlatform.Android ? revAndroid : revIOS;
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
    }
}
