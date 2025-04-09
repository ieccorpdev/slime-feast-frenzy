using _IEC.Features.Common.Extension;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class LightningItem : BaseBoosterItem
    {
        public override void OnCollected(Vector3 position)
        {
            base.OnCollected(position);
            ApplyEffect();
        }

        public override void ApplyEffect()
        {
            //var configIndex = Config.magicPouchConfig.boosters.FindIndex(x => x.type == MagicPouchBoosterType.Lightning);
            //if (configIndex < 0)
            //{
            //    return;
            //}
//
            //var boosterConfig = Config.magicPouchConfig.boosters[configIndex];
            //this.RaiseEvent(EventList.GamePlay.ON_BOOSTER_COLLECTED, boosterConfig);
        }
    }
}