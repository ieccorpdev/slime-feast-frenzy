using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Gameplay;
using UnityEngine;

namespace IEC
{
    public class HoleBombShield : _BaseBehaviour
    {
        public Collider collider;
        public float pushForce = 10f;

        private int _bombCount;
        private float _shieldCountdown;

        private void Start()
        {
            _bombCount = 0;
            _shieldCountdown = 0;
        }

        public void SetupShield(int bombCount, float countdown)
        {
            _bombCount = bombCount;
            _shieldCountdown = countdown;
        }

        private void Update()
        {
            if (_shieldCountdown > 0)
            {
                _shieldCountdown -= Time.deltaTime;
            }
        }

        private void CheckRepelBomb(Rigidbody bombRb)
        {
            if (_shieldCountdown <= 0)
                return;
            
            if (_bombCount <= 0)
                return;
            
            _bombCount--;
            
            if (bombRb != null)
            {
                Vector3 direction = bombRb.transform.position - transform.position;
                //direction.y = 0;
                bombRb.AddForce(direction.normalized * pushForce, ForceMode.Impulse);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var bombObj = other.gameObject.GetComponentInParent<BombItem>();

            if (bombObj != null)
            {
                CheckRepelBomb(bombObj.GetComponent<Rigidbody>());
            }
        }
    }
}
