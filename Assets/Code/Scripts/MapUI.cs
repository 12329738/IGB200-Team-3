using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    public HistoryWindow historyWindow;
    public GameObject optionsMenu;
    public ObjectSelectScreen objectSelectScreen;
    public FinalFormUi finalFormWindow;
    public static MapUI instance;
    public GameObject blocker;
    public Canvas canvas;
    public HistoryWindow historyWindowPrefab;

    void Awake()
    {

        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);


    }
    public void DisplayHistoryWindow(MapObject mapObject)
    {
        if (historyWindow != null)
            Destroy(historyWindow.gameObject);
        historyWindow = Instantiate(historyWindowPrefab, canvas.transform);
        historyWindow.CreateHistory(mapObject);
    }

    public void DisplayObjectSelectScreen(List<MapObject> mapObjects, Action<string> onSelected)
    {
        blocker.SetActive(true);
        ObjectSelectScreen window = Instantiate(objectSelectScreen, canvas.transform);
        window.DisplayObjectChoices(mapObjects, onSelected);
    }

    public void DisplayFinalFormWindow()
    {
        finalFormWindow.Show();
        
    }
    public void DisplayOptionsMenu()
    {
        blocker.SetActive(true);
        optionsMenu.SetActive(true);
    }
}
