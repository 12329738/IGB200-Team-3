using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Zone : MonoBehaviour
{
    public ZoneEnum zone;
    public MapObject currentObject;
    public GameObject zoneWorld;
    public GameObject currentMapImage;
    public Image icon;
    public TextMeshProUGUI text;

    public void OnClick()
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
            foreach (MapObject mapObject in mapObjects)
            {
                if (mapObject.RequiredStoredMaterial != null)
                {
                    if (GameManager.instance.HasRequiredMatierals(mapObject))
                    {
                        ChangeMapObject(mapObject);
                        GameManager.instance.ChangeStoredMaterialAmount(mapObject.RequiredStoredMaterial, mapObject.RequiredStoredMaterialAmount);
                        return;
                    }

                }

                else if (mapObject.HarvestedMaterial != null)
                {
                    HarvestMapObject(mapObject);
                }
            }        
        }      
    }

    private void HarvestMapObject(MapObject mapObject)
    {
        GameManager.instance.ChangeStoredMaterialAmount(mapObject.HarvestedMaterial, 1);
        Destroy(currentMapImage);
        currentObject = null;
        text.text = "";
    }
    private void ChangeMapObject(MapObject mapObject)
    {
        if (mapObject != null)
        {
            Destroy(currentMapImage);
            if (mapObject.Image != null) currentMapImage = Instantiate(mapObject.Image, zoneWorld.transform);
            currentObject = mapObject;
            text.text = mapObject.Name;
        }
    }

    public void ResetZone()
    {
        Destroy(currentMapImage);
        currentObject = null;
        text.text = "";
    }
}
