using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Zone : MonoBehaviour
{
    public ZoneEnum zone;
    public MapObject currentObject;
    public SpriteScript mapObjectPrefab;
    public SpriteScript currentMapObject;
    public MapUI MapUi;

    private void OnMouseDown()
    {
        if (currentObject == null)
        {
            CreateMapObject();
        }

        else 
        {
            if (GameManager.instance.CurrentMaterial != null)
            {
                CombineMapObjectWithMaterial();
            }

            else if (GameManager.instance.CurrentAction != null)
            {
                PerformActionOnMapObject();
            }

            else
            {
                MapUi.DisplayHistoryWindow(currentObject);
            }
        }
    }


    private void CreateMapObject()
    {
        if (GameManager.instance.CurrentMaterial != null)
        {
            if (MapObjectDatabase.instance.ZoneDictionary.TryGetValue((zone, GameManager.instance.CurrentMaterial.Name), out MapObject mapObject))
                ChangeMapObject(mapObject);
        }        
    }

    private void CombineMapObjectWithMaterial()
    {
        if (MapObjectDatabase.instance.CombinationDictionary.TryGetValue((GameManager.instance.CurrentMaterial.Name, currentObject.Name), out MapObject mapObject))
           ChangeMapObject(mapObject);
    }

    private void PerformActionOnMapObject()
    {
        if (MapObjectDatabase.instance.ActionsDictionary.TryGetValue((GameManager.instance.CurrentAction.Name, currentObject.Name), out List<MapObject> mapObjects))
        {
            Debug.Log(mapObjects);
            foreach (MapObject mapObject in mapObjects)
            {
                if (mapObject.RequiredMapObject.Name == currentObject.Name)
                {
                    if (mapObject.HarvestedMaterial != null)
                    {
                        HarvestMapObject(mapObject);
                        return;
                    }
                    else
                    {
                        if (mapObject.RequiredStoredMaterial != null && GameManager.instance.HasRequiredMatierals(mapObject))
                        {
                            GameManager.instance.ChangeStoredMaterialAmount(mapObject.RequiredStoredMaterial, mapObject.RequiredStoredMaterialAmount);
                        }
                        ChangeMapObject(mapObject);
                        return;
                    }
                }             
            }        
        }      
    }

    private void HarvestMapObject(MapObject mapObject)
    {
        GameManager.instance.ChangeStoredMaterialAmount(mapObject.HarvestedMaterial, 1);
        GameManager.instance.objectHistory.Push((currentObject, this));
        Destroy(currentMapObject);
        currentObject = null;
        GameManager.instance.ResetCurrentAction();
    }
    private void ChangeMapObject(MapObject mapObject)
    {
        if (mapObject != null)
        {
            if (currentMapObject == null)
                currentMapObject = Instantiate(mapObjectPrefab, transform);

            if (mapObject.image != null)
            {
                
                currentMapObject.image.sprite = mapObject.image;
            }
            else
            {
                currentMapObject.image.sprite = null;
            }
            GameManager.instance.objectHistory.Push((currentObject, this));
            currentObject = mapObject;
            GameManager.instance.ResetCurrentAction();
            UnHighlightObject();
        }
    }

    public void Undo(MapObject mapObject)
    {
        if (mapObject == null)
        {
            Destroy(currentMapObject);
            currentObject = null;
        }
        else
        {
            currentMapObject.image.sprite = mapObject.image;
            currentObject = mapObject;

        }
       
    }

    internal void HighlightObject(Material material, Action action)
    {

        if (currentObject != null)
        {
            currentMapObject.GetComponent<SpriteHighlight>().SetHighlight(false);
            if (material != null && MapObjectDatabase.instance.CombinationDictionary.TryGetValue((GameManager.instance.CurrentMaterial.Name, currentObject.Name), out MapObject mapObject))
            {
                currentMapObject.GetComponent<SpriteHighlight>().SetHighlight(true);
            }
            else if (action != null && MapObjectDatabase.instance.ActionsDictionary.TryGetValue((GameManager.instance.CurrentAction.Name, currentObject.Name), out List<MapObject> mapObjects))
            {
                currentMapObject.GetComponent<SpriteHighlight>().SetHighlight(true);
            }
        }                  
    }

    public void UnHighlightObject()
    {
        if (currentObject != null)
            currentMapObject.GetComponent<SpriteHighlight>().SetHighlight(false);
    }
}
