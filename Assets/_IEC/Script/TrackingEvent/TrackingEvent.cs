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

        private IEC.InGamePlayGroup m_DataGamePlay;
        public IEC.InGamePlayGroup DataGamePlay => m_DataGamePlay ?? Pool.Get<IEC.InGamePlayGroup>();

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
    public class ClickSpinLuckyWheel : Level
    {
        public override string Name => "click_spin";
        private string goldCost;

        public ClickSpinLuckyWheel(string c)
        {
            goldCost = c;
        }

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["gold_cost"] = goldCost;
            return dict;
        }
    }   
    public class ClickPlayHomeScreen : Level
    {
        public override string Name => "click_play_homescreen";
        private string gold_balance;

        public ClickPlayHomeScreen(string g)
        {
            gold_balance = g;
        }

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["gold_balance"] = gold_balance;
            return dict;
        }
    }
    public class BuySuccessIAP : Level
    {
        public override string Name => "buy_success";
        private string packName;

        public BuySuccessIAP(string namePack)
        {
            packName = namePack;
        }

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["pack_name"] = packName;
            return dict;
        }
    }    
    public class UserHelper : Level
    {
        public override string Name => "use_helper";
        private string helperType;
        private string costType;

        public UserHelper(string hType,string cType)
        {
            helperType = hType;
            costType = cType;
        }

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["helper_type"] = helperType;
            dict["cost_type"] = costType;
            return dict;
        }
    }

    public abstract class Level : Game
    {
        public override string Name => "LEVEL";

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["level"] = DataPlayer.levelTracking;
            dict["level_prop"] = DataGame.crLevelType.ToString();
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
    public class PlayLevelDetail : Level
    {
        public override string Name => "play_level_detail";

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["time"] = DataGame.TimeFinishLevel;
            dict["result"] = DataGamePlay;
            return dict;
        }
    }

    public class GameOver : ResultLevel
    {
        public override string Name => "game_over";

        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["result"] = DataGamePlay;
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
    public class SFApostition : Level
    {
        public string position = "";
        public override string Name => "sfa_position";
        public override Dictionary<string, object> GetParams()
        {
            var param = base.GetParams();
            param["position"] = position;
            return param;
        }
        public SFApostition(string position)
        {
            this.position = position;
        }
    }
    public class SRApostition : Level
    {
        public string position = "";
        public override string Name => "sra_position";
        public override Dictionary<string, object> GetParams()
        {
            var param = base.GetParams();
            param["position"] = position;
            return param;
        }
        public SRApostition(string position)
        {
            this.position = position;
        }
    }
    public class RatingClick : Level
    {
        public override Dictionary<string, object> GetParams()
        {
            var dict = base.GetParams();
            dict["level_show"] = DataPlayer.level;
            return dict;
        }
    }
    
    public class RatingClickClose : RatingClick
    {
        public override string Name => "click_rating_btn_close";
    }

    public class RatingClickLoveIt : RatingClick
    {
        public override string Name => "click_rating_btn_love_it";
    }

    public class RatingClickNoThank : RatingClick
    {
        public override string Name => "click_rating_btn_no_thank";
    }

}