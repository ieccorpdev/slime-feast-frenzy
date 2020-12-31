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

    public abstract class Level : Game
    {
        public override string Name => "LEVEL";

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["level"] = DataPlayer.level;
            return dict;
        }
    }

    public abstract class LevelResult : Level
    {
        public override string Name => "LEVEL_RESULT";

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["time"] = DataPlayer.countPlayThisLevel;
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

    public class ReplayLevel : Level
    {
        public override string Name => "click_btn_replay";
    }

    public class SkipLevel : Level
    {
        public override string Name => "click_btn_skip";
    }

    public class HintLevel : Level
    {
        public override string Name => "click_btn_hint";
    }

    public class UndoLevel : Level
    {
        public override string Name => "click_btn_undo";
    }

    public class ShowRewardUndo : Level
    {
        public override string Name => "click_btn_ads_undo";
    }

    public class ShowRewardAddTime : Level
    {
        public override string Name => "click_btn_ads_add_time";
    }

    public class ShowPopupRating : Level
    {
        public override string Name => "popup_rating_show";
    }

    public class PopupRatingNoThank : Level
    {
        public override string Name => "popup_rating_click_no_thank";
    }

    public class PopupRatingLoveIt : Level
    {
        public override string Name => "popup_rating_click_love_it";
    }

    public class GameOver : LevelResult
    {
        public override string Name => "game_over";

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["time_play"] = DataGame.timePlay;
            dict["moves"] = DataGame.moveCount;
            return dict;
        }
    }

}