using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ColliderImagePair
{
    public string colliderName;
    public Image image;
    public bool enableFlash;
}

public class RaycastHandler : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float maxRayDistance = 10f;
    public LayerMask layerMask;
    public List<float> rayAngles = new List<float> { -30f, -15f, 0f, 15f, 30f };

    [Header("Distance Settings")]
    public float imageDisplayDistance = 7f;    // מרחק להצגת התמונה
    public float flashTriggerDistance = 4f;      // מרחק שבו מופעל פלאש
    public float penaltyDistance = 5f;           // מרחק הפסילה – כאשר הפגיעה מתרחשת בטווח זה, נחשב לפגיעה

    [Header("Penalty Settings")]
    public GameObject penaltyTextPanel;
    public TextMeshProUGUI penaltyText;
    public Image penaltyBackground;
    public float penaltyDisplayDuration = 3f;  // משך הזמן בו מוצג רקע הפסילה (וכן התמונה)

    [Header("Flash Settings")]
    public Image flashImage;
    public int flashRepetitions = 3;
    public float flashDuration = 0.2f;
    private bool isFlashing = false;

    [Header("UI Elements")]
    public Button togglePenaltyTextButton;

    [Header("Ship Wheel UI")]
    public Image shipWheelImage; // אלמנט הגה הספינה

    [Header("Collider-Image Mapping")]
    public List<ColliderImagePair> colliderImagePairs = new List<ColliderImagePair>();
    private Dictionary<string, ColliderImagePair> colliderImageMap = new Dictionary<string, ColliderImagePair>();

    // משתנה לוג לפסילות: ספירת הפגיעות לכל קוליידר
    private Dictionary<string, int> penaltyLog = new Dictionary<string, int>();

    // תמונה שמוצגת ברגע זה
    private Image currentImage;

    // משתנים לניהול כיבוי קוליידרים של האובייקט שזוהה
    private GameObject lastDisabledObject = null;
    private List<Collider> disabledColliders = new List<Collider>();

    // נעילה למניעת זיהוי בזמן הפסילה (אם נדרש)
    private bool isSystemLocked = false;
    private float lockTimer = 0f;

    private void Start()
    {
        // אתחול המיפוי - הכנת המילון ומסתירים את התמונות בתחילה
        foreach (var pair in colliderImagePairs)
        {
            if (pair != null && pair.image != null)
            {
                colliderImageMap[pair.colliderName] = pair;
                pair.image.enabled = false;
            }
        }

        if (penaltyTextPanel != null)
        {
            penaltyTextPanel.SetActive(false);
        }
        if (penaltyText != null)
        {
            penaltyText.text = "";
        }
        if (togglePenaltyTextButton != null)
        {
            togglePenaltyTextButton.onClick.AddListener(TogglePenaltyPanel);
        }
        if (penaltyBackground != null)
        {
            penaltyBackground.enabled = false;
        }
        if (flashImage != null)
        {
            flashImage.enabled = false;
        }
    }

    private void Update()
    {
        // במצב נעילה (למשל, בזמן הפסילה) לא מתבצעים זיהויים חדשים
        if (isSystemLocked)
        {
            lockTimer += Time.deltaTime;
            if (lockTimer >= penaltyDisplayDuration)
            {
                isSystemLocked = false;
                lockTimer = 0f;
                // הסתרת התמונה בלבד, לא את הודעות הפסילה
                HideCurrentImage();
            }
            return;
        }

        bool hitFound = ThrowRays();

        // אם לא זוהה פגיעה, נסתר רק את תמונת הקוליידר
        if (!hitFound)
        {
            HideCurrentImage();
            ResetFlashEffect();
        }

        UpdateShipWheel();
    }

    private void UpdateShipWheel()
    {
        if (shipWheelImage != null)
        {
            float shipRotationY = transform.eulerAngles.y;
            shipWheelImage.rectTransform.localRotation = Quaternion.Euler(0, 0, -shipRotationY);
        }
    }

    /// <summary>
    /// שולחת קרני Ray בזוויות מוגדרות ומנסה לזהות פגיעה.
    /// מחזירה true אם זוהה אובייקט, אחרת false.
    /// </summary>
    private bool ThrowRays()
    {
        bool hitFound = false;
        GameObject currentHitObject = null;
        Collider currentHitCollider = null;

        foreach (float angle in rayAngles)
        {
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            Debug.DrawRay(transform.position, direction * maxRayDistance, Color.green);

            if (Physics.Raycast(ray, out hit, maxRayDistance, layerMask))
            {
                hitFound = true;
                currentHitObject = hit.collider.gameObject;
                currentHitCollider = hit.collider;

                // ניקוי שם האובייקט (הסרת "(Clone)") וחיפוש התאמה במיפוי
                Transform currentTransform = hit.collider.transform;
                string hitName = null;
                while (currentTransform != null)
                {
                    string cleanName = currentTransform.gameObject.name.Replace("(Clone)", "").Trim();
                    if (colliderImageMap.ContainsKey(cleanName))
                    {
                        hitName = cleanName;
                        break;
                    }
                    currentTransform = currentTransform.parent;
                }
                if (string.IsNullOrEmpty(hitName))
                {
                    hitName = FindMatchingInChildren(hit.collider.transform);
                }

                float hitDistance = hit.distance;

                if (!string.IsNullOrEmpty(hitName))
                {
                    // הצגת תמונה כאשר המרחק קטן מ-imageDisplayDistance
                    if (hitDistance <= imageDisplayDistance)
                    {
                        ShowImage(hitName);
                    }
                    else
                    {
                        HideCurrentImage();
                    }

                    // הפעלת פלאש כאשר המרחק קטן מ-flashTriggerDistance והרשות קיימת
                    if (hitDistance <= flashTriggerDistance && colliderImageMap[hitName].enableFlash)
                    {
                        if (!isFlashing)
                        {
                            StartCoroutine(FlashEffect());
                        }
                    }

                    // טיפול בפסילה כאשר המרחק קטן מ-penaltyDistance
                    if (hitDistance <= penaltyDistance)
                    {
                        HandlePenalty(hitName);
                        isSystemLocked = true;
                        lockTimer = 0f;
                    }
                }

                // ניהול כיבוי קוליידרים במידה והאובייקט שזוהה שונה מהקודם
                if (currentHitObject != null)
                {
                    if (lastDisabledObject != currentHitObject)
                    {
                        if (lastDisabledObject != null)
                        {
                            foreach (var col in disabledColliders)
                            {
                                if (col != null)
                                {
                                    col.enabled = true;
                                }
                            }
                            disabledColliders.Clear();
                        }
                        Collider[] cols = currentHitObject.GetComponentsInChildren<Collider>();
                        foreach (var col in cols)
                        {
                            if (col != currentHitCollider)
                            {
                                col.enabled = false;
                                disabledColliders.Add(col);
                            }
                        }
                        lastDisabledObject = currentHitObject;
                    }
                }
                else
                {
                    if (lastDisabledObject != null)
                    {
                        foreach (var col in disabledColliders)
                        {
                            if (col != null)
                            {
                                col.enabled = true;
                            }
                        }
                        disabledColliders.Clear();
                        lastDisabledObject = null;
                    }
                    HideCurrentImage();
                    ResetFlashEffect();
                }

                break; // עוצרים את הלולאה לאחר גילוי האובייקט הראשון
            }
        }
        return hitFound;
    }

    // פונקציה רקורסיבית לחיפוש התאמה באובייקטי הילדים
    private string FindMatchingInChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            string cleanName = child.gameObject.name.Replace("(Clone)", "").Trim();
            if (colliderImageMap.ContainsKey(cleanName))
            {
                return cleanName;
            }

            string result = FindMatchingInChildren(child);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
        }
        return null;
    }

    private void ShowImage(string colliderName)
    {
        // אם יש תמונה מוצגת כבר, מסתירים אותה
        if (currentImage != null)
        {
            currentImage.enabled = false;
        }
        if (colliderImageMap.ContainsKey(colliderName) && colliderImageMap[colliderName].image != null)
        {
            currentImage = colliderImageMap[colliderName].image;
            currentImage.enabled = true;
        }
    }

    private void HideCurrentImage()
    {
        if (currentImage != null)
        {
            currentImage.enabled = false;
            currentImage = null;
        }
    }

    private IEnumerator FlashEffect()
    {
        if (flashImage == null || isFlashing)
        {
            yield break;
        }
        isFlashing = true;
        for (int i = 0; i < flashRepetitions; i++)
        {
            flashImage.enabled = true;
            yield return new WaitForSeconds(flashDuration);
            flashImage.enabled = false;
            yield return new WaitForSeconds(flashDuration);
        }
        isFlashing = false;
    }

    /// <summary>
    /// מעדכן את הלוג ומציג את הודעת הפסילה עבור הקוליידר שנפגע.
    /// </summary>
    private void HandlePenalty(string colliderName)
    {
        if (penaltyLog.ContainsKey(colliderName))
        {
            penaltyLog[colliderName]++;
        }
        else
        {
            penaltyLog[colliderName] = 1;
        }
        StartCoroutine(ShowPenaltyBackground());
        UpdatePenaltyText();
    }

    private IEnumerator ShowPenaltyBackground()
    {
        if (penaltyBackground != null)
        {
            penaltyBackground.enabled = true;
            yield return new WaitForSeconds(penaltyDisplayDuration);
            penaltyBackground.enabled = false;
        }
    }

    private void UpdatePenaltyText()
    {
        if (penaltyText != null && penaltyTextPanel.activeSelf)
        {
            string logText = "";
            foreach (var entry in penaltyLog)
            {
                logText += $"{entry.Key}: {entry.Value}\n";
            }
            penaltyText.text = logText;
        }
    }

    private void TogglePenaltyPanel()
    {
        if (penaltyTextPanel != null)
        {
            bool isActive = !penaltyTextPanel.activeSelf;
            penaltyTextPanel.SetActive(isActive);
            if (!isActive && penaltyText != null)
            {
                penaltyText.text = "";
            }
            else
            {
                UpdatePenaltyText();
            }
        }
    }

    private void ResetFlashEffect()
    {
        if (flashImage != null)
        {
            flashImage.enabled = false;
        }
        isFlashing = false;
    }
}
