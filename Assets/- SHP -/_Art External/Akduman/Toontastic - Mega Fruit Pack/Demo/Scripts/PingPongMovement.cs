using UnityEngine;

public class PingPongMovement : MonoBehaviour
{
    public Vector3 startPoint = new Vector3(0, 0, 0); // Başlangıç noktası
    public Vector3 endPoint = new Vector3(0, 0, 10);  // Bitiş noktası
    public float speed = 2.0f; // Hareket hızı

    void Update()
    {
        // Zamanı hesaplayarak objeyi startPoint ve endPoint arasında hareket ettir
        float t = Mathf.PingPong(Time.time * speed, 1.0f);
        transform.position = Vector3.Lerp(startPoint, endPoint, t);
    }
}