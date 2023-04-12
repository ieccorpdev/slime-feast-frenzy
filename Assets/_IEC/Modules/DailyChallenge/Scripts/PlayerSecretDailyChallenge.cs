using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [System.Serializable]
    public class PlayerSecretDailyChallenge : IData
    {
        [Serializable]
        public class MonthChallenge
        {
            // key: day
            // value: state
            // 0 : not completed
            // 1 : complete step 1
            // 2 : completed
            public Dictionary<int, int> dayChallenge = new Dictionary<int, int>();
        }

        public int currentYear = -1;
        public Dictionary<int, MonthChallenge> monthChallenge = new Dictionary<int, MonthChallenge>();

        public void SetupNewMonthChallenge()
        {
            if (currentYear == DateTime.Now.Year) return;
            currentYear = DateTime.Now.Year;
            for (int i = 1; i <= 12; i++)
            {
                var month = new MonthChallenge();
                var dayInMonths = DateTime.DaysInMonth(currentYear, i);
                for (int j = 1; j <= dayInMonths; j++)
                {
                    month.dayChallenge[j] = 0;
                }

                monthChallenge[i] = month;
            }
        }

        public int GetCountCompletedOfMonth(int month)
        {
            var monthData = monthChallenge[month];
            int countMonth = 0;
            Debug.LogError(monthData.dayChallenge.Count);
            foreach (var dayChallenge in monthData.dayChallenge)
            {
                if (dayChallenge.Value == 2)
                {
                    countMonth++;
                }
            }

           

            // int CountMonth = monthData.dayChallenge.Count(x => x.Value == 2);
            return countMonth;
        }
    }
}