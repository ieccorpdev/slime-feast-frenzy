using System;
using System.Collections;
using System.Collections.Generic;

namespace IEC
{
    public class RetentionModule
    {
        public const string FIRST_TIME_KEY = "Retention.FirstTime";
        public const string LAST_TIME_KEY = "Retention.LastTime";
        public const string COUNT_DATE_KEY = "Retention.CountDate";
        public const string COUNT_DAY_KEY = "Retention.CountDay";

        IDataService m_Data;
        ITimeServer m_Time;

        DateTime m_FirstTime;
        DateTime m_LastTime;

        int m_CountDate;
        int m_CountDay;

        public RetentionModule(IDataService dataService, ITimeServer timeServer)
        {
            m_Data = dataService;
            m_Time = timeServer;
            m_CountDate = 1;
            m_CountDay = 1;
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

            m_CountDate = m_Data.GetInt(COUNT_DATE_KEY, 1);
            m_CountDay = m_Data.GetInt(COUNT_DAY_KEY, 1);

            if (m_LastTime.Date != now.Date)
            {
                m_CountDate += 1;
            }
            m_Data.SetString(LAST_TIME_KEY, now.Ticks.ToString());

            var timeDuration = now - m_FirstTime;
            if (timeDuration.Days >= 0)
            {
                m_CountDay = timeDuration.Days + 1;
            }

            m_Data.SetInt(COUNT_DATE_KEY, m_CountDate);
            m_Data.SetInt(COUNT_DAY_KEY, m_CountDay);
        }

        public int GetCountDate()
        {
            return m_CountDate;
        }

        public int GetCountDay()
        {
            return m_CountDay;
        }
    }
}