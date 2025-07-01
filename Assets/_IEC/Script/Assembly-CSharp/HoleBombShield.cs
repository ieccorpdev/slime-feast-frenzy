using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Gameplay;
using UnityEngine;

namespace IEC
{
    public class HoleBombShield : _BaseBehaviour
    {
        public SphereCollider collider;
        public GameObject shieldSphereObj;
        public ParticleSystem bombExplosionFx;
        public float pushForce = 100f;

        private int _bombCount;
        private float _shieldCountdown;

        private Coroutine _startShieldCoroutine;

        private List<ParticleSystem> listBombExplosionFx = new();

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

        // IEnumerator IEStartShield()
        // {
        //     var initShieldRadius = collider.radius;
        //
        //     collider.radius = 0f;
        //     while (collider.radius < initShieldRadius)
        //     {
        //         collider.radius += Time.deltaTime * 2;
        //         yield return null;
        //     }
        //
        //     collider.radius = initShieldRadius;
        //     
        //     StopCoroutine(_startShieldCoroutine);
        //     _startShieldCoroutine = null;
        //     //yield return null;
        // }

        private void Update()
        {
            if (_shieldCountdown > 0)
            {
                _shieldCountdown -= Time.deltaTime;
            }
            
            shieldSphereObj.SetActive(_shieldCountdown > 0 && _bombCount > 0);
        }

        private void CheckRepelBomb(BombItem bomb)
        {
            if (_shieldCountdown <= 0)
                return;
            
            if (_bombCount <= 0)
                return;
            
            _bombCount--;
            
            if (bomb != null)
            {
                // Vector3 direction = bombRb.transform.position - transform.position + Vector3.up;
                // //direction.y = 0;
                // bombRb.AddForce(direction.normalized * pushForce, ForceMode.Impulse);
                bomb.SetExplode();

                var explosion = listBombExplosionFx.Find(fx => fx.isPlaying);

                if (explosion == null)
                {
                    explosion = bombExplosionFx.Spawn(this.transform);
                    listBombExplosionFx.Add(explosion);
                }

                explosion.transform.position = bomb.transform.position;
                explosion.transform.localScale = Vector3.one * 1.5f;
                explosion.gameObject.SetActive(true);
                explosion.Play();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var bombObj = other.gameObject.GetComponentInParent<BombItem>();

            if (bombObj != null)
            {
                //if (!listRepeledBombs.Contains(bombObj))
                {
                    //listRepeledBombs.Add(bombObj);
                    CheckRepelBomb(bombObj);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            var bombObj = other.gameObject.GetComponentInParent<BombItem>();

            if (bombObj != null)
            {
                //if (!listRepeledBombs.Contains(bombObj))
                {
                    //listRepeledBombs.Add(bombObj);
                    CheckRepelBomb(bombObj);
                }
            }
        }
    }
}
