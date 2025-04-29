using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Gameplay;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class SpawnAllPRefabItem : MonoBehaviour
{
#if UNITY_EDITOR
    public GameplayItemPrefabConfig itemConfig;
    public float spacing = 2f;           // Space between each spawned object
    public float offsetYrange = 2f;           // Space between each spawned object
    public float maxXRange = 50f;           // Space between each spawned object
    public List<GameObject> objWasSpawn;
    void Start()
    {

        // Spawn all the prefabs in the scene with defined spacing
        //SpawnObjectsWithSpacing();
    }
    [Button]
    void SpawnObjectsWithSpacing()
    {
        for (int i = objWasSpawn.Count-1; i >=0    ; i--)
        {
            DestroyImmediate(objWasSpawn[i]);
        }
        objWasSpawn.Clear();
        Vector3 spawnPosition = Vector3.zero;  // Start position for spawning (can be adjusted)
        float maxZ = 0;
        // Loop through the list of prefabs
        for (int i = 0; i < itemConfig.listItemDefinitionShp.Count; i++)
        {
            GameObject prefab = itemConfig.listItemDefinitionShp[i].GetPrefab();
            if (prefab == null)
            {
                Debug.LogError("Can't Found item id :" + itemConfig.listItemDefinitionShp[i].GetId());
                continue;
            }
#if UNITY_EDITOR
                        GameObject spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                        spawnedObject.transform.position = spawnPosition;
            #else
            GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
            #endif
            // Get the collider from the child (assuming it exists in the child)
            spawnedObject.transform.parent = this.transform;
            Collider collider = spawnedObject.GetComponentInChildren<Collider>();

            if (collider != null)
            {
                // Get the size of the collider's bounds (it's typically the 3D bounding box)
                Bounds colliderBounds = collider.bounds;

                // Calculate the next spawn position by adding the spacing and the width of the collider
                // Add spacing + size along the X axis (or any other axis you want)
                spawnPosition.x += spacing + colliderBounds.size.x;
                maxZ = colliderBounds.size.z > maxZ ? colliderBounds.size.z : maxZ;
                if (spawnPosition.x > maxXRange)
                {
                    //Down
                    spawnPosition.x = 0;
                    spawnPosition.z += offsetYrange + spacing + maxZ;
                    maxZ = 0;
                }
            }
            else
            {
                Debug.LogWarning("No collider found in the child of the prefab: " + prefab.name);
            }
            objWasSpawn.Add(spawnedObject);
        }
    }
#endif
}
