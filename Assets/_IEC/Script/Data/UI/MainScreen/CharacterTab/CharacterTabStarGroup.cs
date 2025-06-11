using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class CharacterTabStarGroup : _BaseBehaviour
    {
        public List<CharacterTabStarUI> listStarUI = new();

        public void UpdateStarUI(int level)
        {
            for (int i = listStarUI.Count - 1; i >= 0; i--)
            {
                listStarUI[i].SetStarActive(i <= level);
            }
        }
    }
}
