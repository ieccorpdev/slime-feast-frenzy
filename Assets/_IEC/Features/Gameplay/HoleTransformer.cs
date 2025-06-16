using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class HoleTransformer : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        private Vector3 targetVelocity = Vector3.zero;

        public void Move(Vector3 velocity)
        {
            targetVelocity = velocity;
        }

        private void FixedUpdate()
        {
            if (targetVelocity != Vector3.zero)
            {
                Vector3 newPosition = rb.position + targetVelocity * Time.fixedDeltaTime;
                rb.MovePosition(newPosition);
            }
        }
    }
}
