using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectTrajectory : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public List<GameObject> objects;
    public float height = 5f;
    public float duration = 2f;
    public float delayBetweenObjects = 0.5f;
    public float rotationSpeed = 360f;

    private bool[] objectStopped;
    private Vector3[] rotationAxes;

    void Start()
    {
        objectStopped = new bool[objects.Count];
        rotationAxes = new Vector3[objects.Count];

        for (int i = 0; i < objects.Count; i++)
        {
            rotationAxes[i] = Random.onUnitSphere;
        }

        StartCoroutine(MoveObjectsContinuously());
    }

    System.Collections.IEnumerator MoveObjectsContinuously()
    {
        while (true)
        {
            for (int i = 0; i < objectStopped.Length; i++)
            {
                objectStopped[i] = false;
            }

            for (int i = 0; i < objects.Count; i++)
            {
                GameObject obj = objects[i];
                Vector3 startPos = (i % 2 == 0) ? pointA.position : pointB.position;
                Vector3 endPos = (i % 2 == 0) ? pointB.position : pointA.position;

                StartCoroutine(MoveObject(obj, startPos, endPos, height, duration, i));
                yield return new WaitForSeconds(delayBetweenObjects);
            }

            yield return new WaitUntil(() => AllObjectsStopped());

            yield return new WaitForSeconds(1f); 
        }
    }

    bool AllObjectsStopped()
    {
        foreach (bool stopped in objectStopped)
        {
            if (!stopped) return false;
        }
        return true;
    }

    Vector3 CalculateParabola(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs(start.y - end.y) < 0.1f)
        {
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        }
        else
        {
            Vector3 travelDirection = end - start;
            Vector3 levelDirection = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirection);
            Vector3 up = Vector3.Cross(right, travelDirection);
            if (end.y > start.y) up = -up;
            Vector3 result = start + t * travelDirection;
            result += (parabolicT * parabolicT - 1) * up.normalized * height;
            return result;
        }
    }

    System.Collections.IEnumerator MoveObject(GameObject obj, Vector3 start, Vector3 end, float height, float duration, int index)
    {
        float elapsedTime = 0f;
        Vector3 rotationAxis = rotationAxes[index];
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            obj.transform.position = CalculateParabola(start, end, height, t);

            obj.transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);
            
            yield return null;
        }
        obj.transform.position = end;
        objectStopped[index] = true;
    }
}
