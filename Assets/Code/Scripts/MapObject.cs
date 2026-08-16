using UnityEngine;
using UnityEngine.Experimental.AI;

public class MapObject 
{

    public string Name;
    public GameObject Image;
    public MapObject RequiredMapObject;
    public Material RequiredMaterial;
    public ZoneEnum RequiredZone;
    public Action RequiredAction;
    public Material RequiredStoredMaterial;
    public int RequiredStoredMaterialAmount;
    public Material HarvestedMaterial;

    public MapObject(MapObjectSO SO)
    {
        Name = SO.name;
        Image = SO.Image;
        if (SO.RequiredMapObject != null) RequiredMapObject = new MapObject(SO.RequiredMapObject);
        if (SO.RequiredMaterial != null) RequiredMaterial = new Material(SO.RequiredMaterial);
        RequiredZone = SO.RequiredZone;
        if (SO.RequiredAction != null) RequiredAction = new Action(SO.RequiredAction);
        if (SO.RequiredStoredMaterial != null) RequiredStoredMaterial = new Material(SO.RequiredStoredMaterial);
        RequiredStoredMaterialAmount = SO.RequiredStoredMaterialAmount;
        if (SO.HarvestedMaterial != null) HarvestedMaterial = new Material(SO.HarvestedMaterial);
    }
}
