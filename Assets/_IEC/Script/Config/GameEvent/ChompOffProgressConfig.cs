using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/GameEvent/ChompOffProgressConfig")]
    public class ChompOffProgressConfig : ScriptableObject
    {
        public List<int> listProgressAfterWin = new();

        public int GetProgress(int lostCount)
        {
            if (listProgressAfterWin.Count == 0)
                return 0;
            
            if (lostCount >= listProgressAfterWin.Count)
                return 0;

            return listProgressAfterWin[lostCount];
        }
    }
}
