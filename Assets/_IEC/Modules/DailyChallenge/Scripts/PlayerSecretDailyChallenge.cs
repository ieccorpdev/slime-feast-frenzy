using System;
using System.Collections.Generic;
using System.Linq;

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
            public Dictionary<int, int> days = new Dictionary<int, int>();
        }

        public int currentYear = -1;
        public Dictionary<int, MonthChallenge> months = new Dictionary<int, MonthChallenge>();

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
                    month.days[j] = 0;
                }

                months[i] = month;
            }
        }

        public int GetCountCompletedOfMonth(int month)
        {
            var monthData = months[month];
            return monthData.days.Count(kv => kv.Value == 2);
        }
    }
}