using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class MapObjectDatabase : MonoBehaviour
{
    public static MapObjectDatabase instance;
    public Dictionary<string, MapObject> MapObjectDictionary;
    public Dictionary<(string, string), MapObject> CombinationDictionary;
    public Dictionary<(ActionsEnum, string), MapObject> ActionsDictionary;
    public Dictionary<(ZoneEnum, string), MapObject> ZoneDictionary;

    void Awake()
    {
        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

        CreateDictionaries();
    }
    public void CreateDictionaries()
    {
        MapObjectSO[] mapObjectSO = Resources.LoadAll<MapObjectSO>("Scriptable Objects/Map Objects");
        MapObject[] mapObjects = new MapObject[mapObjectSO.Length];
        for (int i = 0; i < mapObjectSO.Length; i++)
        {
            MapObject mapObject = new MapObject(mapObjectSO[i]);
            mapObjects[i] = mapObject;
            
        }
        CreateMapObjectDictionary(mapObjects);
        CreateCombinationDictionary(mapObjects);
        CreateActionsDictionary(mapObjects);
        CreateZoneDictionary(mapObjects);
    }

    

    public void CreateMapObjectDictionary(MapObject[] mapObjects)
    {
        MapObjectDictionary = new();
        foreach (MapObject obj in mapObjects)
        {
            MapObjectDictionary.Add(obj.Name, obj);
        }

    }

    private void CreateCombinationDictionary(MapObject[] mapObjects)
    {
        CombinationDictionary = new();
        foreach (MapObject obj in mapObjects)
        {
            if (obj.TargetMapObject != null && obj.CombinedMaterial != null) CombinationDictionary.Add((obj.CombinedMaterial.Name, obj.TargetMapObject.Name), obj);
        }

    }

    public void CreateActionsDictionary(MapObject[] mapObjects)
    {
        ActionsDictionary = new();
        foreach (MapObject obj in mapObjects)
        {
            if (obj.TargetMapObject != null) ActionsDictionary.Add((obj.ActionRequired, obj.TargetMapObject.Name), obj);
        }
    }

    public void CreateZoneDictionary(MapObject[] mapObjects)
    {
        ZoneDictionary = new();
        foreach (MapObject obj in mapObjects)
        {
            if (obj.CombinedMaterial != null)
            {
                ZoneDictionary.Add((obj.ZoneRequired, obj.CombinedMaterial.Name), obj);
            }
        }
    }

}
