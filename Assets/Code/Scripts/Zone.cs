using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Zone : MonoBehaviour
{
    public ZoneEnum zone;
    public MapObject currentObject;
    public GameObject currentMapImage;
    public TextMeshProUGUI text;
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
        if (MapObjectDatabase.instance.CombinationDictionary.TryGetValue((GameManager.instance.CurrentMaterial.Name, currentObject.Name), out MapObject mapObject));
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
        Destroy(currentMapImage);
        currentObject = null;
        text.text = "";
        GameManager.instance.ResetCurrentAction();
    }
    private void ChangeMapObject(MapObject mapObject)
    {
        if (mapObject != null)
        {
            Destroy(currentMapImage);
            if (mapObject.Image != null) currentMapImage = Instantiate(mapObject.Image, transform);
            GameManager.instance.objectHistory.Push((currentObject, this));
            currentObject = mapObject;
            text.text = mapObject.Name;
            GameManager.instance.ResetCurrentAction();
        }
    }

    public void Undo(MapObject mapObject)
    {
        if (mapObject == null)
        {
            Destroy(currentMapImage);
            currentObject = null;
            text.text = "";
        }
        else
        {
            Destroy(currentMapImage);
            currentMapImage = Instantiate(mapObject.Image, transform);
            currentObject = mapObject;
            text.text = mapObject.Name;

        }
       
    }
}
