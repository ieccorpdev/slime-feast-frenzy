using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class LevelObject : _BaseBehaviour
    {
        [SerializeField] private PlayerHoleStartPoint startPoint;

        public Vector3 GetStartPlayerHolePosition()
        {
            return startPoint.transform.position;
        }
    }
}