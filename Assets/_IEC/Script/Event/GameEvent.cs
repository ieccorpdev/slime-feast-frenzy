using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public enum GameEvent
    {
        NONE,
        GAME_INIT,
        GAME_START,
        GAME_LOSE,
        GAME_WIN,
        GAME_RESTART,
        GAME_PAUSE,
        GAME_RESUME,
        PLAY_COLLISION,
        ADD_KILL,
        ADD_KILL_UI,
        ADD_SIZE,
        PLAY_SKILL,
        EFFECT_PUSH_AFTER,
        BUTTON_UI_SKILL_DASH,
        BUTTON_START_MAINMENU,
        BUTTON_NO_ADS,
        BUTTON_SETTING,
        BUTTON_MENU_SKIN,
        BUTTON_PAUSE,
        BUTTON_BACK,
        BUTTON_MAIN_MENU,
        BUTTON_SKIN,
        PLAYER_DEAD,
        CHANGE_SKIN_CHARATER_MAIN,
        HIDE_LOGO_AFTER_LOADED_AD,
        SHOW_LOGO_AD_LOAD_FAIL,
        SHOW_GDPR,
        CLOSE_GDPR,
        LOG_SHOW_FULL_AD,
        ADS_GAME_RESTART,
        BUTTON_SOUND,
        BUTTON_MUSIC,
        BUTTON_PRESS,
        SOUND_COLLISION,
        SOUND_COUNT_DOWN
    }

    public enum PlayerSkill
    {
        DASH,
        TRAP
    }

}
