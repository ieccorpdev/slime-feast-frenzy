using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTankController : MonoBehaviour
{
    public Camera targetCamera;

    private float delay;
    private float maxDelay =1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public LayerMask fishLayer;
    void Update()
    {
        delay -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && delay < 0) // Left mouse button or screen touch
        {
            Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition); // Convert screen to ray
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, fishLayer))
            {
                GameObject clickedObject = hit.collider.gameObject;
                if (clickedObject.CompareTag("Fish"))
                {
                    var fsc = clickedObject.GetComponent<FreeSwimingCollider>();
                    if (fsc != null)
                    {
                        fsc.TouchFish();
                        delay = maxDelay;
                    }
                }
            }
        }
    }
}
