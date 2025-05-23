using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class ScreenRatioAdjuster : MonoBehaviour
{
    public Camera targetCamera; 
    public float threshold = 20f / 9f;
    public float matchUpRatio = 0; 
    public float matchDownRatio = 1; 

    void Start()
    {
        AdjustCanvasScaler(true);
    }

    void AdjustCanvasScaler(bool withLog = false)
    {
        if (targetCamera == null)
        {
            return;
        }

        float screenWidth = targetCamera.pixelWidth;
        float screenHeight = targetCamera.pixelHeight;
        float aspectRatio = screenHeight / screenWidth;

        CanvasScaler scaler = GetComponent<CanvasScaler>();

        if (aspectRatio > threshold)
        {
            scaler.matchWidthOrHeight = matchUpRatio; 
        }
        else
        {
            scaler.matchWidthOrHeight = matchDownRatio;
        }

        if (withLog)
        {
            Debug.Log($"Aspect Ratio: {aspectRatio}, matchWidthOrHeight set to: {scaler.matchWidthOrHeight}");
        }

    }

    private void OnDrawGizmos()
    {
        AdjustCanvasScaler();
    }
}
