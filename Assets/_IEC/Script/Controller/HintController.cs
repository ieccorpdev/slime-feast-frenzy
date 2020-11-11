using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            switch (gameObject)
            {

            }
        }

        private void CheckHint()
        {

        }
    }
}