using System.Linq;
using UnityEngine;

public class FreeSwimingCollider : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float minSpeed = 1f;
    public float rotationSpeed = 2f;
    public float avoidanceDistance = 5f;
    public LayerMask obstacleLayer;
    public float targetProximityThreshold = 1f;
    public Collider swimAreaMeshCollider;
    private bool inBoosterTime = false;
    private float timeInBooster = 0f;
    private Vector3 _targetPosition;
    private Collider[] _obstacles;

    private void Start()
    {
        SetRandomTargetPosition();
    }

    private void Update()
    {
        MoveTowardsTarget();
        StayWithinBounds();
        if (timeInBooster >= 0)
        {
            inBoosterTime = true;  
            timeInBooster -= Time.deltaTime;
        }
        else
        {
            inBoosterTime = false;  
        }
    }

    private void SetRandomTargetPosition()
    {
        Bounds bounds = swimAreaMeshCollider.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        _targetPosition = new Vector3(x, y, z);
    }

    private void MoveTowardsTarget()
    {
        _obstacles = Physics.OverlapSphere(transform.position, avoidanceDistance, obstacleLayer);

        if (_obstacles.Length > 0)
        {
            var avoidanceDirection = _obstacles.Aggregate(Vector3.zero, (current, obstacle) => current + (transform.position - obstacle.transform.position).normalized);

            var avoidanceRotation = Quaternion.LookRotation(avoidanceDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, avoidanceRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            ChangeDirMove();
        }

        if (Vector3.Distance(transform.position, _targetPosition) <= targetProximityThreshold)
        {
            SetRandomTargetPosition();
        }
    }

    private void ChangeDirMove()
    {
        var direction = _targetPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        var distanceToTarget = direction.magnitude;
        var speedMultiplier = Mathf.Clamp01(distanceToTarget / avoidanceDistance);
        var speed = Mathf.Lerp(minSpeed, maxSpeed, speedMultiplier);
        transform.Translate(Vector3.forward * (speed * Time.deltaTime * (inBoosterTime ? 2 : 1)));
    }

    public void TouchFish()
    {
        timeInBooster = 2f;
        SetRandomTargetPosition();
        ChangeDirMove();
    }
    private void StayWithinBounds()
    {
        Vector3 currentPosition = transform.position;
        Vector3 closestPoint = swimAreaMeshCollider.ClosestPoint(currentPosition);

        if (currentPosition != closestPoint)
        {
            transform.position = closestPoint;
            SetRandomTargetPosition();
        }
    }

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(transform.position, avoidanceDistance);
    // }
}
