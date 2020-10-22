using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject[] m_Terrains;

    public void SetupTerrain(int type)
    {
        for (int i = 0; i < m_Terrains.Length; i++)
        {
            m_Terrains[i].SetActive(i == type);
        }
    }
}
