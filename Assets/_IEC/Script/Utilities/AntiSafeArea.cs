using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class AntiSafeArea : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform safeAreaParent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        safeAreaParent = transform.parent as RectTransform;

        if (safeAreaParent == null)
        {
            Debug.LogError("AntiSafeArea must be a child of a RectTransform (SafeArea container).");
            enabled = false;
            return;
        }

        ApplyAntiSafeArea();
    }

    private void Start()
    {
        ApplyAntiSafeArea();
    }


    void ApplyAntiSafeArea()
    {
        // Calculate unsafe top and bottom based on Screen.safeArea
        float screenHeight = Screen.height;

        float safeYMin = Screen.safeArea.yMin;
        float safeYMax = Screen.safeArea.yMax;

        float unsafeBottom = safeYMin;                       // Pixels from screen bottom to safe area start
        float unsafeTop = screenHeight - safeYMax;           // Pixels from safe area end to top of screen

        // Stretch anchors to match SafeArea parent
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Total vertical margin to add
        float totalExpand = unsafeTop + unsafeBottom;
        rectTransform.sizeDelta = new Vector2(0, totalExpand);

        // Offset it so expansion is centered around middle
        float verticalOffset = (unsafeTop - unsafeBottom) / 2f;
        rectTransform.anchoredPosition = new Vector2(0, verticalOffset);
    }
}