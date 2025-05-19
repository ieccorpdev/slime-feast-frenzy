using JetBrains.Annotations;

namespace IEC
{
    public static partial class EventList
    {
        public static class GamePlay
        {
            public const string APPLY_AB_TEST = "GamePlay.APPLY_AB_TEST";

            public const string SPAWN_ALLY = "GamePlay.SPAWN_ALLY";
            public const string SPAWN_ENEMY = "GamePlay.SPAWN_ENEMY";

            public const string NEW_GAME = "GamePlay.NEW_GAME";
            public const string NEW_GAME_HARD_LEVEL = "GamePlay.NEW_GAME_HARD_LEVEL";
            public const string NEW_GAME_LEVEL_ALL_ITEM = "GamePlay.NEW_GAME_LEVEL_ALL_ITEM";

            public const string LEVEL_WIN = "GamePlay.LEVEL_WIN";
            public const string LEVEL_LOSE = "GamePlay.LEVEL_LOSE";
            public const string LEVEL_SKIP = "GamePlay.LEVEL_SKIP";
            public const string LEVEL_CONTINUE = "GamePlay.LEVEL_CONTINUE";
            public const string LEVEL_CLEAR = "GamePlay.LEVEL_CLEAR";

            public const string END_LEVEL_ADS = "GamePlay.END_LEVEL_ADS";
            public const string GO_MAIN = "GamePlay.GO_HOME";
            public const string SETUP_NEW_SCENE = "GamePlay.SETUP_NEW_SCENE";
            public const string SHOW_LOADING = "GamePlay.SHOW_LOADING";
            public const string ON_CHOOSE_TAB = "GamePlay.ON_CHOOSE_TAB";
            public const string UPDATE_RESOURCE_UI = "GamePlay.UPDATE_RESOURCE_UI";
            public const string UPDATE_GAME_EVENT_UI = "GamePlay.UPDATE_GAME_EVENT_UI";
            public const string UPDATE_RESOURCE_BOOSTER_UI = "GamePlay.UPDATE_RESOURCE_BOOSTER_UI";
            public const string UPDATE_PLAYER_AVATAR_PROFILE = "GamePlay.UPDATE_PLAYER_PROFILE";
            public const string UPDATE_PLAYER_NAME = "GamePlay.UPDATE_PLAYER_PROFILE";
            public const string BUY_NO_ADS = "GamePlay.BUY_NO_ADS";
            public const string BUY_STARTER_PACK = "GamePlay.BUY_STARTER_PACK";
            
            public const string SHOW_REWARD_INFO_LEADERBOARD = "GamePlay.SHOW_REWARD_INFO_LEADERBOARD";
            public const string SHOW_REWARD_INFO_GAME_EVENT = "GamePlay.SHOW_REWARD_INFO_GAME_EVENT";

            public const string TUTORIAL = "GamePlay.TUTORIAL";
            public const string TUTORIAL_DONE = "GamePlay.TUTORIAL_DONE";

            public const string SETTING_MUSIC = "GamePlay.SETTING_MUSIC";
            public const string SETTING_SOUND = "GamePlay.SETTING_SOUND";
            public const string SETTING_VIBRATES = "GamePlay.SETTING_VIBRATES";

            public const string SET_MUTE_BACKGROUND_MUSIC = "GamePlay.SET_MUTE_BACKGROUND_MUSIC";
            public const string SET_UNMUTE_BACKGROUND_MUSIC_INGAME = "GamePlay.SET_UNMUTE_BACKGROUND_MUSIC_INGAME";
            public const string SET_UNMUTE_BACKGROUND_MUSIC_HOME = "GamePlay.SET_UNMUTE_BACKGROUND_MUSIC_HOME";
            
