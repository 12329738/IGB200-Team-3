using UnityEngine;

public class UndoButton : MonoBehaviour
{
    public void OnClick()
    {
        if (GameManager.instance.lastChangedObjectZone != null)
        GameManager.instance.lastChangedObjectZone.Undo(GameManager.instance.lastChangedObject);
    }
}
