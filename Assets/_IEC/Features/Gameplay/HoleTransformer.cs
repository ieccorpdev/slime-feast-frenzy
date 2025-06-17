using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class HoleTransformer : MonoBehaviour
    {
        private Vector3 targetVelocity = Vector3.zero;

        public void Move(Vector3 velocity)
        {
            targetVelocity = velocity;
        }

        private void Update()
        {
            if (targetVelocity != Vector3.zero)
            {
                transform.Translate(targetVelocity * Time.deltaTime, Space.World);
            }
        }
    }
}
