using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable MapObjects/New MapObject")]
[Serializable]
public class MapObjectSO : ScriptableObject
{
    public string Name;
    public MapObjectSO TargetMapObject;
    public MaterialSO CombinedMaterial;
    public ZoneEnum ZoneRequired;
    public ActionsEnum ActionRequired;
    
    
}
