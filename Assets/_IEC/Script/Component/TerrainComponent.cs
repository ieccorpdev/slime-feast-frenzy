using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainComponent : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer m_Mesh;

    [SerializeField]
    private GameObject m_LockObj;

    [SerializeField]
    private Transform m_TransStartPos1, m_TransEndPos1, m_TransStartPos2, m_TransEndPos2;

    public void SetupTerrain(Material mat)
    {
        m_Mesh.material = mat;
    }

    public void SetActiveLock(bool isActive)
    {
        m_LockObj.SetActive(isActive);
    }

    public void SetArrowPos(Vector3 m_StartPos1, Vector3 m_EndPos1, Vector3 m_StartPos2, Vector3 m_EndPos2)
    {
        m_TransStartPos1.localPosition = GetLocalPos(m_StartPos1);
        m_TransEndPos1.localPosition = GetLocalPos(m_EndPos1);
        m_TransStartPos2.localPosition = GetLocalPos(m_StartPos2);
        m_TransEndPos2.localPosition = GetLocalPos(m_EndPos2);
    }

    private Vector3 GetLocalPos(Vector3 posInput)
    {
        var pos = transform.InverseTransformPoint(posInput);
        pos.y = 0f;
        return pos;
    }
}
