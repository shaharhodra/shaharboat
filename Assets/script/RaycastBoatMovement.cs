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

    [Header("אובייקטים מורשים (פריפאבים)")]
    [Tooltip("רשימה של פריפאבים – רק אובייקטים אלו ישפיעו על התזוזה")]
    public GameObject[] allowedPrefabs;

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
             
                    Debug.Log("קרן פגעה באובייקט: " + hit.collider.gameObject.name);
                    // המשך הקוד...
                

                // בדיקה האם האובייקט שפגענו בו שייך לאחד מהפריפאבים המורשים
                bool considerHit = false;
                if (allowedPrefabs != null && allowedPrefabs.Length > 0)
                {
                    foreach (GameObject prefab in allowedPrefabs)
                    {
                        // כאן מבצעים השוואה לפי שם – ודאו שהשם של מופעי הסצנה אכן מכיל את השם של הפריפאב
                        if (hit.collider.gameObject.name.Contains(prefab.name))
                        {
                            considerHit = true;
                            break;
                        }
                    }
                }
                // אם לא מוגדר מערך, לא נתייחס לאובייקט
                // (אפשר לשנות התנהגות זו לפי הצורך)

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
            // המרת נקודת הפגיעה למרחב המקומי של הסירה
            Vector3 localHitPoint = transform.InverseTransformPoint(closestHit.point);
            // אם נקודת הפגיעה נמצאת מימין (x חיובי) – נסטה שמאלה (שלילית),
            // ואם משמאל – נסטה לימינה (חיובית)
            if (localHitPoint.x > 0f)
                targetSteeringAngle = -maxSteeringAngle;
            else if (localHitPoint.x < 0f)
                targetSteeringAngle = maxSteeringAngle;
        }

        // 3. שינוי הדרגתי של הזווית הנוכחית לכיוון הזווית הרצויה
        if (hitDetected)
            currentSteeringAngle = Mathf.MoveTowards(currentSteeringAngle, targetSteeringAngle, steeringChangeSpeed * Time.deltaTime);
        else
            currentSteeringAngle = Mathf.MoveTowards(currentSteeringAngle, 0f, steeringReturnSpeed * Time.deltaTime);

        // 4. יישום הסיבוב (התמרון)
        transform.Rotate(0, currentSteeringAngle * Time.deltaTime, 0, Space.World);

        // 5. תנועת הסירה קדימה
        transform.position += transform.forward * forwardSpeed * Time.deltaTime;
    }
}
