using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("הגדרות תנועה")]
    [Tooltip("מהירות תנועה קדימה")]
    public float forwardSpeed = 5f;
    [Tooltip("מהירות סיבוב (מעלות לשנייה)")]
    public float turnSpeed = 50f;
    [Tooltip("הטיית הסירה המקסימלית (במעלות)")]
    public float tiltAmount = 15f;

    [Header("הגדרות גל")]
    [Tooltip("עוצמת התנועה האנכית (בובינג)")]
    public float waveAmplitude = 0.5f;
    [Tooltip("תדירות הגלים")]
    public float waveFrequency = 1f;

    [Header("הגדרות סוויפ")]
    [Tooltip("רגישות הסוויפ (כפול ערך הזזת אצבע בפיקסלים)")]
    public float swipeSensitivity = 0.05f;

    // משתנים פנימיים
    private float initialY;            // הגובה ההתחלתי של הסירה
    private float currentHeading = 0f; // הזווית הנוכחית (סיבוב סביב ציר Y)
    private float currentTilt = 0f;    // זווית ההטיה (סיבוב סביב ציר Z)
    private float steering = 0f;       // ערך הנגזרת של תנועת הסוויפ

    // משתנים לזיהוי סוויפ במובייל
    private Vector2 touchStartPos;
    private bool isSwiping = false;

    void Start()
    {
        // שמירת הגובה ההתחלתי של הסירה
        initialY = transform.position.y;
        currentHeading = transform.eulerAngles.y;
    }

    void Update()
    {
        // תנועה קדימה – הסירה זזה בכיוון החזיתי שלה
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // תנועת "בובינג" – שינוי אנכי לפי פונקציית סינוס
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        Vector3 pos = transform.position;
        pos.y = initialY + waveOffset;
        transform.position = pos;

        // טיפול בקלט סוויפ בטלפון (או במצב Editor ניתן להוסיף גם קלט עכבר/מקלדת במידת הצורך)
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
                // שימוש בתנועה אופקית כדי לקבוע את ערך הסוויפ
                steering = delta.x * swipeSensitivity;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isSwiping = false;
                steering = 0f;
            }
        }
        else
        {
            // במידה ואין קלט, מאפסים בהדרגה את ערך הסוויפ
            steering = Mathf.Lerp(steering, 0, Time.deltaTime * 5f);
        }

        // עדכון כיוון הסירה (סיבוב סביב ציר Y) בהתאם לסוויפ
        currentHeading += steering * turnSpeed * Time.deltaTime;
        // עדכון הטיית הסירה (סיבוב סביב ציר Z) – כך שהסירה תטה לצד הפוך לכיוון הסוויפ
        currentTilt = Mathf.Lerp(currentTilt, -steering * tiltAmount, Time.deltaTime * turnSpeed);

        // עדכון הסיבוב הכולל של הסירה: שומר על סיבוב סביב ציר Y והטיה סביב ציר Z
        transform.rotation = Quaternion.Euler(0, currentHeading, currentTilt);
    }
}
