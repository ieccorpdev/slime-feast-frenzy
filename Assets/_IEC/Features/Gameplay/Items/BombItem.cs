using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using DG.Tweening;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class BombItem : BaseItem
    {
        public override void OnCollected(Vector3 position)
        {
            this.RaiseEvent(EventList.GamePlay.COLLECT_ITEM,new CollectItemData()
            {
                itemID = "Bomb",
                position = position,
                xp = 0
            });
            
            DisableAllCollider();
        }
    }
}