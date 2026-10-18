using LitMotion.Animation;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StorageUI : MonoBehaviour
{
    public GameObject StorageUi;
    public GameObject StorageIconPrefab;
    public GameObject storageCounter;
    public TextMeshProUGUI text;
    public RectTransform canvasRect;
    LitMotionAnimation animation;

    void Start()
    {
        storageCounter = Instantiate(StorageIconPrefab, StorageUi.transform);
        text = storageCounter.GetComponentInChildren<TextMeshProUGUI>();
        text.text = $"{GameManager.instance.currentRecycledWaste}";
        animation = text.gameObject.GetComponent<LitMotionAnimation>();

    }

    public void ChangeStorageAmount(int amount)
    {
        RectTransform uiRect = GetComponent<RectTransform>();
        Debug.Log(uiRect.position);
        Canvas canvas = uiRect.GetComponentInParent<Canvas>();

        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
            uiCamera,
            uiRect.position
        );
        Debug.Log(screenPoint);

        // Project onto a world-space plane, such as the XZ ground plane at Y = 0.
        Plane groundPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, 0f));
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);

        Vector3 position = Vector3.zero;
        if (groundPlane.Raycast(ray, out float distance))
        {
            position = ray.GetPoint(distance);
        }
        Debug.Log(position);
        TextPopup popup = Popup.instance.ShowText(position, $"{amount} <sprite name=\"Waste edit\">");

        text.text = $"{GameManager.instance.currentRecycledWaste}";
    }

    public RectTransform GetMaterialUILocation()
    {

        return storageCounter.GetComponent<RectTransform>();
    }
}
