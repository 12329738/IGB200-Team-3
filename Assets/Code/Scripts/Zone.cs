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
    public MapUI MapUi;
    private bool isHovering;
    public Color hoverColour;

    void Start()
    {
        gameManager = GameManager.instance;
        mapObjectDatabase = MapObjectDatabase.instance;
    }
    private void OnMouseDown()
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
             return;
        }

        if (currentMapObjectSprite != null && currentMapObjectSprite.popup.isActiveAndEnabled)
        {
            currentMapObjectSprite.popup.Disable(); return;
        }

        if (currentObject == null)
        {
            CreateMapObject();
        }

        else 
        {
            if (gameManager.CurrentMaterial != null)
            {
                CombineMapObjectWithMaterial();
                return;

            }
                    
            else
            {

                string action = mapObjectDatabase.ActionsDictionary
                    .Where(x => x.Key.Item2 == currentObject.Name)
                    .Select(x => x.Key.Item1)
                    .FirstOrDefault();

                currentMapObjectSprite.popup.Initialize(action, () => MapUI.instance.DisplayHistoryWindow(currentObject), () => PerformActionOnMapObject(action), () => PerformActionOnMapObject(action));
            }
        }
    }


    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (IsPointerOverPopup())
            return;

        if (IsPointerOverCollider())
            return;

        if (currentMapObjectSprite != null &&
            currentMapObjectSprite.popup.isActiveAndEnabled)
        {
            currentMapObjectSprite.popup.Disable();
        }
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
            if (result.gameObject.transform.IsChildOf(
                    currentMapObjectSprite.popup.transform))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPointerOverCollider()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.gameObject == gameObject ||
                   hit.collider.transform.IsChildOf(transform);
        }

        return false;
    }
    private void CreateMapObject()
    {
        if (gameManager.CurrentMaterial != null)
        {
            if (mapObjectDatabase.ZoneDictionary.TryGetValue((zone, gameManager.CurrentMaterial.Name), out MapObject mapObject))
                ChangeMapObject(mapObject);
        }        
    }

    private void CombineMapObjectWithMaterial()
    {
        if (mapObjectDatabase.CombinationDictionary.TryGetValue((gameManager.CurrentMaterial.Name, currentObject.Name), out MapObject mapObject))
           ChangeMapObject(mapObject);
    }

    private void PerformActionOnMapObject(string action)
    {
        if (action == null)
            return;
        if (mapObjectDatabase.ActionsDictionary.TryGetValue((action, currentObject.Name), out List<MapObject> mapObjects))
        {
            if (mapObjects.Count > 1)
            {
                MapUI.instance.DisplayObjectSelectScreen(mapObjects, OnObjectSelected);
            }
            else 
            {
                OnObjectSelected(mapObjects[0].Name);          
            }        
        }      
    }
    private void OnObjectSelected(string objectName)
    {
        if (objectName == null)
            return;
        MapObject mapObject = mapObjectDatabase.MapObjectDictionary[objectName];
        if (mapObject.HarvestedMaterial != null)
        {
            RecycleMapObject(mapObject);
            return;
        }
        if (mapObject.RequiredMapObject.Name == currentObject.Name)
        {

            if (mapObject.RequiredStoredMaterial != null)
            {
                if (!gameManager.HasRequiredMatierals(mapObject))
                    return;
                else
                {
                    gameManager.ChangeStoredMaterialAmount(mapObject.RequiredStoredMaterial, mapObject.RequiredStoredMaterialAmount);
                }

            }

            ChangeMapObject(mapObject);
            return;

        }
    }

 

    private void RecycleMapObject(MapObject mapObject)
    {
        gameManager.ChangeStoredMaterialAmount(mapObject.HarvestedMaterial, 1);
        gameManager.objectHistory.Push((currentObject, this));
        Destroy(currentMapObjectSprite.gameObject);
        currentObject = null;
        gameManager.ResetCurrentAction();
        UnHighlightObject();
        currentMapObjectSprite.popup.Disable();
    }
    private void ChangeMapObject(MapObject mapObject)
    {
        if (mapObject != null)
        {
            if (currentObject == null)
                currentMapObjectSprite = Instantiate(mapObjectPrefab, transform);

            if (mapObject.image != null)
            {
                
                currentMapObjectSprite.image.sprite = mapObject.image;
            }
            else
            {
                currentMapObjectSprite.image.sprite = null;
            }
            gameManager.objectHistory.Push((currentObject, this));
            currentObject = mapObject;
            gameManager.ResetCurrentAction();
            UnHighlightObject();

            mapObjectDatabase.KnownRecipeDictionary.TryAdd(mapObject.Name, mapObject);
            if (mapObject.RequiredAction != null)
                mapObjectDatabase.KnownRecipeDictionary.TryAdd(mapObject.RequiredAction.Name, mapObject.RequiredAction);
            foreach (var historyItem in mapObject.createdFrom)
            {
                mapObjectDatabase.KnownRecipeDictionary.TryAdd(historyItem.Name, historyItem);
            }
            currentMapObjectSprite.popup.Disable();

            if (!gameManager.goalItemsFinished && gameManager.goalItems.Contains(mapObject.Name))
            {
                if (!gameManager.completedGoalItems.Contains(mapObject.Name))
                {
                    gameManager.AddCompletedItem(mapObject.Name);
                }          
            }
        }
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

    internal void HighlightObject(Material material, Action action)
    {
        
        if (material != null && currentObject == null)
        {
            selection.SetVisible(true);
            selection.GetComponent<SpriteScript>().SetHighlight(true);
            return;
        }

        if (currentObject != null)
        {
            currentMapObjectSprite.GetComponent<SpriteScript>().SetHighlight(false);


            
            if (material != null && mapObjectDatabase.CombinationDictionary.TryGetValue((gameManager.CurrentMaterial.Name, currentObject.Name), out MapObject mapObject))
            {
                currentMapObjectSprite.GetComponent<SpriteScript>().SetHighlight(true);
                selection.SetVisible(true);
                selection.GetComponent<SpriteScript>().SetHighlight(true);
                
            }
            else if (action != null && mapObjectDatabase.ActionsDictionary.TryGetValue((gameManager.CurrentAction.Name, currentObject.Name), out List<MapObject> mapObjects))
            {
                currentMapObjectSprite.GetComponent<SpriteScript>().SetHighlight(true);
                selection.SetVisible(true);
                selection.GetComponent<SpriteScript>().SetHighlight(true);
                
            }
        }                  
    }

    public void UnHighlightObject()
    {
        selection.GetComponent<SpriteScript>().SetHighlight(false);
        selection.SetVisible(false);
        if (currentObject != null)
            currentMapObjectSprite.GetComponent<SpriteScript>().SetHighlight(false);           
    }
}
