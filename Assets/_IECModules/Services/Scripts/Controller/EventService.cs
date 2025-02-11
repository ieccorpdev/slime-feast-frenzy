namespace IEC
{
    public static partial class EventList
    {
        public static class Service
        {
            public const string LOAD_SCENE = "Service.LOAD_SCENE";
            public const string INIT_DONE = "Service.INIT_DONE";
            public const string LOGIN_DONE = "Service.LOGIN_DONE";
            public const string FIREBASE_LOADED = "Service.FIREBASE_LOADED";
            public const string FACEBOOK_LOADED = "Service.FACEBOOK_LOADED";
            public const string INIT_AD_DONE = "Service.INIT_AD_DONE";
            public const string INIT_IAP_DONE = "Service.INIT_IAP_DONE";
            public const string COUNT_ONE_SEC = "Service.COUNT_ONE_SEC";

            public const string REMOTE_CONFIG_LOADED = "Service.REMOTE_CONFIG_LOADED";
            public const string UPDATE_TIME_SERVER = "Service.UPDATE_TIME_SERVER";

            public const string SHOW_INTER_DONE = "Service.SHOW_INTER_DONE";
            public const string SHOW_REWARD = "Service.SHOW_REWARD";
            public const string PUSH_SHOW_INTER = "Service.PUSH_SHOW_INTER";
            public const string RESTORE_PURCHASE = "Service.RESTORE_PURCHASE";

            public const string ON_ADD_ANALYTIC_SAVE_DATA = "Service.ON_ADD_ANALYTIC_SAVE_DATA";
        }

        public static class Setting
        {
            public const string SETTING_MUSIC = "Setting.SETTING_MUSIC";
            public const string SETTING_SOUND = "Setting.SETTING_SOUND";
            public const string SETTING_VIBRATES = "Setting.SETTING_VIBRATES";
        }

        public static class Leaderboard
        {
            public const string SHOW_LEADERBOARD = "Leaderboard.SHOW_LEADERBOARD";
            public const string LOAD_DATA_FROM_SERVER = "Leaderboard.LOAD_DATA_FROM_SERVER";
            public const string PUSH_PLAYER_DATA = "Leaderboard.PUSH_PLAYER_DATA";
        }
    }
}