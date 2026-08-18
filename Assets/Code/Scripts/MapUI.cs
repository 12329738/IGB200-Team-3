using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    public HistoryWindow historyWindow;

    public void DisplayHistoryWindow(MapObject mapObject)
    {
        HistoryWindow window = Instantiate(historyWindow, this.transform);
        window.CreateHistory(mapObject);
    }
}
