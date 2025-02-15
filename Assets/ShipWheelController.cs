using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShipWheelController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Ship Wheel UI")]
    public Image shipWheelImage; // ����� ��� ������ (UI)
    [Tooltip("������ ������ ��� �����")]
    public float steeringSensitivity = 0.5f;
    [Tooltip("������ ����� ����� (������ ������) ���� ������ ����� �����")]
    public float steeringReturnSpeed = 180f;

    // ����� ������ ������ ������ (������)
    private float currentRotation = 0f;
    // ��� ������ ���� ���� �����?
    private bool isDragging = false;

    private void Update()
    {
        if (!isDragging)
        {
            // ���� ��� �����, ����� ������ �� ������ ���� ����
            currentRotation = Mathf.MoveTowards(currentRotation, 0, steeringReturnSpeed * Time.deltaTime);
            UpdateWheelRotation();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ����� �� ������ ��� ����� ������ �������
        currentRotation -= eventData.delta.x * steeringSensitivity;
        UpdateWheelRotation();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void UpdateWheelRotation()
    {
        if (shipWheelImage != null)
        {
            shipWheelImage.rectTransform.localRotation = Quaternion.Euler(0, 0, currentRotation);
        }
    }
}
