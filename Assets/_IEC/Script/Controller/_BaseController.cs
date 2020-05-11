using UnityEngine;

namespace IEC
{
    public class _BaseController : MonoBehaviour
    {
        protected static ServicesController Services => Controllers.Get<ServicesController>();

        protected static Game m_Game;
        protected static Game Game => m_Game ?? Pool.Get<Game>();

        protected static GamePlayGroup m_GamePlay;
        protected static GamePlayGroup GamePlay => m_GamePlay ?? Pool.Get<GamePlayGroup>();

        protected static PlayerSecret m_Player;
        protected static PlayerSecret PlayerData => m_Player ?? Pool.Get<PlayerSecret>();

        protected static UIGroup m_UIGroup;
        protected static UIGroup UI => m_UIGroup ?? Pool.Get<UIGroup>();

        protected static GameConfig m_Config;
        protected static GameConfig Config => m_Config ?? Pool.Get<GameConfig>();

    }
}