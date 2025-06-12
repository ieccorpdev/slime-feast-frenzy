using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class CharacterTabStarUI : _BaseBehaviour
    {
        public GameObject activeStarObject;

        public void SetStarActive(bool active)
        {
            activeStarObject.SetActive(active);
        }
    }
}
