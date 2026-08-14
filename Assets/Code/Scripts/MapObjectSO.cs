using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable MapObjects/New MapObject")]
[Serializable]
public class MapObjectSO : ScriptableObject
{
    public string Name;
    public MapObjectSO RequiredMapObject;
    public MaterialSO RequiredMaterial;
    public ZoneEnum RequiredZone;
    public ActionSO RequiredAction;
    
    
}
