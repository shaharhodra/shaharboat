using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShipWheelController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Ship Wheel UI")]
    public Image shipWheelImage; // אלמנט הגה הספינה (UI)
    [Tooltip("רגישות הסיבוב בעת גרירה")]
    public float steeringSensitivity = 0.5f;
    [Tooltip("מהירות החזרה למרכז (במעלות לשנייה) כאשר המשתמש מפסיק לגרור")]
    public float steeringReturnSpeed = 180f;

    // משתנה לשמירת הסיבוב הנוכחי (במעלות)
    private float currentRotation = 0f;
    // האם המשתמש נמצא במצב גרירה?
    private bool isDragging = false;

    private void Update()
    {
        if (!isDragging)
        {
            // כאשר אין גרירה, נעדכן בהדרגה את הסיבוב חזרה לאפס
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
        // נעדכן את הסיבוב לפי תנועת הגרירה האופקית
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
