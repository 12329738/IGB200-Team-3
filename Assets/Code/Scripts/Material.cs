using UnityEngine;
using UnityEngine.UI;

public class Material 
{
    public string Name;
    public Image Icon;

    public Material (MaterialSO SO)
    {
        Name = SO.Name;
        Icon = SO.Icon;
    }
}
