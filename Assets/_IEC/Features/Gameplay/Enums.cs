using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public enum ItemType
    {
        Item1,
        Item2,
        Item3
    }

    public enum FallingItemSetType
    {
        ASIAN_FOOD = 0,
        CAKES = 1,
        FRUITS = 2,
        TOYS = 3,
        VEGETABLES = 4,
        SCHOOL_SUPPLIES = 5,
        DRINKS = 6,
        FASTFOOD = 7,
        COIN = 100,
        OTHER_ITEMS = 101,
        ALL = 102
    }
    
    public enum PhysicFallingItemType
    {
        None = 0,
        Table = 1,
        Cube = 2,
        Column = 3,
        Box = 4,
        Arch = 5,
        Spline = 6
    }
    public enum SpecialItemID
    {
        SoccerBall = 0,
        Chair = 1,
        Cup = 2,
        Easel = 3,
        Flashlight = 4,
        Guitar = 5,
        Kettle = 6,
        Stove = 7,
        Typewriter = 8,
        Skateboard = 9,
        ScubaMask = 10,
        RollingPin = 11,
        FryingPan = 12,
        Lollipop = 13,
        Shovel = 14,
        Boosters = 15
    }

    public enum HelperId
    {
        None,
        Magnet,
        FrozenTime,
        ScaleUp,
        Compass
    }

    public enum LevelType
    {
        NONE,
        NORMAL,
        NEW_NORMAL,
        HARD,
        COIN
    }
}
