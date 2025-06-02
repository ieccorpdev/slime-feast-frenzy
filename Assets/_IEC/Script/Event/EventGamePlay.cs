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
            public const string HOME = "GamePlay.HOME"; // Raise this event to show screen home

            public const string LEVEL_WIN = "GamePlay.LEVEL_WIN";
            public const string LEVEL_LOSE = "GamePlay.LEVEL_LOSE";
            public const string LEVEL_SKIP = "GamePlay.LEVEL_SKIP";

            public const string END_LEVEL_ADS = "GamePlay.END_LEVEL_ADS";
            
            public const string SCREEN_CHANGE = "GamePlay.SCREEN_CHANGE";
            public const string ON_USER_AUTHENTICATE_DONE = "GamePlay.ON_USER_AUTHENTICATE_DONE";
        }
    }
}