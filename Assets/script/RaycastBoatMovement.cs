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

    [Header("��������� ������ (���������)")]
    [Tooltip("���� ���������� ������ ������ ������. �� ����� ��� � ������ ��� ����������.")]
    public Collider[] targetColliders;

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
    // ������: -45 �� 45 ���� 0 ���� ���� ����� ������ "���"
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
                Debug.Log($"{gameObject.name} - ��� ����� ��������: {hit.collider.name} (isTrigger: {hit.collider.isTrigger})");

                bool considerHit = false;
                // �� ����� ���� targetColliders, ����� ��� �������� ����� ��
                if (targetColliders != null && targetColliders.Length > 0)
                {
                    foreach (Collider col in targetColliders)
                    {
                        if (hit.collider == col)
                        {
                            considerHit = true;
                            break;
                        }
                    }
                }
                else
                {
                    // �� �� ������ ��������� �������� � ������ ��� ����������
                    considerHit = true;
                }

                // �� ����� ������ � ������ ���� �� �� ���� ����� (���� ����� ��� ����� �����)
                if (hit.collider.isTrigger)
                    considerHit = true;

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
            // ���� ����� ������ ����� ������ �� �������� (����, �����)
            Vector3 localHitPoint = transform.InverseTransformPoint(closestHit.point);
            Debug.Log($"{gameObject.name} - ����� ������ �������: {localHitPoint}");

            // �� ����� ������ ����� ����� (x �����) � ���� ����� (������),
            // ��� ����� � ���� ������ (������)
            if (localHitPoint.x > 0f)
                targetSteeringAngle = -maxSteeringAngle;
            else if (localHitPoint.x < 0f)
                targetSteeringAngle = maxSteeringAngle;

            Debug.Log($"{gameObject.name} - ������� �����. ����� ������� �����: {targetSteeringAngle} �����");
        }

        // 3. ����� ������ �� ������ ������� ������ ������ ������
        // ���� �� ������� � ����� ������� ����� ������,
        // ����� ��� � ����� ������ ������ 0.
        if (hitDetected)
        {
            currentSteeringAngle = Mathf.MoveTowards(currentSteeringAngle, targetSteeringAngle, steeringChangeSpeed * Time.deltaTime);
        }
        else
        {
            currentSteeringAngle = Mathf.MoveTowards(currentSteeringAngle, 0f, steeringReturnSpeed * Time.deltaTime);
        }

        // 4. ����� ������ (������) � ��� ����� ������ ���� ��� �-Y ������� ����� ������ �-steering �������.
        // ������: �� currentSteeringAngle = 45, ����� ������ �-45 ����� ������.
        transform.Rotate(0, currentSteeringAngle * Time.deltaTime, 0, Space.World);
        Debug.Log($"{gameObject.name} - ����� �����: {currentSteeringAngle * Time.deltaTime} �����");

        // 5. ����� ����� ����� � ��� ����� ���� �� ���� ������ ����� ������ ���.
        transform.position += transform.forward * forwardSpeed * Time.deltaTime;
    }
}
