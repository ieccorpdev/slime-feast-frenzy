using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using _IEC.Features.Gameplay.ObserverEventData;
using UnityEngine;

namespace IEC
{
    public class BaseCharacterShardItem : BaseItem
    {
        public CharacterConfig CharacterConfig { get; private set; }
        
        public void Setup(CharacterConfig characterConfig)
        {
            CharacterConfig = characterConfig;
        }

        public override void OnCollected(Vector3 position)
        {
            base.OnCollected(position);
            
            this.RaiseEvent(EventList.GamePlay.COLLECT_CHARACTER_SHARD, CharacterConfig.characterName);
        }
    }
}
