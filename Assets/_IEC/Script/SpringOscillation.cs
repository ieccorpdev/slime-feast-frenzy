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
    private float movementLimit = 1.5f;

    private float amplitude = 0f;
    private float time = 0f;
    private bool isOscillating = false;

    private Vector3 upDirection = Vector3.up;

    private void Start()
    {
        damping = Config.gamePlayConfig.damping;
        frequency = Config.gamePlayConfig.frequency;
        minAmplitude = Config.gamePlayConfig.minAmplitude;
        amplitude = Config.gamePlayConfig.amplitude;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyForce(-1f);
        }
        if (!isOscillating) return;

        time += Time.deltaTime;

        float decay = Mathf.Exp(-damping * time);
        float offset = -amplitude * decay * Mathf.Cos(2 * Mathf.PI * frequency * time);

        // Tính vị trí gốc theo vị trí của parent
        Vector3 basePosition = transform.parent ? transform.parent.position : Vector3.zero;
        
        // Giới hạn offset để không vượt quá movementLimit
        float clampedOffset = Mathf.Clamp(offset, -movementLimit, movementLimit);

        transform.position = basePosition + upDirection * clampedOffset;

        if (Mathf.Abs(offset) < minAmplitude)
        {
            isOscillating = false;
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

        // Cộng dồn lực nếu đang dao động
        if (isOscillating)
        {
            amplitude += newForce;
        }
        else
        {
            amplitude = newForce;
            time = 0f;
            isOscillating = true;
        }

        // Giới hạn biên độ
        amplitude = Mathf.Min(amplitude, movementLimit);
    }
}