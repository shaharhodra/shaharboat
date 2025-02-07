using UnityEngine;

public class KeepScreenAwake : MonoBehaviour
{
    void Start()
    {
        // ���� ����� ������ ���� ����
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // ���� ������ �� OnDestroy ��� ������ �� �������� ������ ����� ����� ������:
    void OnDestroy()
    {
        // ����� �� ������ ������ ����� (����� ������ �� ����� �����)
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }
}
