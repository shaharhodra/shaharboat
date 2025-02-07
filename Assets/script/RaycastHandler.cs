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
    public float imageDisplayDistance = 7f;    // מרחק הצגת תמונה
    public float flashTriggerDistance = 4f;      // מרחק שבו הפלאש יופעל

    [Header("Penalty Settings")]
    public GameObject penaltyTextPanel;
    public TextMeshProUGUI penaltyText;
    public Image penaltyBackground;
    public float penaltyDisplayDuration = 3f;

    [Header("Flash Settings")]
    public Image flashImage;
    public int flashRepetitions = 3;
    public float flashDuration = 0.2f;
    private bool isFlashing = false;

    [Header("UI Elements")]
    public Button togglePenaltyTextButton;

    [Header("Collider-Image Mapping")]
    public List<ColliderImagePair> colliderImagePairs = new List<ColliderImagePair>();
    private Dictionary<string, ColliderImagePair> colliderImageMap = new Dictionary<string, ColliderImagePair>();

    // במקום HashSet – נשתמש ב־Dictionary לעדכון ספירת הפגיעות לכל קוליידר.
    private Dictionary<string, int> penaltyLog = new Dictionary<string, int>();

    private Image currentImage;

    private void Start()
    {
        // אתחול המיפוי
        foreach (var pair in colliderImagePairs)
        {
            if (pair != null && pair.image != null)
            {
                colliderImageMap[pair.colliderName] = pair;
                pair.image.enabled = false; // התחלתית – התמונות מוסתרות
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
        ThrowRays();
    }

    private void ThrowRays()
    {
        bool hitSomething = false;

        foreach (float angle in rayAngles)
        {
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            Debug.DrawRay(transform.position, direction * maxRayDistance, Color.green);

            if (Physics.Raycast(ray, out hit, maxRayDistance, layerMask))
            {
                hitSomething = true;

                // נבדוק את ההיררכיה של האובייקט שנפגע וננקה את השם מתוספת "(Clone)"
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

                // אם לא נמצא בהתבוננות כלפי מעלה, נבדוק גם בין הילדים
                if (string.IsNullOrEmpty(hitName))
                {
                    hitName = FindMatchingInChildren(hit.collider.transform);
                }

                float hitDistance = hit.distance;

                if (!string.IsNullOrEmpty(hitName))
                {
                    // הצגת תמונה לפי מרחק
                    if (hitDistance <= imageDisplayDistance)
                    {
                        ShowImage(hitName);
                    }
                    else
                    {
                        HideCurrentImage();
                    }

                    // הפעלת פלאש אם קרוב מספיק והרשאה קיימת
                    if (hitDistance <= flashTriggerDistance && colliderImageMap[hitName].enableFlash)
                    {
                        if (!isFlashing)
                        {
                            StartCoroutine(FlashEffect());
                        }
                    }

                    // טיפול בפסילה – בכל פעם שהמרחק עומד בתנאי, נעדכן את הלוג
                    if (hitDistance <= penaltyDisplayDuration)
                    {
                        HandlePenalty(hitName);
                    }
                }

                return; // עוצרים את הלולאה לאחר שמצאנו אובייקט
            }
        }

        if (!hitSomething)
        {
            ResetFlashEffect();
            HideCurrentImage();
        }
    }

    // פונקציה רקורסיבית לחיפוש התאמה גם באובייקטי הילדים
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
    /// כל פעם שמתרחשת פסילה – מעדכנים את הלוג כך שמספר הפגיעות של כל קוליידר מתעדכן,
    /// ומעדכנים את טקסט ה־UI בהתאם.
    /// </summary>
    /// <param name="colliderName">שם האובייקט שבו התרחש הפגיעה</param>
    private void HandlePenalty(string colliderName)
    {
        // עדכון הלוג – אם האובייקט כבר קיים, מגדילים את הספירה, אחרת מוסיפים אותו עם ערך התחלתי 1
        if (penaltyLog.ContainsKey(colliderName))
        {
            penaltyLog[colliderName]++;
        }
        else
        {
            penaltyLog[colliderName] = 1;
        }

        // הפעלת אנימציית רקע הפסילה (ניתן להתאמה אישית)
        StartCoroutine(ShowPenaltyBackground());
        // עדכון הטקסט ב־UI כך שיציג את מספר הפגיעות לכל קוליידר
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

    /// <summary>
    /// מעדכן את טקסט הפסילה ב־UI כך שיציג את הלוג המצטבר
    /// </summary>
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
