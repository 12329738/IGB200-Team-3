using LitMotion.Animation;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteScript : MonoBehaviour
{
    public MapObject mapObject;
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
        if (!popupOpen && GameManager.instance.CurrentMaterial == null)
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

        string action = MapObjectDatabase.instance.GetActionForCurrentObject(mapObject.Name);

        popup.Initialize(action, () => MapUI.instance.DisplayHistoryWindow(mapObject), () => PerformActionOnMapObject(action), () => RecycleMapObject(action));
    }

    private void RecycleMapObject(string action)
    {
        if (mapObject.Name == "Waste")
        {
            GameManager.instance.ChangeStoredMaterialAmount(mapObject.HarvestedMaterial, 1);
            
            TutorialPromptManager.ShowOnce(TutorialPromptId.FirstRecycle);

            Popup.instance.ShowText(gameObject, $"+1 Recycled {mapObject.HarvestedMaterial.Name}");
            Destroy(this.gameObject);

        }
        else
        zone.PerformActionOnMapObject(action);
    }

    private void PerformActionOnMapObject(string action)
    {
        zone.PerformActionOnMapObject(action);
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
