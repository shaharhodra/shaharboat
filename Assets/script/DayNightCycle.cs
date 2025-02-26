using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnvironmentState
{
    Day,
    Night,
    Special
}

public class DayNightCycle : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Color dayAmbientColor = Color.white;
    [SerializeField] private Color nightAmbientColor = Color.black;
    [SerializeField] private Color specialAmbientColor = Color.gray; // צבע מיוחד למצב המיוחד
    [SerializeField] private float dayLightIntensity = 1f;
    [SerializeField] private float nightLightIntensity = 0.2f;
    [SerializeField] private float specialLightIntensity = 0.5f; // עוצמת אור למצב המיוחד

    [Header("Skybox Settings")]
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material nightSkybox;
    [SerializeField] private Material specialSkybox;

    [Header("Layers")]
    [SerializeField] private LayerMask dayLayerMask;
    [SerializeField] private LayerMask nightLayerMask;
    [SerializeField] private LayerMask specialLayerMask;

    [Header("Dynamic Layer Object")]
    [SerializeField] private GameObject dynamicLayerObject;
    [SerializeField] private string dayLayerName = "Default";
    [SerializeField] private string nightLayerName = "NightOnlyLayer";
    [SerializeField] private string specialLayerName = "SpecialLayer";

    private EnvironmentState currentState = EnvironmentState.Day;

    private void Start()
    {
        UpdateEnvironment();
    }

    private void Update()
    {
        // אם אין צורך בעדכון בכל פריים, אפשר להסיר את הקריאה מה-Update
        // UpdateEnvironment();
    }

    private void UpdateEnvironment()
    {
        if (directionalLight == null || daySkybox == null || nightSkybox == null || specialSkybox == null)
        {
            Debug.LogError("One or more required components are not assigned in the Inspector!");
            return;
        }

        // עדכון ה-Skybox, צבע האור והעוצמה בהתאם למצב הנוכחי
        switch (currentState)
        {
            case EnvironmentState.Day:
                RenderSettings.skybox = daySkybox;
                RenderSettings.ambientLight = dayAmbientColor;
                directionalLight.intensity = dayLightIntensity;
                break;
            case EnvironmentState.Night:
                RenderSettings.skybox = nightSkybox;
                RenderSettings.ambientLight = nightAmbientColor;
                directionalLight.intensity = nightLightIntensity;
                break;
            case EnvironmentState.Special:
                RenderSettings.skybox = specialSkybox;
                RenderSettings.ambientLight = specialAmbientColor;
                directionalLight.intensity = specialLightIntensity;
                break;
        }

        UpdateCameraCullingMask();
        UpdateDynamicLayerObject();
    }

    private void UpdateCameraCullingMask()
    {
        if (Camera.main != null)
        {
            switch (currentState)
            {
                case EnvironmentState.Day:
                    Camera.main.cullingMask = dayLayerMask;
                    Debug.Log("Day Mode: Showing DayLayerMask");
                    break;
                case EnvironmentState.Night:
                    Camera.main.cullingMask = nightLayerMask;
                    Debug.Log("Night Mode: Showing NightLayerMask");
                    break;
                case EnvironmentState.Special:
                    Camera.main.cullingMask = specialLayerMask;
                    Debug.Log("Special Mode: Showing SpecialLayerMask");
                    break;
            }
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
            string targetLayerName = "";
            switch (currentState)
            {
                case EnvironmentState.Day:
                    targetLayerName = dayLayerName;
                    break;
                case EnvironmentState.Night:
                    targetLayerName = nightLayerName;
                    break;
                case EnvironmentState.Special:
                    targetLayerName = specialLayerName;
                    break;
            }
            dynamicLayerObject.layer = LayerMask.NameToLayer(targetLayerName);
            Debug.Log($"Dynamic Layer Object set to: {targetLayerName}");
        }
    }

    // מעבר בין מצב יום ולילה
    public void ToggleDayNight()
    {
        if (currentState == EnvironmentState.Special || currentState == EnvironmentState.Night)
            currentState = EnvironmentState.Day;
        else if (currentState == EnvironmentState.Day)
            currentState = EnvironmentState.Night;

        UpdateEnvironment();
    }


    // כפתור לעבור למצב המיוחד
    public void SwitchToSpecialMode()
    {
        currentState = EnvironmentState.Special;
        UpdateEnvironment();
    }

    public bool IsDay()
    {
        return currentState == EnvironmentState.Day;
    }
}
