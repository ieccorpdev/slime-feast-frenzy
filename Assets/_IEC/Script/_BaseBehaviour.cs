using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class _BaseBehaviour : BaseBehaviour
    {
        protected static ServicesController Services => Controllers.Get<ServicesController>();
        protected static GameController GameController => Controllers.Get<GameController>();
        protected static LevelController LevelController => Controllers.Get<LevelController>();
        protected static UIController UIController => Controllers.Get<UIController>();
        protected static SoundController Sound => Controllers.Get<SoundController>();
        protected static HintController Hint => Controllers.Get<HintController>();
        protected static EffectController EffectController => Controllers.Get<EffectController>();

        private static Game m_Game;
        protected static Game Game => m_Game ?? Pool.Get<Game>();
        
        protected static GamePlayGroup GamePlay => Pool.Get<GamePlayGroup>();
        
        protected static PlayerSecret PlayerData => Pool.Get<PlayerSecret>();

        private static UIGroup m_UI;
        protected static UIGroup UI => m_UI ?? Pool.Get<UIGroup>();

        private static GameConfig m_GameConfig;
        protected static GameConfig Config => m_GameConfig ?? Pool.Get<GameConfig>();
        protected static ShopConfig ShopConfig => Config.shopConfig;
        protected static ComponentConfig ComponentConfig => Config.componentConfig;
    }
}