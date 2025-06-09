using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Gameplay;
using IEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class AddForceOnCollision : MonoBehaviour
{
    public Rigidbody rb;
    public float force = 20f;
    private bool hasBounced = false;
    public Action OnCollision;
    //public List<BaseItem> listItem = new List<BaseItem>();


    private void OnCollisionEnter(Collision collision)
    {
        if (hasBounced) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Hole"))
        {
            hasBounced = true;
            var randomRangeX = Random.Range(0, 180f);
            var randomRangeY = Random.Range(0, 180f);
            var randomRangeZ = Random.Range(0, 180f);
            Vector3 direction = new Vector3(randomRangeX, randomRangeY, randomRangeZ).normalized;
            GetComponent<Rigidbody>()?.AddForce(direction * force, ForceMode.Impulse);
            //EventController.RaiseEvent(EventList.Audio.RELIC_CLONE);
            OnCollision?.Invoke();
        }
    }

    // IEnumerator IAddForce()
    // {
    //     var was = new WaitForSeconds(0.5f);
    //     foreach (BaseItem item in listItem)
    //     {
    //         item.gameObject.SetActive(true);
    //         item.Rb.AddForce(direction * force, ForceMode.Impulse);
    //         yield return was;
    //     }
    // }
}