using System.Collections;
using System.Collections.Generic;
using IEC.Wind.Leaderboard;
using UnityEngine;

namespace IEC
{
    public class _BaseBehaviour : BaseBehaviour
    {
        
        protected static Game Game => Pool.Get<Game>();
        
        protected static InGamePlayGroup GamePlay => Pool.Get<InGamePlayGroup>();
        
        protected static PlayerSecret PlayerData => Pool.Get<PlayerSecret>();
        
        protected static Joystick Joystick => Pool.Get<Joystick>();
        
        protected static GameConfig Config => PlayableController.Instance.gameConfig;

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
        protected static LeaderboardConfig LeaderboardConfig => Pool.Get<LeaderboardConfig>();
        protected static LeaderboardSaveData LeaderboardData => Pool.Get<LeaderboardSaveData>();
    }
}
