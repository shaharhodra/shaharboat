using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("תנועת הסירה")]
    public float forwardSpeed = 5f;
    public float turnSpeed = 50f;
    public float tiltAmount = 15f;

    [Header("גלים לבובינג")]
    public float waveAmplitude = 0.5f;
    public float waveFrequency = 1f;

    [Header("קלט סוויפ")]
    public float swipeSensitivity = 0.05f;
    public float steeringSmooth = 5f;

    [Header("אפקט חרטום – תנועות קטנות נוספות")]
    [Tooltip("מקדם להצטברות תנועת Yaw מהקלט (ככל שהמשתמש מזיז, הערך מצטבר)")]
    public float extraYawMultiplier = 10f;
    [Tooltip("קצב דעיכת תנועת ה-Yaw (כשאין מגע, הערך מתמעט לאט)")]
    public float extraYawDecayRate = 0.5f;
    [Tooltip("עוצמת תנודת ה-Pitch (תנועה עדינה המדמה גלים)")]
    public float extraPitchAmplitude = 3f;
    [Tooltip("תדירות תנודת ה-Pitch")]
    public float extraPitchFrequency = 2f;

    // משתנים פנימיים
    private float initialY;
    private float currentHeading = 0f;
    private float currentTilt = 0f;
    private float currentSteering = 0f;

    // משתנה לאינרציה – מצטבר את תנועת ה-extra yaw
    private float inertialExtraYaw = 0f;

    // משתנים לטיפול בקלט מובייל
    private Vector2 touchStartPos;
    private bool isSwiping = false;

    void Start()
    {
        initialY = transform.position.y;
        currentHeading = transform.eulerAngles.y;
    }

    void Update()
    {
        // תנועה קדימה
         transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // בובינג – שינוי אנכי המדמה גלים
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        Vector3 pos = transform.position;
        pos.y = initialY + waveOffset;
        transform.position = pos;

        // טיפול בקלט סוויפ
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

        // עדכון חלקי של קלט הסוויפ – מעבר חלק
        currentSteering = Mathf.Lerp(currentSteering, targetSteering, Time.deltaTime * steeringSmooth);

        // עדכון הסיבוב הבסיסי של הסירה (Heading ו-Tilt)
        currentHeading += currentSteering * turnSpeed * Time.deltaTime;
        currentTilt = Mathf.Lerp(currentTilt, -currentSteering * tiltAmount, Time.deltaTime * turnSpeed);
        Quaternion baseRotation = Quaternion.Euler(0, currentHeading, currentTilt);

        // עדכון האינרציה – כאשר יש קלט, מצטברים ערכי extra yaw; כשאין קלט, הערך מתמעט באיטיות
        if (Mathf.Abs(targetSteering) > 0.001f)
        {
            inertialExtraYaw += currentSteering * extraYawMultiplier * Time.deltaTime;
        }
        else
        {
            inertialExtraYaw = Mathf.Lerp(inertialExtraYaw, 0, extraYawDecayRate * Time.deltaTime);
        }

        // תנודת Pitch עדינה המדמה תנועה של גלים (אפקט חרטום)
        float extraPitch = Mathf.Sin(Time.time * extraPitchFrequency) * extraPitchAmplitude;

        // שילוב הסיבובים: הסיבוב הבסיסי + extra yaw (עם אינרציה) + תנודת pitch
        Vector3 finalEuler = baseRotation.eulerAngles;
        finalEuler.y += inertialExtraYaw;
        finalEuler.x += extraPitch;

        transform.rotation = Quaternion.Euler(finalEuler);
    }
}
