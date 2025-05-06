using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace IEC
{
    public class SpecialItemParticleEffectHighlight : MonoBehaviour
    {
        public ParticleSystem mainParticle;

        public void Start()
        {
            mainParticle.Stop();
            DOVirtual.DelayedCall(3f, DelayStartParticle).SetId(GetInstanceID() + "_DelayCallItem");
        }

        void DelayStartParticle()
        {
            mainParticle.Play();
        }
        void Update()
        {
            transform.rotation = Quaternion.identity;
        }

        private void OnDestroy()
        {
            DOTween.Kill(GetInstanceID() + "_DelayCallItem");
        }
    }
}
