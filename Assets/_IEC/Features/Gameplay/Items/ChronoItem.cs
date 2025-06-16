using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class ChronoItem : BaseBoosterItem
    {
        public override void OnCollected(Vector3 position)
        {
            base.OnCollected(position);
            ApplyEffect();
        }

        public override void ApplyEffect()
        {
            //var configIndex = Config.magicPouchConfig.boosters.FindIndex(x => x.type == MagicPouchBoosterType.Chrono);
            //if (configIndex < 0)
            //{
            //    return;
            //}
            //ShowClockVfx();
            //
            //var boosterConfig = Config.magicPouchConfig.boosters[configIndex];
            //this.RaiseEvent(EventList.GamePlay.ON_BOOSTER_COLLECTED, boosterConfig);
        }
        

        private void ShowClockVfx()
        {
            //TODO
        }
    }
}
