using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC.TrackingEvent
{
    public abstract class Game : AnalyticEvent
    {
        private IEC.Game m_DataGame;
        public IEC.Game DataGame => m_DataGame ?? Pool.Get<IEC.Game>();

        private IEC.PlayerSecret m_DataPlayer;
        public IEC.PlayerSecret DataPlayer => m_DataPlayer ?? Pool.Get<IEC.PlayerSecret>();

        private IEC.GamePlayGroup m_DataGamePlay;
        public IEC.GamePlayGroup DataGamePlay => m_DataGamePlay ?? Pool.Get<IEC.GamePlayGroup>();

        private IEC.GameConfig m_DataConfig;
        public IEC.GameConfig DataConfig => m_DataConfig ?? Pool.Get<IEC.GameConfig>();

        public override string Name => "GAME";
    }

    public class OpenCheat : AnalyticEvent
    {
        public override string Name => "open_cheat";
    }

    public class UnlockCheat : AnalyticEvent
    {
        public override string Name => "unlock_cheat";
    }

    public abstract class Level : Game
    {
        public override string Name => "LEVEL";

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["level"] = DataPlayer.levelTracking;
            return dict;
        }
    }

    public class PlayLevel : Level
    {
        public override string Name => "play_level";
    }

    public class PlayLevelFirstTime : Level
    {
        public override string Name => "play_level_first_time";
    }

    public class ResultLevel : Level
    {
        public override string Name => "RESULT_LEVEL";

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["time"] = DataGame.TimeFinishLevel;
            dict["session_count"] = DataPlayer.LevelSessionCount;
            return dict;
        }
    }

    public class GameOver : ResultLevel
    {
        public override string Name => "game_over";

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["result"] = DataGamePlay.IsWin;
            return dict;
        }
    }

    public class ClickBtnReplay : Level
    {
        public override string Name => "click_btn_replay";
    }

    public class ClickBtnRetry : Level
    {
        public override string Name => "click_btn_retry";
    }
    
    public class ClickBtnNext : Level
    {
        public override string Name => "click_btn_next";
    }
    public class ClickBtnTracking : Level
    {
        public override string Name => "click_btn";
        private string btn_name;

        public ClickBtnTracking(string position)
        {
            btn_name = position;
        }

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["btn_name"] = btn_name;
            return dict;
        }
    }
}