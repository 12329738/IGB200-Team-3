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
    public SelectionBox selection;
    GameManager gameManager;
    MapObjectDatabase mapObjectDatabase;
    private bool isHovering;
    public Color hoverColour;
    private AudioHandle creationSoundHandle;


    private void Start()
    {
        gameManager = GameManager.instance;
        mapObjectDatabase = MapObjectDatabase.instance;
        selection = GetComponentInChildren<SelectionBox>();
    }

    public void CreateMapObject()
    {
        Material material = gameManager.CurrentMaterial;

        if (material == null)
            return;

        if (!mapObjectDatabase.BasicDictionary.TryGetValue(material.Name, out MapObject mapObject))    
            return;
        
        ChangeMapObject(mapObject);

        TutorialPromptManager.ShowOnce(TutorialPromptId.FirstMaterialPlaced);
    }

    public void CombineMapObjectWithMaterial()
    {
        Material material = gameManager.CurrentMaterial;

        if (material == null || currentObject == null)
            return;

        if (!mapObjectDatabase.CombinationDictionary.TryGetValue((material.Name, currentObject.Name), out MapObject mapObject))     
            return;   

        ChangeMapObject(mapObject);

        TutorialPromptManager.ShowOnce(TutorialPromptId.FirstMaterialCombined);
    }

    public void PerformActionOnMapObject(string action)
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

        gameManager.ChangeStoredMaterialAmount(mapObject.RequiredStoredMaterial.Name, mapObject.RequiredStoredMaterialAmount);
    }

    private void RecycleMapObject(MapObject mapObject)
    {
        if (mapObject == null || currentObject == null)
            return;

        if (mapObject.HarvestedMaterial != null)
        {
            
        }

        TutorialPromptManager.ShowOnce(TutorialPromptId.FirstRecycle);

        gameManager.objectHistory.Push((currentObject, this));
        gameManager.RecycleItem(mapObject.HarvestedMaterial, currentMapObjectSprite.gameObject);
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

        Popup.instance.ShowText(currentMapObjectSprite.gameObject, mapObject.Name);
        SetMapObjectVisual(mapObject);
        ZoneManager.instance.PlayCreationSound(mapObject);
        currentObject = mapObject;

        gameManager.ResetCurrentAction();

        UnHighlightObject();

        DiscoverMapObject(mapObject);
        CheckGoalItem(mapObject);

        currentMapObjectSprite.popup.Disable();
        currentMapObjectSprite.creationAnimation.Restart();

        if (mapObject.isFinalForm)
            MoveFinalForm(mapObject);
        CreateWaste(mapObject);

    }

    private void CreateWaste(MapObject mapObject)
    {
        
        List<Material> materials = new();
        foreach (HistoryItem historyItem in mapObject.createdFrom)
        {
            if (historyItem == null)
                continue;

            if (historyItem is Material material)
            {
                materials.Add(material);
            }
        }

        foreach (Material material in materials)
        {
            MapObject waste = mapObjectDatabase.WasteDictionary[material.Name];
            ZoneManager.instance.PlaceItemOnIsland(waste);
        }
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
        currentMapObjectSprite.mapObject = mapObject;
    }

    private void DiscoverMapObject(MapObject mapObject)
    {
        mapObjectDatabase.KnownRecipeDictionary.TryAdd(mapObject.Name, mapObject);
        ZoneManager.instance.MarkItemAsBuilt(mapObject.Name);

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

        ZoneManager.instance.PlaceItemOnIsland(mapObject);
        currentObject = null;
        Destroy(currentMapObjectSprite.gameObject);

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
            selection.highlight.SetVisible(false);
            selection.highlight.SetHighlight(false);
        }

        if (currentMapObjectSprite != null)
            currentMapObjectSprite.highlight.SetHighlight(false);
    }

    private void ShowObjectHighlight()
    {
        if (currentMapObjectSprite != null)
            currentMapObjectSprite.highlight.SetHighlight(true);
    }

    private void ShowSelectionHighlight()
    {
        if (selection == null)
            return;

        selection.highlight.SetVisible(true);
        selection.highlight.SetHighlight(true);
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
