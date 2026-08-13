using UnityEngine;

public class MapObject 
{

    public string Name;
    public MapObject TargetMapObject;
    public Material CombinedMaterial;
    public ZoneEnum ZoneRequired;
    public ActionsEnum ActionRequired;

    public MapObject(MapObjectSO SO)
    {
        Name = SO.name;
        if (SO.TargetMapObject != null) TargetMapObject = new MapObject(SO.TargetMapObject);
        if (SO.CombinedMaterial != null) CombinedMaterial = new Material(SO.CombinedMaterial);
        ZoneRequired = SO.ZoneRequired;
        ActionRequired = SO.ActionRequired;
    }
}
