using UnityEngine;
using System.Collections.Generic;

public class ShaderChecker : MonoBehaviour
{
    void Start()
    {
        // Dictionary: Shader -> List of GameObjects using it
        Dictionary<Shader, List<GameObject>> shaderToObjects = new Dictionary<Shader, List<GameObject>>();

        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.sharedMaterials)
            {
                if (mat != null && mat.shader != null)
                {
                    if (!shaderToObjects.ContainsKey(mat.shader))
                    {
                        shaderToObjects[mat.shader] = new List<GameObject>();
                    }
                    shaderToObjects[mat.shader].Add(rend.gameObject);
                }
            }
        }

        // In ra Console
        Debug.Log("===== Shader Usage Report =====");
        foreach (var pair in shaderToObjects)
        {
            Debug.Log($"Shader: {pair.Key.name} is used by:");
            foreach (GameObject go in pair.Value)
            {
                Debug.Log($"    - {go.name}", go); // Kèm theo link tới object trong console
            }
        }
    }
}