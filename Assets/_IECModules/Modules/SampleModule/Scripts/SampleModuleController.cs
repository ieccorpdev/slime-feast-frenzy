using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC.SampleModule
{
    public class SampleModuleController : _BaseBehaviour, IController, IEventAwake, IEventListener
    {
        public SampleModuleDataSave DataSave { get; set; }
        public SampleModuleConfig DataConfig { get; set; }

        #region Implement
        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourAwake()
        {
            InitData();
        }


        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case SampleModuleEventList.TEST_EVENT:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Private
        private void InitData()
        {
            //Load DataSave:
            DataController Data = Controllers.Get<DataController>();
            DataSave = Data.LoadData<SampleModuleDataSave>(SampleModuleConstants.PLAYER_DATA);
            Data.SetAutoSaveObject(SampleModuleConstants.PLAYER_DATA, DataSave);
            //Load Config:
            DataConfig = Resources.Load<SampleModuleConfig>(SampleModuleConstants.CONFIG_PATH);
        }

        #endregion

        #region Public
        #endregion
    }
}