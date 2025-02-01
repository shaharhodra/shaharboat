using UnityEngine;

public class RaycastBoatMovement : MonoBehaviour
{
    [Header("הגדרות קרני Raycast")]
    [Tooltip("מספר קרני Ray במניפה")]
    public int numberOfRays = 5;
    [Tooltip("טווח הזוויות במניפה (במעלות)")]
    public float angleSpread = 30f;
    [Tooltip("מרחק הקרן")]
    public float rayDistance = 10f;

    [Header("קוליידרים לבלימה (אופציונלי)")]
    [Tooltip("מערך הקוליידרים שעליהם מתבצעת הבדיקה. אם המערך ריק – נתייחס לכל ההתנגשויות.")]
    public Collider[] targetColliders;

    [Header("תנועת הסירה")]
    [Tooltip("מהירות התנועה קדימה")]
    public float forwardSpeed = 5f;

    [Header("הגדרות תמרון (Steering) כמו כלי שייט")]
    [Tooltip("הזווית המקסימלית לפנייה בעת התנגשות (במעלות)")]
    public float maxSteeringAngle = 45f;
    [Tooltip("מהירות שינוי הזווית בעת התנגשות (מעלות לשנייה)")]
    public float steeringChangeSpeed = 90f;
    [Tooltip("מהירות החזרה לזווית נייטרלית כאשר אין התנגשות (מעלות לשנייה)")]
    public float steeringReturnSpeed = 60f;

    // משתנה שמייצג את הזווית הנוכחית של ה-steering (במעלות)
    // לדוגמה: -45 עד 45 כאשר 0 אומר שאין סטייה מכיוון "ישר"
    private float currentSteeringAngle = 0f;

    void Update()
    {
        bool hitDetected = false;
        float closestDistance = rayDistance;
        RaycastHit closestHit = new RaycastHit();

        // 1. יצירת מניפה של קרניים ובדיקת התנגשות
        float startAngle = -angleSpread / 2f;
        float angleIncrement = (numberOfRays > 1) ? angleSpread / (numberOfRays - 1) : 0f;

        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = startAngle + i * angleIncrement;
            // חישוב כיוון הקרן: סיבוב של transform.forward סביב ציר ה-Y בזווית angle
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            // ציור הקרן ל-Debug (צבע אדום)
            Debug.DrawRay(transform.position, direction * rayDistance, Color.red);

            // ביצוע Raycast כולל זיהוי טריגרים
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, rayDistance, ~0, QueryTriggerInteraction.Collide))
            {
                Debug.Log($"{gameObject.name} - קרן פוגעת באובייקט: {hit.collider.name} (isTrigger: {hit.collider.isTrigger})");

                bool considerHit = false;
                // אם הוגדר מערך targetColliders, נבדוק האם האובייקט מופיע בו
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
                    // אם לא הוגדרו קוליידרים ספציפיים – נתייחס לכל ההתנגשויות
                    considerHit = true;
                }

                // אם מדובר בטריגר – נתייחס אליו גם אם אינו במערך (ניתן לשנות זאת בהתאם לצורך)
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

        // 2. חישוב הזווית הרצויה (target steering) בהתאם למיקום נקודת הפגיעה במרחב המקומי
        float targetSteeringAngle = 0f; // ברירת מחדל: אין סטייה (0 מעלות)
        if (hitDetected)
        {
            // המרת נקודת הפגיעה למרחב המקומי של האובייקט (למשל, הסירה)
            Vector3 localHitPoint = transform.InverseTransformPoint(closestHit.point);
            Debug.Log($"{gameObject.name} - נקודת הפגיעה המקומית: {localHitPoint}");

            // אם נקודת הפגיעה נמצאת מימין (x חיובי) – נסטה שמאלה (שלילית),
            // ואם משמאל – נסטה לימינה (חיובית)
            if (localHitPoint.x > 0f)
                targetSteeringAngle = -maxSteeringAngle;
            else if (localHitPoint.x < 0f)
                targetSteeringAngle = maxSteeringAngle;

            Debug.Log($"{gameObject.name} - התנגשות זוהתה. זווית סטירינג רצויה: {targetSteeringAngle} מעלות");
        }

        // 3. שינוי הדרגתי של הזווית הנוכחית לכיוון הזווית הרצויה
        // כאשר יש התנגשות – נשתמש במהירות שינוי מוגדרת,
        // וכאשר אין – נחזור בהדרגה לזווית 0.
        if (hitDetected)
        {
            currentSteeringAngle = Mathf.MoveTowards(currentSteeringAngle, targetSteeringAngle, steeringChangeSpeed * Time.deltaTime);
        }
        else
        {
            currentSteeringAngle = Mathf.MoveTowards(currentSteeringAngle, 0f, steeringReturnSpeed * Time.deltaTime);
        }

        // 4. יישום הסיבוב (התמרון) – כאן הסירה מסובבת סביב ציר ה-Y במהירות תלויה בזווית ה-steering הנוכחית.
        // לדוגמה: אם currentSteeringAngle = 45, הסירה תסתובב ב-45 מעלות לשנייה.
        transform.Rotate(0, currentSteeringAngle * Time.deltaTime, 0, Space.World);
        Debug.Log($"{gameObject.name} - סיבוב מיושם: {currentSteeringAngle * Time.deltaTime} מעלות");

        // 5. תנועת הסירה קדימה – היא תמשיך לנוע כל הזמן בכיוון הפנים הנוכחי שלה.
        transform.position += transform.forward * forwardSpeed * Time.deltaTime;
    }
}
