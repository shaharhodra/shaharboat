using UnityEngine;
using System.Collections.Generic;

public class DayNightCycle : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Color dayAmbientColor = Color.white;
    [SerializeField] private Color nightAmbientColor = Color.black;
    [SerializeField] private float dayLightIntensity = 1f;
    [SerializeField] private float nightLightIntensity = 0.2f;

    [Header("Skybox Settings")]
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material nightSkybox;

    [Header("Layers")]
    [SerializeField] private LayerMask dayLayerMask;        // ����� ������ ����
    [SerializeField] private LayerMask nightLayerMask;      // ����� ������ �����

    [Header("Dynamic Layer Object")]
    [SerializeField] private GameObject dynamicLayerObject; // ������� ����� ���� ���� ������
    [SerializeField] private string dayLayerName = "Default";  // �� ���� ����
    [SerializeField] private string nightLayerName = "NightOnlyLayer"; // �� ���� �����

    private bool isDay = true;
    private const string DayNightKey = "DayNightState";

    private void Start()
    {
        LoadDayNightState();
        UpdateEnvironment();
    }

    private void Update()
    {
        // �� ���� ������� �� ����� �������
        UpdateEnvironment();
    }

    private void UpdateEnvironment()
    {
        if (directionalLight == null || daySkybox == null || nightSkybox == null)
        {
            Debug.LogError("One or more required components are not assigned in the Inspector!");
            return;
        }

        // ����� Skybox ���� ���� ��������
        RenderSettings.skybox = isDay ? daySkybox : nightSkybox;
        RenderSettings.ambientLight = isDay ? dayAmbientColor : nightAmbientColor;

        // ����� ����� ������ - �� ��� �� ���� ����� �� ����� ������, ���� ������ �� ��
        directionalLight.intensity = isDay ? dayLightIntensity : nightLightIntensity;

        // ����� ����� �-Culling �� ������
        UpdateCameraCullingMask();

        // ����� ���� �������� ������
        UpdateDynamicLayerObject();
    }

    private void UpdateCameraCullingMask()
    {
        if (Camera.main != null)
        {
            Camera.main.cullingMask = isDay ? dayLayerMask : nightLayerMask;
            Debug.Log(isDay ? "Day Mode: Showing DayLayerMask" : "Night Mode: Showing NightLayerMask");
        }
        else
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    private void UpdateDynamicLayerObject()
    {
        if (dynamicLayerObject != null)
        {
            string targetLayerName = isDay ? dayLayerName : nightLayerName;
            dynamicLayerObject.layer = LayerMask.NameToLayer(targetLayerName);
            Debug.Log($"Dynamic Layer Object set to: {targetLayerName}");
        }
    }

    public void ToggleDayNight()
    {
        isDay = !isDay;
        UpdateEnvironment();
        SaveDayNightState();
    }

    private void SaveDayNightState()
    {
        PlayerPrefs.SetInt(DayNightKey, isDay ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadDayNightState()
    {
        if (PlayerPrefs.HasKey(DayNightKey))
        {
            isDay = PlayerPrefs.GetInt(DayNightKey) == 1;
        }
    }

    public bool IsDay()
    {
        return isDay;
    }
}
