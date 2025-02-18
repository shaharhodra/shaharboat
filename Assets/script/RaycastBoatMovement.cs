using UnityEngine;

public class RaycastBoatMovement : MonoBehaviour
{
    [Header("������ ���� Raycast")]
    [Tooltip("���� ���� Ray ������")]
    public int numberOfRays = 5;
    [Tooltip("���� ������� ������ (������)")]
    public float angleSpread = 30f;
    [Tooltip("���� ����")]
    public float rayDistance = 10f;

    [Header("��������� ������ (��������)")]
    [Tooltip("����� �� �������� � �� ��������� ��� ������ �� ������")]
    public GameObject[] allowedPrefabs;

    [Header("����� �����")]
    [Tooltip("������ ������ �����")]
    public float forwardSpeed = 5f;

    [Header("������ ����� (Steering) ��� ��� ����")]
    [Tooltip("������ ��������� ������ ��� ������� (������)")]
    public float maxSteeringAngle = 45f;
    [Tooltip("������ ����� ������ ��� ������� (����� ������)")]
    public float steeringChangeSpeed = 90f;
    [Tooltip("������ ����� ������ �������� ���� ��� ������� (����� ������)")]
    public float steeringReturnSpeed = 60f;

    // ����� ������ �� ������ ������� �� �-steering (������)
    private float currentSteeringAngle = 0f;

    void Update()
    {
        bool hitDetected = false;
        float closestDistance = rayDistance;
        RaycastHit closestHit = new RaycastHit();

        // 1. ����� ����� �� ������ ������ �������
        float startAngle = -angleSpread / 2f;
        float angleIncrement = (numberOfRays > 1) ? angleSpread / (numberOfRays - 1) : 0f;

        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = startAngle + i * angleIncrement;
            // ����� ����� ����: ����� �� transform.forward ���� ��� �-Y ������ angle
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            // ���� ���� �-Debug (��� ����)
            Debug.DrawRay(transform.position, direction * rayDistance, Color.red);

            // ����� Raycast ���� ����� �������
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, rayDistance, ~0, QueryTriggerInteraction.Collide))
            {
             
                    Debug.Log("��� ���� ��������: " + hit.collider.gameObject.name);
                    // ���� ����...
                

                // ����� ��� �������� ������ �� ���� ���� ���������� �������
                bool considerHit = false;
                if (allowedPrefabs != null && allowedPrefabs.Length > 0)
                {
                    foreach (GameObject prefab in allowedPrefabs)
                    {
                        // ��� ������ ������ ��� �� � ���� ���� �� ����� ����� ��� ���� �� ��� �� �������
                        if (hit.collider.gameObject.name.Contains(prefab.name))
                        {
                            considerHit = true;
                            break;
                        }
                    }
                }
                // �� �� ����� ����, �� ������ ��������
                // (���� ����� ������� �� ��� �����)

                if (considerHit && hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    closestHit = hit;
                    hitDetected = true;
                }
            }
        }

        // 2. ����� ������ ������ (target steering) ����� ������ ����� ������ ����� ������
        float targetSteeringAngle = 0f; // ����� ����: ��� ����� (0 �����)
        if (hitDetected)
        {
            // ���� ����� ������ ����� ������ �� �����
            Vector3 localHitPoint = transform.InverseTransformPoint(closestHit.point);
            // �� ����� ������ ����� ����� (x �����) � ���� ����� (������),
            // ��� ����� � ���� ������ (������)
            if (localHitPoint.x > 0f)
                targetSteeringAngle = -maxSteeringAngle;
            else if (localHitPoint.x < 0f)
                targetSteeringAngle = maxSteeringAngle;
        }

        // 3. ����� ������ �� ������ ������� ������ ������ ������
        if (hitDetected)
            currentSteeringAngle = Mathf.MoveTowards(currentSteeringAngle, targetSteeringAngle, steeringChangeSpeed * Time.deltaTime);
        else
            currentSteeringAngle = Mathf.MoveTowards(currentSteeringAngle, 0f, steeringReturnSpeed * Time.deltaTime);

        // 4. ����� ������ (������)
        transform.Rotate(0, currentSteeringAngle * Time.deltaTime, 0, Space.World);

        // 5. ����� ����� �����
        transform.position += transform.forward * forwardSpeed * Time.deltaTime;
    }
}
