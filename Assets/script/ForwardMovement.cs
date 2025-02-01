using UnityEngine;

public class ForwardMovement : MonoBehaviour
{
    public float speed = 5f; // מהירות התנועה

    void Update()
    {
        // תנועת דמות קדימה באופן אוטומטי
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
