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
                    numberFrozenTime = Pool.Get<GameConfig>().gamePlayConfig.GetStartNumber(HelperId.FrozenTime),
                    numberCompass = Pool.Get<GameConfig>().gamePlayConfig.GetStartNumber(HelperId.Compass),
                    numberMagnet = Pool.Get<GameConfig>().gamePlayConfig.GetStartNumber(HelperId.Magnet),
                    numberScaleUp = Pool.Get<GameConfig>().gamePlayConfig.GetStartNumber(HelperId.ScaleUp)
                };
                PlayerData.helperData = helperData;
            }
            
            EventController.RaiseEvent(EventList.SAVE_DATA);
        }
    }
}
