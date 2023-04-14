using System;
using System.Collections.Generic;

namespace IEC
{
    /// <summary>
    /// example AB_20210705_v72
    /// date = 20210705
    /// version = 72
    /// </summary>
    [System.Serializable]
    public class ABTestDate
    {
        public ABTestConfig config;
        public int date;
        public int version;

        public bool IsApplyResult(ABTestDate curTest, int overrideDate)
        {
            if (overrideDate >= 0 && date <= overrideDate)
            {
                return true;
            }
            return date < curTest.date;
        }

        public bool IsApplyUpdate(int overrideDate)
        {
            return date >= overrideDate;
        }

        public bool IsOverrideResult(ABTestDate activeDate, Dictionary<int, Func<int, int, bool>> dictOverrideDate, out int overrideDate)
        {
            foreach (var key in dictOverrideDate.Keys)
            {
                if (date <= key && key <= activeDate.date)
                {
                    var ignoreFunc = dictOverrideDate[key];
                    if (ignoreFunc == null || ignoreFunc.Invoke(date, version) == false)
                    {
                        overrideDate = key;
                        return true;
                    }
                }
            }
            overrideDate = -1;
            return false;
        }
    }
}