using UnityEngine;

namespace IEC
{
    public class _BaseController : MonoBehaviour
    {
        protected ServicesController Services => Controllers.Get<ServicesController>();

        protected Game m_Game;
        protected Game Game => m_Game ?? Pool.Get<Game>();

        protected GamePlayGroup m_GamePlay;
        protected GamePlayGroup GamePlay => m_GamePlay ?? Pool.Get<GamePlayGroup>();

        protected PlayerSecret m_Player;
        protected PlayerSecret PlayerData => m_Player ?? Pool.Get<PlayerSecret>();

        protected UIGroup m_UIGroup;
        protected UIGroup UI => m_UIGroup ?? Pool.Get<UIGroup>();

        protected GameConfig m_Config;
        protected GameConfig Config => m_Config ?? Pool.Get<GameConfig>();

    }
}