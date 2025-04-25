using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class HoleTransformer : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        public void Move(Vector3 velocity)
        {
            rb.velocity = velocity;
        }
    }
}