using LitMotion.Animation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SpriteScript : MonoBehaviour
{
    public SpriteRenderer image;
    public ObjectPopup popup;
    public ImageHighlight highlight;
    public LitMotionAnimation creationAnimation;
    public LitMotionAnimation idleAnimation;
    public Zone zone;
    bool popupOpen;
    private void Awake()
    {
        highlight = GetComponent<ImageHighlight>();
        zone = GetComponentInParent<Zone>();
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;
    }

    private void OnMouseEnter()
    {
        if (!popupOpen && GameManager.instance.CurrentMaterial == null && !Mouse.current.leftButton.isPressed)
          OpenObjectPopup();
    }

    private void Update()
    {
        if (!popupOpen)
            return;

        if (!IsPointerOverMySprite() && !IsPointerOverPopup())
        {
            ClosePopup();
        }
    }



    private void OnMouseDown()
    {
        if (!popupOpen && GameManager.instance.CurrentMaterial == null)
            OpenObjectPopup();
        else
            zone.selection.OnMouseDown();
    }
    private void OpenObjectPopup()
    {
        popupOpen = true;
        TutorialPromptManager.ShowOnce(
            TutorialPromptId.FirstObjectOpened
        );

        string action = zone.GetActionForCurrentObject();

        popup.Initialize(action, () => MapUI.instance.DisplayHistoryWindow(zone.currentObject), () => zone.PerformActionOnMapObject(action), () => zone.PerformActionOnMapObject(action));
    }

    private void ClosePopup()
    {
        popupOpen = false;
        popup.Disable();
    }

    private bool IsPointerOverPopup()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            Transform hit = result.gameObject.transform;

            if (hit == popup.transform ||
                hit.IsChildOf(popup.transform))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPointerOverMySprite()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }

}
