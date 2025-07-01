using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(Canvas))]
public class ScreenRatioAdjuster : MonoBehaviour
{
    private Camera targetCamera; 
    public float threshold = 20f / 9f;
    private float matchUpRatio = 0;
    private float matchDownRatio = 1; 

    void Start()
    {
        targetCamera = this.GetComponent<Canvas>().worldCamera;
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

    // private void OnDrawGizmos()
    // {
    //     AdjustCanvasScaler();
    // }
}
