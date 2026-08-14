using UnityEngine;
using UnityEngine.UI;

public class Action
{
    public string Name;
    public Image Icon;

    public Action(ActionSO SO)
    {
        Name = SO.Name;
        Icon = SO.Icon;
    }
}
