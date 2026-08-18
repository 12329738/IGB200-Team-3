using System;
using TMPro;
using UnityEngine;

public class HistoryWindow : MonoBehaviour
{
    public GameObject objectIcon;
    public GameObject historyUI;
    public GameObject arrow;
    public GameObject historyBranch;
    public GameObject historyRow;
    public void OnClick()
    {
        Destroy(this.gameObject);
    }

    internal void CreateHistory(MapObject mapObject)
    {
        CreatePreviousHistory(mapObject as HistoryItem, historyUI.transform);
        
    }

    internal void CreatePreviousHistory(HistoryItem historyItem, Transform parent)
    {
        GameObject icon = Instantiate(objectIcon, parent);

        TextMeshProUGUI text = icon.GetComponentInChildren<TextMeshProUGUI>();
        text.text = historyItem.Name;

        if (historyItem.Image != null)
            Instantiate(historyItem.Image, icon.transform);

        if (historyItem is MapObject mapObject)
        {
            

            if (mapObject.createdFrom.Count >1)
            {
                GameObject branch = Instantiate(historyBranch, historyUI.transform);
                foreach (HistoryItem previousHistory in mapObject.createdFrom)
                {
                    GameObject row = Instantiate(historyRow, branch.transform);
                    Instantiate(arrow, row.transform);
                    CreatePreviousHistory(previousHistory, row.transform);
                }
            }

            else
            {
                Instantiate(arrow, parent);
                CreatePreviousHistory(mapObject.createdFrom[0],parent);
            }
        }
    }
}
