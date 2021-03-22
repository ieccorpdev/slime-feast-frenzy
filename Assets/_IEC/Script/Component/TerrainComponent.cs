using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainComponent : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer m_Mesh;

    [SerializeField]
    private GameObject m_LockObj;

    public void SetupTerrain(Material mat)
    {
        m_Mesh.material = mat;
    }

    public void SetActiveLock(bool isActive)
    {
        m_LockObj.SetActive(isActive);
    }
}
