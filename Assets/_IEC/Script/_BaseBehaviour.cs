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

        protected static Game Game => Pool.Get<Game>();
        
        protected static GamePlayGroup GamePlay => Pool.Get<GamePlayGroup>();
        
        protected static PlayerSecret PlayerData => Pool.Get<PlayerSecret>();

        protected static UIGroup UI => Pool.Get<UIGroup>();
        protected static GameConfig Config => Pool.Get<GameConfig>();
        protected static ShopConfig ShopConfig => Config.shopConfig;
        protected static ComponentConfig ComponentConfig => Config.componentConfig;
    }
}