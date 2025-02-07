using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("������ �����")]
    [Tooltip("������ ����� �����")]
    public float forwardSpeed = 5f;
    [Tooltip("������ ����� (����� ������)")]
    public float turnSpeed = 50f;
    [Tooltip("����� ����� ��������� (������)")]
    public float tiltAmount = 15f;

    [Header("������ ��")]
    [Tooltip("����� ������ ������ (������)")]
    public float waveAmplitude = 0.5f;
    [Tooltip("������ �����")]
    public float waveFrequency = 1f;

    [Header("������ �����")]
    [Tooltip("������ ������ (���� ��� ���� ���� ��������)")]
    public float swipeSensitivity = 0.05f;

    // ������ �������
    private float initialY;            // ����� ������� �� �����
    private float currentHeading = 0f; // ������ ������� (����� ���� ��� Y)
    private float currentTilt = 0f;    // ����� ����� (����� ���� ��� Z)
    private float steering = 0f;       // ��� ������ �� ����� ������

    // ������ ������ ����� �������
    private Vector2 touchStartPos;
    private bool isSwiping = false;

    void Start()
    {
        // ����� ����� ������� �� �����
        initialY = transform.position.y;
        currentHeading = transform.eulerAngles.y;
    }

    void Update()
    {
        // ����� ����� � ����� ��� ������ ������ ���
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // ����� "������" � ����� ���� ��� �������� �����
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        Vector3 pos = transform.position;
        pos.y = initialY + waveOffset;
        transform.position = pos;

        // ����� ���� ����� ������ (�� ���� Editor ���� ������ �� ��� ����/����� ����� �����)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                isSwiping = true;
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                Vector2 delta = touch.position - touchStartPos;
                // ����� ������ ������ ��� ����� �� ��� ������
                steering = delta.x * swipeSensitivity;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isSwiping = false;
                steering = 0f;
            }
        }
        else
        {
            // ����� ���� ���, ������ ������ �� ��� ������
            steering = Mathf.Lerp(steering, 0, Time.deltaTime * 5f);
        }

        // ����� ����� ����� (����� ���� ��� Y) ����� ������
        currentHeading += steering * turnSpeed * Time.deltaTime;
        // ����� ����� ����� (����� ���� ��� Z) � �� ������ ��� ��� ���� ������ ������
        currentTilt = Mathf.Lerp(currentTilt, -steering * tiltAmount, Time.deltaTime * turnSpeed);

        // ����� ������ ����� �� �����: ���� �� ����� ���� ��� Y ����� ���� ��� Z
        transform.rotation = Quaternion.Euler(0, currentHeading, currentTilt);
    }
}
