using UnityEngine;

public class PreserveRotation : MonoBehaviour
{
    // ����� �� ������� ��������� �� ���� (pitch �-roll)
    private float initialPitch;
    private float initialRoll;
    // ����� �� �� ������ ������� �� yaw ����� �����
    private float initialYawOffset;

    void Start()
    {
        // ���� �� ������ ������ ������
        Vector3 localEuler = transform.localEulerAngles;
        initialPitch = localEuler.x;
        initialRoll = localEuler.z;
        initialYawOffset = localEuler.y;
    }

    void LateUpdate()
    {
        if (transform.parent != null)
        {
            // ���� ����� ���� �� �-yaw �� ����� ����
            float parentYaw = transform.parent.eulerAngles.y;
            // ����� ����� ���: pitch �-roll ������ �������, �-yaw ����� �� ����� ������ ����� �������
            Quaternion fixedRotation = Quaternion.Euler(initialPitch, parentYaw + initialYawOffset, initialRoll);
            transform.rotation = fixedRotation;
        }
    }
}
