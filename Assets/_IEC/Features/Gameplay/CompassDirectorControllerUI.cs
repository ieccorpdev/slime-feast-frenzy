using System;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class CompassDirectorControllerUI : _BaseBehaviour
    {
        private readonly List<BaseItemDirectorUI> m_ListDirectorUI = new List<BaseItemDirectorUI>();
        private float m_CoolDownTime = 0;
        private bool m_IsPausing;
        private string m_CurrentID;
        private int MaxNumberDirectorUI => Config.gamePlayConfig.maxNumberDirectorUI;
        private void Awake()
        {
            this.RegisterListener(EventList.GamePlay.TURN_ON_COMPASS_DIRECTOR,OnTurnOnCompassDirector);
            this.RegisterListener(EventList.GamePlay.PAUSE_GAME,OnPauseGame);
            this.RegisterListener(EventList.GamePlay.RESUME_GAME,OnResumeGame);
        }

        private void OnDestroy()
        {
            this.UnregisterListener(EventList.GamePlay.TURN_ON_COMPASS_DIRECTOR,OnTurnOnCompassDirector);
            this.UnregisterListener(EventList.GamePlay.PAUSE_GAME,OnPauseGame);
            this.UnregisterListener(EventList.GamePlay.RESUME_GAME,OnResumeGame);
        }

        private void OnPauseGame(object param)
        {
            m_IsPausing = true;
        }

        private void OnResumeGame(object param)
        {
            m_IsPausing = false;
        }
        private void Update()
        {
            if (m_IsPausing) return;
            UpdateAllDirectorUI();
            m_CoolDownTime -= Time.deltaTime;
            if (m_CoolDownTime <=0)
            {
                TurnOff();
            }
        }

        private void UpdateAllDirectorUI()
        {
            int count = 0;
            foreach (var directorUI in m_ListDirectorUI)
            {
                if (directorUI.IsNeededDirector() && count < Config.gamePlayConfig.maxNumberDirectorUI)
                {
                    count++;
                    if(directorUI.gameObject.activeInHierarchy == false)
                        directorUI.gameObject.SetActive(true);
                    directorUI.UpdateDirectToBaseItem();
                }
                else
                {
                    directorUI.gameObject.SetActive(false);
                }
            }
        }

        private void OnTurnOnCompassDirector(object param)
        {
            var data = (TurnOnCompassDirector) param;
            LoadData(data.remainingObjectiveItems,data.holeTransform);
            TurnOn();
        }
        

        private void LoadData(List<BaseItem> baseItems, Transform holeTransform)
        {
            foreach (var baseItem in baseItems)
            {
                if(baseItem == null) continue;
                var director = Config.prefabConfig.itemDirectorUI.Spawn(transform);
                director.Reset();
                director.LoadData(holeTransform,baseItem);
                m_ListDirectorUI.Add(director);
            }
        }

        private void TurnOn()
        {
            m_CoolDownTime = Config.gamePlayConfig.GetHelperActiveTime(HelperId.Compass);
            //gameObject.SetActive(true);
        }
        
        public void TurnOff()
        {
            foreach (var directorUI in m_ListDirectorUI)
            {
                directorUI.gameObject.Recycle();
            }
            m_ListDirectorUI.Clear();
            //gameObject.SetActive(false);
        }
    }
}