namespace IEC
{
    public enum GridType
    {
        NONE,
        WALL,
        GRID_SOURCE,
        GRID_TARGET,
        GRID_STRAIGHT,
        GRID_CROSS,
        GRID_CURVE
    }

    public enum WaterPoint
    {
        TOP,
        RIGHT,
        BOT,
        LEFT,
        CENTER
    }

    public enum GameState
    {
        Init,
        Pause,
        Playing,
        GameOver
    }

    public enum HolePos
    {
        None,
        TopLeft,
        TopRight,
        BotLeft,
        BotRight
    }

    public enum IapPackages
    {
        None,
        NoAds
    }
}
