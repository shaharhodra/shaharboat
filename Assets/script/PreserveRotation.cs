using UnityEngine;

public class PreserveRotation : MonoBehaviour
{
    // נשמור את הזוויות ההתחלתיות של הילד (pitch ו-roll)
    private float initialPitch;
    private float initialRoll;
    // נשמור גם את הזווית הבסיסית של yaw יחסית להורה
    private float initialYawOffset;

    void Start()
    {
        // קולט את הסיבוב המקומי בהתחלה
        Vector3 localEuler = transform.localEulerAngles;
        initialPitch = localEuler.x;
        initialRoll = localEuler.z;
        initialYawOffset = localEuler.y;
    }

    void LateUpdate()
    {
        if (transform.parent != null)
        {
            // נרצה שהילד יקבל את ה-yaw של ההורה בלבד
            float parentYaw = transform.parent.eulerAngles.y;
            // נרכיב סיבוב חדש: pitch ו-roll קבועים מההתחלה, ו-yaw מבוסס על ההורה בתוספת ההפרש ההתחלתי
            Quaternion fixedRotation = Quaternion.Euler(initialPitch, parentYaw + initialYawOffset, initialRoll);
            transform.rotation = fixedRotation;
        }
    }
}
