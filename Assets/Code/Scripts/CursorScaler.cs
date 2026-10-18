using UnityEngine;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    public RectTransform cursorRect;
    public Canvas canvas;
    public Vector2 referenceResolution = new(1920, 1080);
    public float referenceSize = 48f;
    static CustomCursor instance;

    private void Awake()
    {
        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);
        Cursor.lockState = CursorLockMode.None;
        DontDestroyOnLoad(gameObject);


        Cursor.visible = false;

        UpdateCursorSize();
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        UpdateCursorPosition();
        UpdateCursorSize();
    }

    private void UpdateCursorPosition()
    {
        Vector2 mousePosition = Input.mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint
        );

        cursorRect.anchoredPosition = localPoint;
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }
    }
    private void UpdateCursorSize()
    {
        float scale = Screen.height / referenceResolution.y;
        cursorRect.sizeDelta = Vector2.one * referenceSize * scale;
    }
}