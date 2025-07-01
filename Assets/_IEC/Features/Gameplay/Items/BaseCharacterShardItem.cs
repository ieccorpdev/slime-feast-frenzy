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

            var obj = GetComponentInChildren<Animator>()?.gameObject;

            if (obj != null)
            {
                var parentTransform = obj.transform.parent;
                var pos = obj.transform.localPosition;
                var rotate = obj.transform.localRotation;
                var scale = obj.transform.localScale;
                Destroy(obj);
                obj = CharacterConfig.characterHomePrefab.Spawn(parentTransform).gameObject;
                obj.transform.localPosition = pos;
                obj.transform.localRotation = rotate;
                obj.transform.localScale = scale;
                obj.GetComponent<Animator>().enabled = false;
                obj.GetComponent<FreeSwimingCollider>().enabled = false;
                obj.GetComponent<Collider>().enabled = false;
                obj.layer = gameObject.layer;
            }
        }

        public override void OnCollected(Vector3 position)
        {
            base.OnCollected(position);
            
            this.RaiseEvent(EventList.GamePlay.COLLECT_CHARACTER_SHARD, CharacterConfig.characterName);
        }
    }
}
