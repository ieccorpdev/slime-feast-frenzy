using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TerrainComponent : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer m_Mesh;

    [SerializeField]
    private GameObject m_LockObj;

    [SerializeField]
    private Transform[] m_TransArrows;

    private bool m_IsTweenArrow;

    public void SetupTerrain(Material mat)
    {
        m_Mesh.material = mat;
    }

    public void SetActiveLock(bool isActive)
    {
        m_LockObj.SetActive(isActive);
    }

    private Vector3 m_StartPos1, m_EndPos1, m_StartPos2, m_EndPos2;

    private Vector3[] arrayPos = new Vector3[3];

    private List<Vector3> listPos = new List<Vector3>();

    public void SetArrowPos(Vector3 startPos1, Vector3 endPos1, Vector3 startPos2, Vector3 endPos2)
    {
        listPos.Clear();

        m_StartPos1 = GetLocalPos(startPos1);
        m_EndPos1 = GetLocalPos(endPos1);
        m_StartPos2 = GetLocalPos(startPos2);
        m_EndPos2 = GetLocalPos(endPos2);

        arrayPos[0] = m_EndPos1;
        arrayPos[1] = m_StartPos2;
        arrayPos[2] = m_EndPos2;

        //StartCoroutine(IETweenArrow());
    }

    private int arrowIndex;
    private IEnumerator IETweenArrow()
    {
        m_IsTweenArrow = true;
        while (m_IsTweenArrow)
        {
            SpawnArrow(arrowIndex);

            arrowIndex += 1;
            if(arrowIndex == m_TransArrows.Length)
            {
                arrowIndex = 0;
            }

            yield return new WaitForSeconds(.75f);
        }
    }

    private void SpawnArrow(int index)
    {
        var transArrow = m_TransArrows[index];
        transArrow.gameObject.SetActive(true);
        transArrow.localPosition = m_StartPos1;

        transArrow.DOLocalPath(arrayPos, 2f, PathType.CatmullRom, PathMode.Full3D).SetLookAt(.01f).SetEase(Ease.Linear)
            .OnComplete(() => {
                transArrow.gameObject.SetActive(false);
            });
    }

    private Vector3 GetLocalPos(Vector3 posInput)
    {
        var pos = transform.InverseTransformPoint(posInput);
        pos.y = 0.015f;
        return pos;
    }

    private Tween TweenArrowItem(Transform transArrow, Vector3 startPos, Vector3 endPos, float delay)
    {
        transArrow.localPosition = startPos;
        transArrow.gameObject.SetActive(true);

        return transArrow.DOLocalMove(endPos, .5f).SetDelay(delay).OnComplete(() => {
            transArrow.gameObject.SetActive(false);
        });
    }

    public void OnKillTween()
    {
        m_IsTweenArrow = false;

        for (int i = 0; i < m_TransArrows.Length; i++)
        {
            m_TransArrows[i].gameObject.SetActive(false);
        }
    }
}
