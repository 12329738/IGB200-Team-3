using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Zone : MonoBehaviour
{
    public ZoneEnum zone;
    public MapObject currentObject;
    public SpriteScript mapObjectPrefab;
    public SpriteScript currentMapObjectSprite;
    public SpriteScript selection;
    GameManager gameManager;
    MapObjectDatabase mapObjectDatabase;
    private bool isHovering;
    public Color hoverColour;
    private AudioHandle creationSoundHandle;


    private void Start()
    {
        gameManager = GameManager.instance;
        mapObjectDatabase = MapObjectDatabase.instance;
    }

    private void Update()
    {
        HandleHover();
        HandlePopupClosing();
    }


    private void OnMouseDown()
    {
        if (IsPointerOverUI())
            return;

        if (IsPopupOpen())
        {
            ClosePopup();
            return;
        }

        if (currentObject == null)
        {
            CreateMapObject();
            return;
        }

        if (gameManager.CurrentMaterial != null)
        {
            CombineMapObjectWithMaterial();
            return;
        }

        OpenObjectPopup();
    }
    private void HandleHover()
    {
        bool pointerOverZone = IsPointerOverCollider();

        if (pointerOverZone && !isHovering)
        {
            SetHoverState(true);
        }
        else if (!pointerOverZone && isHovering)
        {
            SetHoverState(false);
        }
    }

    private void SetHoverState(bool hovering)
    {
        isHovering = hovering;

        if (currentMapObjectSprite != null)
            currentMapObjectSprite.SetHighlight(hovering);

        if (selection != null && selection.image.color.a == 1f)
            selection.image.color = hovering ? hoverColour : Color.white;
    }
    private void HandlePopupClosing()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (!IsPopupOpen())
            return;

        if (IsPointerOverPopup())
            return;

        if (IsPointerOverCollider())
            return;

        ClosePopup();
    }
    private void OpenObjectPopup()
    {
        if (currentObject == null || currentMapObjectSprite == null)
            return;

        TutorialPromptManager.ShowOnce(
            TutorialPromptId.FirstObjectOpened
        );

        string action = GetActionForCurrentObject();

        currentMapObjectSprite.popup.Initialize(action, () => MapUI.instance.DisplayHistoryWindow(currentObject), () => PerformActionOnMapObject(action), () => PerformActionOnMapObject(action));
    }
    private string GetActionForCurrentObject()
    {
        foreach (var entry in mapObjectDatabase.ActionsDictionary)
        {
            if (entry.Key.Item2 == currentObject.Name)
                return entry.Key.Item1;
        }

        return null;
    }

    private void CreateMapObject()
    {
        Material material = gameManager.CurrentMaterial;

        if (material == null)
            return;

        if (!mapObjectDatabase.BasicDictionary.TryGetValue(material.Name, out MapObject mapObject))    
            return;
        
        ChangeMapObject(mapObject);

        TutorialPromptManager.ShowOnce(TutorialPromptId.FirstMaterialPlaced);
    }

    private void CombineMapObjectWithMaterial()
    {
        Material material = gameManager.CurrentMaterial;

        if (material == null || currentObject == null)
            return;

        if (!mapObjectDatabase.CombinationDictionary.TryGetValue((material.Name, currentObject.Name), out MapObject mapObject))     
            return;   

        ChangeMapObject(mapObject);

        TutorialPromptManager.ShowOnce(TutorialPromptId.FirstMaterialCombined);
    }

    private void PerformActionOnMapObject(string action)
    {
        if (string.IsNullOrEmpty(action) || currentObject == null)
            return;

        if (!mapObjectDatabase.ActionsDictionary.TryGetValue((action, currentObject.Name), out List<MapObject> mapObjects))      
            return;
        
        if (mapObjects == null || mapObjects.Count == 0)
            return;

        TutorialPromptManager.ShowOnce(TutorialPromptId.FirstActionUsed);

        if (mapObjects.Count == 1)
        {
            OnObjectSelected(mapObjects[0].Name);
            return;
        }

        MapUI.instance.DisplayObjectSelectScreen(mapObjects, OnObjectSelected
        );
    }

    private void OnObjectSelected(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
            return;

        if (!mapObjectDatabase.MapObjectDictionary.TryGetValue(objectName, out MapObject mapObject))
            return;
       
        if (mapObject.HarvestedMaterial != null)
        {
            RecycleMapObject(mapObject);
            return;
        }

        if (mapObject.RequiredMapObject == null)
            return;

        if (mapObject.RequiredMapObject.Name != currentObject?.Name)
            return;

        if (!HasRequiredMaterials(mapObject))
            return;

        ConsumeRequiredMaterials(mapObject);
        ChangeMapObject(mapObject);
    }

    private bool HasRequiredMaterials(MapObject mapObject)
    {
        if (mapObject.RequiredStoredMaterial == null)
            return true;

        return gameManager.HasRequiredMatierals(mapObject);
    }

    private void ConsumeRequiredMaterials(MapObject mapObject)
    {
        if (mapObject.RequiredStoredMaterial == null)
            return;

        gameManager.ChangeStoredMaterialAmount(mapObject.RequiredStoredMaterial, mapObject.RequiredStoredMaterialAmount);
    }

    private void RecycleMapObject(MapObject mapObject)
    {
        if (mapObject == null || currentObject == null)
            return;

        if (mapObject.HarvestedMaterial != null)
        {
            gameManager.ChangeStoredMaterialAmount(mapObject.HarvestedMaterial, 1);
        }

        TutorialPromptManager.ShowOnce(TutorialPromptId.FirstRecycle);

        gameManager.objectHistory.Push((currentObject, this));

        if (currentMapObjectSprite != null)
            Destroy(currentMapObjectSprite.gameObject);
        ZoneManager.instance.StopSound();
        currentObject = null;
        currentMapObjectSprite = null;

        gameManager.ResetCurrentAction();
        UnHighlightObject();
    }

    private void ChangeMapObject(MapObject mapObject)
    {
        if (mapObject == null)
            return;

        EnsureMapObjectSpriteExists();

        if (currentObject != null)
            gameManager.objectHistory.Push((currentObject, this));

        SetMapObjectVisual(mapObject);
        ZoneManager.instance.PlayCreationSound(mapObject);
        currentObject = mapObject;

        gameManager.ResetCurrentAction();

        UnHighlightObject();

        DiscoverMapObject(mapObject);
        CheckGoalItem(mapObject);

        currentMapObjectSprite.popup.Disable();
        currentMapObjectSprite.lmAnimation.Restart();

        if (mapObject.isFinalForm)
            MoveFinalForm(mapObject);
    }
    private void EnsureMapObjectSpriteExists()
    {
        if (currentMapObjectSprite != null)
            return;

        if (mapObjectPrefab == null)
        {
            return;
        }

        currentMapObjectSprite = Instantiate(
            mapObjectPrefab,
            transform
        );
    }


    private void SetMapObjectVisual(MapObject mapObject)
    {
        if (currentMapObjectSprite == null)
            return;

        currentMapObjectSprite.image.sprite = mapObject.image;
    }

    private void DiscoverMapObject(MapObject mapObject)
    {
        mapObjectDatabase.KnownRecipeDictionary.TryAdd(mapObject.Name, mapObject);

        if (mapObject.RequiredAction != null)
        {
            mapObjectDatabase.KnownRecipeDictionary.TryAdd(mapObject.RequiredAction.Name,mapObject.RequiredAction);
        }

        if (mapObject.createdFrom == null)
            return;

        foreach (HistoryItem historyItem in mapObject.createdFrom)
        {
            if (historyItem == null)
                continue;

            mapObjectDatabase.KnownRecipeDictionary.TryAdd(historyItem.Name, historyItem);
        }
    }
    private void CheckGoalItem(MapObject mapObject)
    {
        if (gameManager.goalItemsFinished)
            return;

        if (!gameManager.goalItems.Contains(mapObject.Name))
            return;

        if (gameManager.completedGoalItems.Contains(mapObject.Name))
            return;

        gameManager.AddCompletedItem(mapObject.Name);
    }

    private void MoveFinalForm(MapObject mapObject)
    {
        ZoneManager.instance.PlaceItemOnIsland(currentMapObjectSprite);
        currentObject = null;
        currentMapObjectSprite = null;

    }

    public void Undo(MapObject mapObject)
    {
        if (mapObject == null)
        {
            Destroy(currentMapObjectSprite);
            currentObject = null;
        }
        else
        {
            currentMapObjectSprite.image.sprite = mapObject.image;
            currentObject = mapObject;

        }

    }

    public void HighlightObject(Material material, Action action)
    {
        UnHighlightObject();

        if (material != null && currentObject == null)
        {
            ShowSelectionHighlight();
            return;
        }

        if (currentObject == null)
            return;

        if (material != null && CanCombine(material))
        {
            ShowObjectHighlight();
            ShowSelectionHighlight();
            return;
        }

        if (action != null && CanPerformAction(action))
        {
            ShowObjectHighlight();
            ShowSelectionHighlight();
        }
    }

    public void UnHighlightObject()
    {
        if (selection != null)
        {
            selection.SetVisible(false);
            selection.SetHighlight(false);
        }

        if (currentMapObjectSprite != null)
            currentMapObjectSprite.SetHighlight(false);
    }

    private void ShowObjectHighlight()
    {
        if (currentMapObjectSprite != null)
            currentMapObjectSprite.SetHighlight(true);
    }

    private void ShowSelectionHighlight()
    {
        if (selection == null)
            return;

        selection.SetVisible(true);
        selection.SetHighlight(true);
    }

    private bool CanCombine(Material material)
    {
        if (material == null || currentObject == null)
            return false;

        return mapObjectDatabase.CombinationDictionary.ContainsKey(
            (material.Name, currentObject.Name)
        );
    }

    private bool CanPerformAction(Action action)
    {
        if (action == null || currentObject == null)
            return false;

        return mapObjectDatabase.ActionsDictionary.ContainsKey(
            (action.Name, currentObject.Name)
        );
    }

    private bool IsPopupOpen()
    {
        return currentMapObjectSprite != null &&
               currentMapObjectSprite.popup != null &&
               currentMapObjectSprite.popup.isActiveAndEnabled;
    }

    private void ClosePopup()
    {
        if (currentMapObjectSprite?.popup != null)
            currentMapObjectSprite.popup.Disable();
    }

    private bool IsPointerOverPopup()
    {
        if (!IsPopupOpen())
            return false;

        PointerEventData pointerData = new PointerEventData(
            EventSystem.current
        )
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(
            pointerData,
            results
        );

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.transform.IsChildOf(
                    currentMapObjectSprite.popup.transform))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    }


    private bool IsPointerOverCollider()
    {
        if (Camera.main == null)
            return false;

        Ray ray = Camera.main.ScreenPointToRay(
            Input.mousePosition
        );

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return false;

        return hit.collider.gameObject == gameObject ||
               hit.collider.transform.IsChildOf(transform);
    }
}
