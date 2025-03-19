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

            public const string LEVEL_WIN = "GamePlay.LEVEL_WIN";
            public const string LEVEL_LOSE = "GamePlay.LEVEL_LOSE";
            public const string LEVEL_SKIP = "GamePlay.LEVEL_SKIP";

            public const string END_LEVEL_ADS = "GamePlay.END_LEVEL_ADS";
            public const string GO_MAIN = "GamePlay.GO_HOME";
            public const string SETUP_NEW_SCENE = "GamePlay.SETUP_NEW_SCENE";
            public const string SHOW_LOADING = "GamePlay.SHOW_LOADING";
            public const string ON_CHOOSE_TAB = "GamePlay.ON_CHOOSE_TAB";
            public const string UPDATE_RESOURCE_UI = "GamePlay.UPDATE_RESOURCE_UI";
            public const string BUY_NO_ADS = "GamePlay.BUY_NO_ADS";
            
            public const string SHOW_REWARD_INFO_LEADERBOARD = "GamePlay.SHOW_REWARD_INFO_LEADERBOARD";

            public const string TUTORIAL = "GamePlay.TUTORIAL";
            public const string TUTORIAL_DONE = "GamePlay.TUTORIAL_DONE";

            public const string SETTING_MUSIC = "GamePlay.SETTING_MUSIC";
            public const string SETTING_SOUND = "GamePlay.SETTING_SOUND";
            public const string SETTING_VIBRATES = "GamePlay.SETTING_VIBRATES";

            public const string SET_MUTE_BACKGROUND_MUSIC = "GamePlay.SET_MUTE_BACKGROUND_MUSIC";
            public const string SET_UNMUTE_BACKGROUND_MUSIC_INGAME = "GamePlay.SET_UNMUTE_BACKGROUND_MUSIC_INGAME";
            public const string SET_UNMUTE_BACKGROUND_MUSIC_HOME = "GamePlay.SET_UNMUTE_BACKGROUND_MUSIC_HOME";
            
            public const string UPDATE_HELPER_UI = "GamePlay.UPDATE_HELPER_UI";
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