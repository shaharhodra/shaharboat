using UnityEngine;

public class KeepScreenAwake : MonoBehaviour
{
    void Start()
    {
        // מונע מהמסך להיכנס למצב שינה
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // אפשר להוסיף גם OnDestroy כדי להחזיר את ההתנהגות לברירת המחדל במידה ורוצים:
    void OnDestroy()
    {
        // מחזיר את ההגדרה לברירת המחדל (מערכת אחראית על ניהול השינה)
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }
}
