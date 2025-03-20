using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Gameplay;
using UnityEngine;

namespace IEC
{
    public class CoinItem : BaseItem
    {
        private bool _isRotating;
        private Quaternion startRotation;

        [SerializeField] private float tiltAngle = 20f;
        [SerializeField] private float tiltSpeed = 2.5f;

        private void Update()
        {
            if (!_isRotating)
            {
                return;
            }

            float currentTilt = tiltAngle * Mathf.Sin(Time.time * tiltSpeed);
            transform.rotation = startRotation * Quaternion.Euler(0f, currentTilt, 0f);
        }

        public override void LoadData(_IEC.Features.Gameplay.ItemData itemData, ItemDefinition itemDefinition)
        {
            base.LoadData(itemData, itemDefinition);
            startRotation = transform.rotation;
            _isRotating = true;
        }

        protected override void OnBecomeCollectable()
        {
            _isRotating = false;
            base.OnBecomeCollectable();
        }
    }
}