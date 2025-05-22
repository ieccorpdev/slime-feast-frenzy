using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class _BaseBehaviour : BaseBehaviour
    {
        protected static ServicesController Services => Controllers.Get<ServicesController>();
        protected static DataController Data => Controllers.Get<DataController>();
        
        protected static SceneController Scene => Controllers.Get<SceneController>();

        protected static Game Game => Pool.Get<Game>();
        
        protected static InGamePlayGroup GamePlay => Pool.Get<InGamePlayGroup>();
        
        protected static PlayerSecret PlayerData => Pool.Get<PlayerSecret>();
        
        protected static UIGroup UI => Pool.Get<UIGroup>();
        protected static Joystick Joystick => Pool.Get<Joystick>();
        
        protected static GameConfig Config => Pool.Get<GameConfig>();
        protected static SoundController SoundController => Pool.Get<SoundController>();

        protected static HybridConfig HybridConfig => Config.hybridConfig;
		public virtual void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public virtual void SetActive(GameObject gameObject, bool isActive)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(isActive);
            }
        }

        public virtual void SetActive(Component component, bool isActive)
        {
            if (component != null)
            {
                component.gameObject.SetActive(isActive);
            }
        }
        //("LeaderBoard")
        protected static LeaderboardController LeaderboardController => Controllers.Get<LeaderboardController>();
    }
}
