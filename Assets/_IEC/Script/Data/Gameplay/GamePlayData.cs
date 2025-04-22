using System;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IEC
{
    public class GamePlayData : _BaseBehaviour
    {
        private bool _initialized;
        
        #region Properties
        
        private const int MAX_LIVES_TO_PLAY = 5;
        private const long LIVES_COOLDOWN = 30 * TimeSpan.TicksPerMinute;
    
        #endregion


        
#region Achievements

        private List<LevelAchievementData> m_AchievementData = new List<LevelAchievementData>();

        public void ClearAchievements()
        {
            m_AchievementData.Clear();
        }

        public void AddAchievementItem(string id, int number)
        {
            var item = m_AchievementData.FirstOrDefault(x => x.id == id);
            if (item != null)
            {
                item.number += number;
            }
            else
            {
                item = new LevelAchievementData();
                item.id = id;
                item.number = number;
                m_AchievementData.Add(item);
            }
        }

        public int GetAchievementItemCount(string id)
        {
            var item = m_AchievementData.FirstOrDefault(x => x.id == id);
            if (item != null)
            {
                return item.number;
            }

            return 0;
        }

#endregion
    }
}