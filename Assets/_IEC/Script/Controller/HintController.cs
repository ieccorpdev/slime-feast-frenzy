using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace IEC
{
    public class HintController : _BaseBehaviour, IController, IEventAwake, IEventListener
    {
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
            
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {

            }
        }

        public BaseComponent target;

        [Button]
        private void FindLine()
        {
            target = Game.listTarget[0];
            var listGridNear = target.GetGridNearCanConnect(target.ID, 0);

            for (int i = 0; i < listGridNear.Count; i++)
            {
                Debug.LogError("Name: " + listGridNear[i].name);
            }
        }

    }
}