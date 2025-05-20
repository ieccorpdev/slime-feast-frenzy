using System;
using IEC;
using UnityEngine;

public class SpringOscillation : _BaseBehaviour
{
    [Header("Oscillation Settings")]
    public float damping = 0.8f;
    public float frequency = 1.0f;
    public float minAmplitude = 0.01f;

    [Header("Limit Settings")]
    public float movementLimit = 1.5f;
    public float amplitudeSmoothSpeed = 5f; // tốc độ nội suy amplitude

    private float amplitude = 0f;
    private float targetAmplitude = 0f; // amplitude mục tiêu
    private float time = 0f;
    private bool isOscillating = false;
    private Vector3 upDirection = Vector3.up;

    private void Start()
    {
        //damping = Config.gamePlayConfig.damping;
        //frequency = Config.gamePlayConfig.frequency;
        //minAmplitude = Config.gamePlayConfig.minAmplitude;
        //amplitude = Config.gamePlayConfig.amplitude;
    }
    

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ApplyForce(-1f);
            }
            if (!isOscillating) return;
            // Nội suy biên độ để tránh giật
            amplitude = Mathf.Lerp(amplitude, targetAmplitude, Time.deltaTime * amplitudeSmoothSpeed);

            time += Time.deltaTime;

            float decay = Mathf.Exp(-damping * time);
            float offset = -amplitude * decay * Mathf.Cos(2 * Mathf.PI * frequency * time);

            Vector3 basePosition = transform.parent ? transform.parent.position : Vector3.zero;
            float clampedOffset = Mathf.Clamp(offset, -movementLimit, movementLimit);
            transform.position = basePosition + upDirection * clampedOffset;

            if (Mathf.Abs(offset) < minAmplitude && Mathf.Abs(targetAmplitude - amplitude) < 0.01f)
            {
                isOscillating = false;
                amplitude = 0f;
                targetAmplitude = 0f;
                transform.position = basePosition;
            }
        }

        public void ApplyForce(float force)
        {
            Debug.LogError(force);

            if (force >= 0f)
            {
                Debug.LogWarning("SpringOscillation: Force must be negative to simulate downward pull.");
                return;
            }

            float newForce = Mathf.Abs(force);

            // Set amplitude mục tiêu, nội suy từ từ thay vì thay đổi ngay lập tức
            targetAmplitude = Mathf.Min(targetAmplitude + newForce, movementLimit);

            if (!isOscillating)
            {
                time = 0f;
                amplitude = 0f; // bắt đầu từ 0 để tránh nhảy đột ngột
                isOscillating = true;
            }
        }
}