            public const string UPDATE_HELPER_UI = "GamePlay.UPDATE_HELPER_UI";
            //Swallowhole
            public const string GOLD_CHANGE = "GamePlay.GOLD_CHANGE";
            public const string LIVES_TO_PLAY_CHANGE = "GamePlay.LIVES_TO_PLAY_CHANGE";
            public const string COLLECT_ITEM = "GamePlay.COLLECT_ITEM";
            public const string COLLECT_OBJECTIVE_ITEM_NEEDED_FOR_UI = "GamePlay.COLLECT_OBJECTIVE_ITEM_NEEDED_FOR_UI";
            public const string COLLECT_SPECIAL_ITEM_FOR_UI = "Gameplay.COLLECT_SPECIAL_ITEM_FOR_UI";

            public const string UPDATE_CAMERA_TARGET = "Gameplay.UPDATE_CAMERA_TARGET";
            public const string TIMER_TIME_OUT = "Gameplay.TIMER_TIME_OUT";
            public const string USER_USE_HELPER = "Gameplay.USER_USE_HELPER";
            public const string TURN_ON_COMPASS_DIRECTOR = "Gameplay.TURN_ON_COMPASS_DIRECTOR";

            public const string PAUSE_GAME = "Gameplay.PAUSE_GAME";
            public const string RESUME_GAME = "Gameplay.RESUME_GAME";

            public const string UPDATE_CURRENCY_UI = "Gameplay.UPDATE_CURRENCY_UI";
            public const string ADD_TIME_FOR_TIMER = "Gameplay.ADD_TIME_FOR_TIMER";
            public const string REVIVE_GAME = "Gameplay.REVIVE_GAME";
            
            public const string COMPLETE_HELPER = "Gameplay.COMPLETE_FROZEN_TIME_HELPER";

            public const string IAP_PRODUCT_REQUEST = "GamePlay.IAP_PRODUCT_REQUEST";
            public const string IAP_PURCHASE_SUCCESS = "Gameplay.IAP_PURCHASE_SUCCESS";
            public const string IAP_PURCHASE_FAIL = "Gameplay.IAP_PURCHASE_FAIL";

            public const string START_TUTORIAL = "Gameplay.START_TUTORIAL";
            public const string HELPER_TUTORIAL = "Gameplay.HELPER_TUTORIAL";

            public const string ON_SOUND_SETTINGS = "Gameplay.ON_SOUND_SETTINGS";
            
            public const string USE_BOOSTER = "Gameplay.USE_BOOSTER";
            public const string UPDATE_BOOSTER_UI = "Gameplay.UPDATE_BOOSTER_UI";

            public const string ON_BOOSTER_COLLECTED = "Gameplay.ON_BOOSTER_COLLECTED";
            public const string SHOW_ANIM_SPAWN_BOOSTER = "Gameplay.SHOW_ANIM_SPAWN_BOOSTER";
            public const string SPAWN_BOOSTER_ITEM = "Gameplay.SPAWN_BOOSTER_ITEM";

            public const string CHEAT_SPECIAL_OBJECT = "Gameplay.CHEAT_SPECIAL_OBJECT";
         

            public const string ON_LEVEL_LOADED = "Gameplay.ON_LEVEL_LOADED";
            public const string Kingdom_Special = "Kingdom_Special";
            public const string ON_LEVEL_DEBUG_LOADED = "Gameplay.ON_LEVEL_DEBUG_LOADED";
            
            public const string SHOW_FULL_SCREEN_BY_MEDIATION_ADS = "Gameplay.SHOW_FULL_SCREEN_BY_MEDIATION_ADS";
            public const string HIDE_FULL_SCREEN_BY_MEDIATION_ADS = "Gameplay.HIDE_FULL_SCREEN_BY_MEDIATION_ADS";
        }
        public static class Tutorial
        {
            public const string LEVEL_1_FIRST_CUBE = "Tutorial.LEVEL_1_FIRST_CUBE";
            public const string LEVEL_1_SECOND_CUBE = "Tutorial.LEVEL_2_SECOND_CUBE";
            public const string LEVEL_2_CLEAR_HELPER = "Tutorial.LEVEL_2_CLEAR_HELPER";
            public const string LEVEL_4_STACK_END = "Tutorial.LEVEL_4_STACK_END";
        }
    }
}