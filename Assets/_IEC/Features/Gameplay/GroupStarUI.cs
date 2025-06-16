using System.Collections.Generic;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class GroupStarUI : _BaseBehaviour
    {
        [SerializeField] private List<StarUI> stars;

        public void SetNumberStar(int newNumber)
        {
            DeActiveAllStar();
            if (newNumber > stars.Count)
            {
                newNumber = stars.Count;
            }

            for (int i = 0; i < newNumber; i++)
            {
                stars[i].SetActiveStar(true);
            }
        }

        private void DeActiveAllStar()
        {
            foreach (var starUI in stars)
            {
                starUI.SetActiveStar(false);
            }
        }
    }
}