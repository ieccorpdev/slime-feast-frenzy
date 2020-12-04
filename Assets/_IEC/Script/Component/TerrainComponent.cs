using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainComponent : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer m_Mesh;

    public void SetupTerrain(Material mat)
    {
        m_Mesh.material = mat;
    }
}
