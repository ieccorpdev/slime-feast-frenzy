using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC.TrackingEvent
{
    public class ClickBtnPlaySaga : Level
    {
        public override string Name => "click_btn_play_saga";
        public bool isBonus;
        public int level;
        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["level_actual"] = level;
            dict["bonus"] = isBonus;
            return dict;
        }

        public ClickBtnPlaySaga(int _level, bool _isBonus)
        {
            isBonus = _isBonus;
            level = _level;
        }
    }
}