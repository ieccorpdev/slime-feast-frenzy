using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    public class AdditiveUIGroup : MonoBehaviour, IGroup
    {
        public List<ScreenUI> ListScreen => new List<ScreenUI>();
            
        
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