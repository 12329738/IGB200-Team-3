using UnityEngine;
using UnityEngine.Experimental.AI;

public class MapObject 
{

    public string Name;
    public MapObject RequiredMapObject;
    public Material RequiredMaterial;
    public ZoneEnum RequiredZone;
    public Action RequiredAction;

    public MapObject(MapObjectSO SO)
    {
        Name = SO.name;
        if (SO.RequiredMapObject != null) RequiredMapObject = new MapObject(SO.RequiredMapObject);
        if (SO.RequiredMaterial != null) RequiredMaterial = new Material(SO.RequiredMaterial);
        RequiredZone = SO.RequiredZone;
        if (SO.RequiredAction != null) RequiredAction = new Action(SO.RequiredAction);  
    }
}
