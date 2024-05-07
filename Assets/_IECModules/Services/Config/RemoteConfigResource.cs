using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Services/RemoteConfigResource")]
    public class RemoteConfigResource : BaseConfig
    {
        public RemoteConfigData abConfig;
        public RemoteConfigData isOverrideAb;
        public RemoteConfigData levelDifficult;
        public RemoteConfigData homeActivation;
        public RemoteConfigData shopActivation;

        private List<RemoteConfigData> m_ListData = new List<RemoteConfigData>();
        public List<RemoteConfigData> GetAllListConfigData(bool isNeedClear = false)
        {
            if (isNeedClear)
            {
                m_ListData.Clear();
            }

            if (m_ListData.Count != 0) return m_ListData;
            var listProperty = typeof(RemoteConfigResource).GetFields();
            foreach (var field in listProperty)
            {
                // Debug.LogError(field.FieldType);
                if (field.FieldType != typeof(RemoteConfigData)) continue;
                var value = (RemoteConfigData) field.GetValue(this);
                m_ListData.Add(value);
            }

            return m_ListData;
        }
        
        /*private ServicesController Services => Controllers.Get<ServicesController>();
        private RemoteConfigResource s_RemoteResource => Services.config.remoteConfigResource;
        protected IRemoteDataService RemoteData => Services.DataRemote();

        public dynamic this[string key]
        {
            get => GetAllListConfigData().First(d => d.keyName.Equals(key)).GetDefault;
        }*/
        
    }
}