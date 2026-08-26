using UnityEngine;

public class ActionButton : MonoBehaviour
{
    [HideInInspector]
    public Action action;

    public void SetActiveAction()
    {
        GameManager.instance.SetCurrentAction(action);
        ZoneManager.instance.HighlightObject(action: action);
    }
}
