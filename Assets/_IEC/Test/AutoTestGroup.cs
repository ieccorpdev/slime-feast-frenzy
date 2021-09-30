using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace IEC
{
    public class AutoTestGroup : _BaseBehaviour, IGroup
    {
        public AltUnityRunner AltUnity => SingleGetChild<AltUnityRunner>(true);

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

        }

        [Button]
        public void SetReady()
        {
            var readyFlag = transform.GetChild(0).gameObject;
            readyFlag.SetActive(true);
        }

        [Button]
        public void StartAltUnity()
        {
            AltUnity.gameObject.SetActive(true);
        }

        [Button]
        public void StartRecordAction()
        {
            
        }

        [Button]
        public void StopRecordAction()
        {
        }
    }
}