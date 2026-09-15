using UnityEngine;
using UnityEngine.UI;

public class FinalFormButton : MonoBehaviour
{
    public Image image;
    public MapObject mapObject;
    public GameObject checkMark;

    public void OnClick()
    {
        MapUI.instance.DisplayHistoryWindow(mapObject);
    }

    public void SetComplete()
    {
        checkMark.SetActive(true);
    }
}
