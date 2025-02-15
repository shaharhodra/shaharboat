using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("����� �����")]
    public float forwardSpeed = 5f;
    public float turnSpeed = 50f;
    public float tiltAmount = 15f;

    [Header("���� �������")]
    public float waveAmplitude = 0.5f;
    public float waveFrequency = 1f;

    [Header("��� �����")]
    public float swipeSensitivity = 0.05f;
    public float steeringSmooth = 5f;

    [Header("���� ����� � ������ ����� ������")]
    [Tooltip("���� �������� ����� Yaw ����� (��� ������� ����, ���� �����)")]
    public float extraYawMultiplier = 10f;
    [Tooltip("��� ����� ����� �-Yaw (����� ���, ���� ����� ���)")]
    public float extraYawDecayRate = 0.5f;
    [Tooltip("����� ����� �-Pitch (����� ����� ����� ����)")]
    public float extraPitchAmplitude = 3f;
    [Tooltip("������ ����� �-Pitch")]
    public float extraPitchFrequency = 2f;

    // ������ �������
    private float initialY;
    private float currentHeading = 0f;
    private float currentTilt = 0f;
    private float currentSteering = 0f;

    // ����� �������� � ����� �� ����� �-extra yaw
    private float inertialExtraYaw = 0f;

    // ������ ������ ���� ������
    private Vector2 touchStartPos;
    private bool isSwiping = false;

    void Start()
    {
        initialY = transform.position.y;
        currentHeading = transform.eulerAngles.y;
    }

    void Update()
    {
        // ����� �����
         transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // ������ � ����� ���� ����� ����
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        Vector3 pos = transform.position;
        pos.y = initialY + waveOffset;
        transform.position = pos;

        // ����� ���� �����
        float targetSteering = 0f;
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
                targetSteering = delta.x * swipeSensitivity;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isSwiping = false;
                targetSteering = 0f;
            }
        }
        else
        {
            targetSteering = 0f;
        }

        // ����� ���� �� ��� ������ � ���� ���
        currentSteering = Mathf.Lerp(currentSteering, targetSteering, Time.deltaTime * steeringSmooth);

        // ����� ������ ������ �� ����� (Heading �-Tilt)
        currentHeading += currentSteering * turnSpeed * Time.deltaTime;
        currentTilt = Mathf.Lerp(currentTilt, -currentSteering * tiltAmount, Time.deltaTime * turnSpeed);
        Quaternion baseRotation = Quaternion.Euler(0, currentHeading, currentTilt);

        // ����� �������� � ���� �� ���, ������� ���� extra yaw; ����� ���, ���� ����� �������
        if (Mathf.Abs(targetSteering) > 0.001f)
        {
            inertialExtraYaw += currentSteering * extraYawMultiplier * Time.deltaTime;
        }
        else
        {
            inertialExtraYaw = Mathf.Lerp(inertialExtraYaw, 0, extraYawDecayRate * Time.deltaTime);
        }

        // ����� Pitch ����� ����� ����� �� ���� (���� �����)
        float extraPitch = Mathf.Sin(Time.time * extraPitchFrequency) * extraPitchAmplitude;

        // ����� ��������: ������ ������ + extra yaw (�� �������) + ����� pitch
        Vector3 finalEuler = baseRotation.eulerAngles;
        finalEuler.y += inertialExtraYaw;
        finalEuler.x += extraPitch;

        transform.rotation = Quaternion.Euler(finalEuler);
    }
}
