using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AutoScale : MonoBehaviour
{
    public float scaleSpeed = 1.5f;  // Speed of scaling up and down
    public float minScale = 0.8f;  // Minimum scale value
    public float maxScale = 1.2f;    // Maximum scale value

    private Vector3 initialScale;

    void Start()
    {
        // Store the initial scale of the object
        initialScale = transform.localScale;
    }

    void Update()
    {
        // Call the function to scale the object up and down
        ScaleObject();
    }

    void ScaleObject()
    {
        // Calculate the scaling factor based on a sine wave
        float scaleFactor = Mathf.Sin(Time.time * scaleSpeed) * 0.5f + 0.5f; // Sin oscillates between -1 and 1, so we scale it to be between 0 and 1

        // Scale the object up and down smoothly between minScale and maxScale
        float newScale = Mathf.Lerp(minScale, maxScale, scaleFactor);

        // Apply the scaling to the object
        transform.localScale = initialScale * newScale;
    }
}
