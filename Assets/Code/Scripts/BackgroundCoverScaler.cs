using UnityEngine;

public class BackgroundCoverScaler : MonoBehaviour
{
    [SerializeField] private float referenceWidth = 1920f;
    [SerializeField] private float referenceHeight = 1080f;

    private RectTransform rectTransform;
    private RectTransform parentRect;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRect = rectTransform.parent as RectTransform;

        UpdateScale();
    }

    private void OnEnable()
    {
        UpdateScale();
    }

    private void Update()
    {
        UpdateScale();
    }

    private void UpdateScale()
    {
        if (parentRect == null)
            return;

        float widthScale =
            parentRect.rect.width / referenceWidth;

        float heightScale =
            parentRect.rect.height / referenceHeight;

        // Use whichever scale is larger so the artwork
        // always covers the whole screen.
        float scale =
            Mathf.Max(widthScale, heightScale);

        rectTransform.localScale =
            new Vector3(scale, scale, 1f);
    }
}