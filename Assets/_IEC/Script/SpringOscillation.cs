using System;
using IEC;
using UnityEngine;
using UnityEngine;

public class SpringOscillation : _BaseBehaviour
{
    [Header("Oscillation Settings")] public float damping = 0.8f;
    public float frequency = 1.0f;
    public float minAmplitude = 0.01f;

    [Header("Limit Settings")] public float movementLimit = 1.5f;
    public float amplitudeSmoothSpeed = 5f; // tốc độ nội suy amplitude

    private float amplitude = 0f;
    private float targetAmplitude = 0f;
    private float time = 0f;
    private bool isOscillating = false;

    private Vector3 localStartPosition; // Vị trí local gốc lúc start

    void Start()
    {
        damping = Config.gamePlayConfig.damping;
        frequency = Config.gamePlayConfig.frequency;
        minAmplitude = Config.gamePlayConfig.minAmplitude;
        amplitude = Config.gamePlayConfig.amplitude;

        // Ghi nhận vị trí local ban đầu lúc start
        localStartPosition = transform.localPosition;
    }

    void Update()
    {
        if (!isOscillating) return;

        // Nội suy biên độ để tránh giật
        amplitude = Mathf.Lerp(amplitude, targetAmplitude, Time.deltaTime * amplitudeSmoothSpeed);

        time += Time.deltaTime;

        float decay = Mathf.Exp(-damping * time);
        float offset = -amplitude * decay * Mathf.Cos(2 * Mathf.PI * frequency * time);

        Vector3 worldBasePosition = transform.parent
            ? transform.parent.TransformPoint(localStartPosition)
            : localStartPosition;

        float clampedOffset = Mathf.Clamp(offset, -movementLimit, movementLimit);

        // Áp dụng dao động chỉ theo trục Y thế giới
        transform.position = new Vector3(
            worldBasePosition.x,
            worldBasePosition.y + clampedOffset,
            worldBasePosition.z
        );

        if (Mathf.Abs(offset) < minAmplitude && Mathf.Abs(targetAmplitude - amplitude) < 0.01f)
        {
            isOscillating = false;
            amplitude = 0f;
            targetAmplitude = 0f;

            // Trở về vị trí ban đầu (ổn định)
            transform.position = worldBasePosition;
        }
    }

    public void ApplyForce(float force)
    {
        Debug.LogError(force);

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