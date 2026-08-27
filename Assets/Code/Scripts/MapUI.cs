using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    public HistoryWindow historyWindow;
    public static MapUI instance;
    public GameObject blocker;
    public Canvas canvas;

    void Awake()
    {

        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);


    }
    public void DisplayHistoryWindow(MapObject mapObject)
    {
        blocker.SetActive(true);
        HistoryWindow window = Instantiate(historyWindow, canvas.transform);
        window.CreateHistory(mapObject);
    }
}
