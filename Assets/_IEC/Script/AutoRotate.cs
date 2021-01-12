using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 Direction = Vector3.forward;
    public float Speed = 100;
    Coroutine m_RotateCorou;

    IEnumerator IE_Rotate()
    {
        Direction = new Vector3(0, 0, -1);
        var update = new WaitForSecondsRealtime(0.02f);
        while (true)
        {
            transform.Rotate(Direction, 0.02f * Speed);
            yield return update;
        }
    }

    private void OnEnable()
    {
        if (m_RotateCorou != null)
        {
            StopCoroutine(m_RotateCorou);
        }
        m_RotateCorou = StartCoroutine(IE_Rotate());
    }

    private void OnDisable()
    {
        if (m_RotateCorou != null)
        {
            StopCoroutine(m_RotateCorou);
        }
    }
}
