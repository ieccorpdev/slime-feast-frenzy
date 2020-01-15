using System;
using System.Collections;
using System.Collections.Generic;

namespace IEC
{
    public class RetentionModule
    {
        public const string FIRST_TIME_KEY = "Retention.FirstTime";
        public const string LAST_TIME_KEY = "Retention.LastTime";
        public const string DAY_X_KEY = "Retention.DayX";
        public const string DAY_Y_KEY = "Retention.DayY";

        IDataService m_Data;
        ITimeServer m_Time;

        DateTime m_FirstTime;
        DateTime m_LastTime;

        int m_DayX;
        int m_DayY;

        public RetentionModule(IDataService dataService, ITimeServer timeServer)
        {
            m_Data = dataService;
            m_Time = timeServer;
        }

        public void UpdateTimeNow()
        {
            var now = m_Time.GetTimeNowToLocal();
            long firstTimeTicks;
            long lastTimeTicks;
            if (m_Data.HasKey(FIRST_TIME_KEY))
            {
                firstTimeTicks = long.Parse(m_Data.GetString(FIRST_TIME_KEY, now.Ticks.ToString()));
                lastTimeTicks = long.Parse(m_Data.GetString(LAST_TIME_KEY, now.Ticks.ToString()));
            }
            else
            {
                firstTimeTicks = now.Ticks;
                lastTimeTicks = now.Ticks;
                m_Data.SetString(FIRST_TIME_KEY, firstTimeTicks.ToString());
                m_Data.SetString(LAST_TIME_KEY, lastTimeTicks.ToString());
            }

            m_FirstTime = new DateTime(firstTimeTicks);
            m_LastTime = new DateTime(lastTimeTicks);

            m_DayX = m_Data.GetInt(DAY_X_KEY, 1);
            m_DayY = m_Data.GetInt(DAY_Y_KEY, 1);

            if (m_LastTime.Date != now.Date)
            {
                m_DayX += 1;
            }
            m_Data.SetString(LAST_TIME_KEY, now.Ticks.ToString());

            var timeDuration = now - m_FirstTime;
            if (timeDuration.Days >= 0)
            {
                m_DayY = timeDuration.Days + 1;
            }

            m_Data.SetInt(DAY_X_KEY, m_DayX);
            m_Data.SetInt(DAY_Y_KEY, m_DayY);
        }

        public int GetDayX()
        {
            return m_DayX;
        }

        public int GetDayY()
        {
            return m_DayY;
        }
    }
}