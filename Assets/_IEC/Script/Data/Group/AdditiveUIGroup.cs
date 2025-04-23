using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    public class AdditiveUIGroup : _BaseBehaviour, IGroup
    {
        public List<ScreenUI> ListScreen =>
            SingleCreateIfNotExist("ListScreen", () => transform.GetComponentsInChildren<ScreenUI>(true).ToList());
        
        public void LocateGroup()
        {
            var screens = ListScreen;
            foreach (var s in screens)
            {
                s.LocateScreen();
            }
        }
    }
}