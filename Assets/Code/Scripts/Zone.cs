using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Zone : MonoBehaviour
{
    public ZoneEnum zone;
    public MapObject currentObject;
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
            MapObjectDatabase.instance.ZoneDictionary.TryGetValue((zone, GameManager.instance.CurrentMaterial.Name), out MapObject mapObject);
            if (mapObject != null)
            {
                currentObject = mapObject;
                text.text = mapObject.Name;
            }
        }        
    }

    private void CombineMapObjectWithMaterial()
    {
        MapObjectDatabase.instance.CombinationDictionary.TryGetValue((GameManager.instance.CurrentMaterial.Name, currentObject.Name), out MapObject mapObject);
        if (mapObject != null)
        {
            currentObject = mapObject;
            text.text = mapObject.Name;
        }
    }

    private void PerformActionOnMapObject()
    {
        MapObjectDatabase.instance.ActionsDictionary.TryGetValue((GameManager.instance.CurrentAction.Name, currentObject.Name), out MapObject mapObject);
        if (mapObject != null)
        {
            currentObject = mapObject;
            text.text = mapObject.Name;
        }
    }

    public void ResetZone()
    {
        currentObject = null;
        text.text = "";
    }
}
