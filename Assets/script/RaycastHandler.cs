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
    public float imageDisplayDistance = 7f; // ✅ מרחק הצגת תמונה (ניתן לשינוי ב-Inspector)
    public float flashTriggerDistance = 4f; // ✅ מרחק שבו הפלאש יופעל (ניתן לשינוי)

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

    private Image currentImage;
    private HashSet<string> uniqueColliders = new HashSet<string>();

    private void Start()
    {
        foreach (var pair in colliderImagePairs)
        {
            if (pair != null && pair.image != null)
            {
                colliderImageMap[pair.colliderName] = pair;
                pair.image.enabled = false; // ✅ ודא שהתמונות מוסתרות בהתחלה
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
                string hitName = hit.collider.gameObject.name;
                float hitDistance = hit.distance;

                if (colliderImageMap.ContainsKey(hitName))
                {
                    if (hitDistance <= imageDisplayDistance) // ✅ הצגת תמונה לפי מרחק
                    {
                        ShowImage(hitName);
                    }
                    else
                    {
                        HideCurrentImage();
                    }

                    if (hitDistance <= flashTriggerDistance && colliderImageMap[hitName].enableFlash) // ✅ הפעלת פלאש רק אם קרוב מספיק
                    {
                        if (!isFlashing)
                        {
                            StartCoroutine(FlashEffect());
                        }
                    }
                }

                if (hitDistance <= penaltyDisplayDuration) // ✅ פסילה אם המרחק קטן מסף הפסילה
                {
                    HandlePenalty(hitName);
                }

                return;
            }
        }

        if (!hitSomething)
        {
            ResetFlashEffect();
            HideCurrentImage();
        }
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
            currentImage.enabled = true; // ✅ הצגת התמונה רק אם קרובים מספיק
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

    private void HandlePenalty(string colliderName)
    {
        if (uniqueColliders.Add(colliderName))
        {
            StartCoroutine(ShowPenaltyBackground());
            UpdatePenaltyText();
        }
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

    private void UpdatePenaltyText()
    {
        if (penaltyText != null && penaltyTextPanel.activeSelf)
        {
            penaltyText.text = string.Join("\n", uniqueColliders);
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