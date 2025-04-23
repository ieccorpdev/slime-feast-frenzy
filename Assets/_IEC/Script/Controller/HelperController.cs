using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Booster;
using _IEC.Features.Gameplay;
using UnityEngine;

namespace IEC
{
    public class HelperController : _BaseBehaviour, IController, IEventStart
    {
        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeInHierarchy;
        }

        public void BehaviourStart()
        {
            InitData();
        }
        
        void InitData()
        {
            var helperData = PlayerData.helperData;

            if (helperData == null)
            {
                helperData = new PlayerHelperData
                {
                    numberFrozenTime = PlayableController.Instance.gameConfig.gamePlayConfig.GetStartNumber(HelperId.FrozenTime),
                    numberCompass = PlayableController.Instance.gameConfig.gamePlayConfig.GetStartNumber(HelperId.Compass),
                    numberMagnet = PlayableController.Instance.gameConfig.gamePlayConfig.GetStartNumber(HelperId.Magnet),
                    numberScaleUp = PlayableController.Instance.gameConfig.gamePlayConfig.GetStartNumber(HelperId.ScaleUp)
                };
                PlayerData.helperData = helperData;
            }
            
            EventController.RaiseEvent(EventList.SAVE_DATA);
        }
    }
}
