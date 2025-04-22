using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class HoleTransformer : BaseBehaviour
    {
        [SerializeField] private Rigidbody rb;
        public void Move(Vector3 velocity)
        {
            rb.velocity = velocity;
        }
    }
}