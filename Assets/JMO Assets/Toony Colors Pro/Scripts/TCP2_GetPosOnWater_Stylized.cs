using System;
using UnityEngine;

public class TCP2_GetPosOnWater_Stylized : MonoBehaviour
{
        [Header("Water Material")]
    public Material WaterMaterial;

    [Header("Wave Offset Settings")]
    public float heightOffset = 0f;
    public float bobAmount = 0f;
    public float bobSpeed = 1f;

    // Internal wave parameters
    float wave1Direction, wave1Wavelength, wave1Amplitude, wave1Speed;
    float wave2Direction, wave2Wavelength, wave2Amplitude, wave2Speed;

    private void Start()
    {
        UpdateWaveParams();
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        float y = GetWaveHeight(pos);
        
        // Optional bobbing effect
        float bob = bobAmount > 0f ? Mathf.Sin(Time.time * bobSpeed) * bobAmount : 0f;

        transform.position = new Vector3(pos.x, y + heightOffset + bob, pos.z);
    }

    float GetWaveHeight(Vector3 worldPosition)
    {
        //UpdateWaveParams();

        Vector2 position = new Vector2(worldPosition.x, worldPosition.z);

        Vector2 dir1 = new Vector2(Mathf.Cos(Mathf.PI * wave1Direction), Mathf.Sin(Mathf.PI * wave1Direction));
        Vector2 dir2 = new Vector2(Mathf.Cos(Mathf.PI * wave2Direction), Mathf.Sin(Mathf.PI * wave2Direction));

        float wave1 = SimpleWave(position, dir1, wave1Wavelength, wave1Amplitude, wave1Speed);
        float wave2 = SimpleWave(position, dir2, wave2Wavelength, wave2Amplitude, wave2Speed);

        return wave1 + wave2;
    }

    float SimpleWave(Vector2 position, Vector2 direction, float wavelength, float amplitude, float speed)
    {
        float shaderTime = Shader.GetGlobalVector("_Time").y*0.68f;
        float x = Mathf.PI * Vector2.Dot(position, direction) / wavelength;
        float phase = speed * shaderTime; // chính xác hơn với shader
        return amplitude * Mathf.Sin(x + phase);
    }

    void UpdateWaveParams()
    {
        if (WaterMaterial == null) return;

        wave1Direction = WaterMaterial.GetFloat("_Wave1Direction");
        wave1Wavelength = WaterMaterial.GetFloat("_Wave1Wavelength");
        wave1Amplitude = WaterMaterial.GetFloat("_Wave1Amplitude");;
        wave1Speed = WaterMaterial.GetFloat("_Wave1Speed");

        wave2Direction = WaterMaterial.GetFloat("_Wave2Direction");
        wave2Wavelength = WaterMaterial.GetFloat("_Wave2Wavelength");
        wave2Amplitude = WaterMaterial.GetFloat("_Wave2Amplitude");;
        wave2Speed = WaterMaterial.GetFloat("_Wave2Speed");
    }
}
