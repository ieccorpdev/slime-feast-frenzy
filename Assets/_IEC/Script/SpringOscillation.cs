using _IEC.Script.GamePlay;
using IEC;
using UnityEngine;

public class SpringOscillation : _BaseBehaviour
{
    [Header("Oscillation Settings")]
    public float damping = 0.8f;
    public float frequency = 1.0f;
    public float minAmplitude = 0.01f;

    [Header("Size Multipliers")]
    public float sizeAmplitudeMultiplier = 1.0f;

    [Tooltip("Hệ số nhân giới hạn dao động theo kích cỡ object.")]
    public float sizeMovementLimitMultiplier = 1.0f;

    [Header("Smooth Settings")]
    public float amplitudeSmoothSpeed = 5f;

    private float amplitude = 0f;
    private float targetAmplitude = 0f;
    private float movementLimit = 1.5f;
    private float time = 0f;
    private bool isOscillating = false;

    private Vector3 localStartPosition;

    void Start()
    {
        damping = Config.gamePlayConfig.damping;
        frequency = Config.gamePlayConfig.frequency;
        minAmplitude = Config.gamePlayConfig.minAmplitude;
        amplitude = Config.gamePlayConfig.amplitude;
        amplitudeSmoothSpeed = Config.gamePlayConfig.amplitudeSmoothSpeed;
        sizeAmplitudeMultiplier = Config.gamePlayConfig.sizeAmplitudeMultiplier;
        sizeMovementLimitMultiplier = Config.gamePlayConfig.sizeMovementLimitMultiplier;
        
        localStartPosition = transform.localPosition;
    }

    public void SetSizeDependentLimits(Vector3 boundsSize)
    {
        float maxSize = Mathf.Max(boundsSize.x, boundsSize.y, boundsSize.z);
        if ( Config.gamePlayConfig != null)
        {
            amplitude = Config.gamePlayConfig.amplitude * maxSize * sizeAmplitudeMultiplier;
            movementLimit = maxSize * sizeMovementLimitMultiplier;
        }

    }

    public void ManagedUpdate()
    {
        CheckSpringObject();
    }

    public void CheckSpringObject()
    {
        if (!isOscillating) return;

        amplitude = Mathf.Lerp(amplitude, targetAmplitude, Time.deltaTime * amplitudeSmoothSpeed);
        time += Time.deltaTime;

        float decay = Mathf.Exp(-damping * time);
        float offset = -amplitude * decay * Mathf.Cos(2 * Mathf.PI * frequency * time);

        Vector3 worldBasePosition = transform.parent
            ? transform.parent.TransformPoint(localStartPosition)
            : localStartPosition;

        float clampedOffset = Mathf.Clamp(offset, -movementLimit, movementLimit);

        transform.position = new Vector3(
            worldBasePosition.x,
            worldBasePosition.y + clampedOffset,
            worldBasePosition.z
        );

        if (Mathf.Abs(offset) < minAmplitude && Mathf.Abs(targetAmplitude - amplitude) < minAmplitude)
        {
            isOscillating = false;
            amplitude = 0f;
            targetAmplitude = 0f;
            transform.position = worldBasePosition;
        }
    }

    public void ApplyForce(float force)
    {
        if (force >= 0f)
        {
            return;
        }

        float newForce = Mathf.Abs(force);
        targetAmplitude = Mathf.Min(targetAmplitude + newForce, movementLimit);

        if (!isOscillating)
        {
            time = 0f;
            amplitude = 0f;
            isOscillating = true;
        }
    }
}
