using System;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class PlayerHole : _BaseBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Collider triggerCollider;

        public void SetStartPosition(Vector3 position)
        {
            transform.position = position;
        }
        public void Move(Vector2 direction)
        {
            
            var dir = Vector3.forward * direction.y + Vector3.right * direction.x;
           
            if(dir.magnitude > Mathf.Epsilon)
                transform.rotation = Quaternion.LookRotation(dir);
        }

        private void OnTriggerEnter(Collider other)
        {
            //todo
            Debug.LogError("trigger enter");
            
        }

        private void OnTriggerExit(Collider other)
        {
            var itemObject = other.GetComponent<BaseItem>();
            if (itemObject != null)
            {
                itemObject.ExitHoleTrigger();
            }
        }
        private void OnTriggerStay(Collider other)
        {
            // Get the bounds of the trigger zone and the other collider
            var item = other.GetComponent<BaseItem>();
            if (item == null) return;
            Bounds triggerBounds = triggerCollider.bounds;
            Bounds otherBounds = other.bounds;
            
            if (IsColliderCompletelyInTrigger(triggerBounds, otherBounds))
            {
                item.EnterHoleTrigger();
            }
        }
        
        private bool IsColliderCompletelyInTrigger(Bounds triggerBounds, Bounds otherBounds)
        {
            return triggerBounds.Contains(otherBounds.min) && triggerBounds.Contains(otherBounds.max);
        }
    }
}
