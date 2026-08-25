using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        Instantiate(arrow, parent);
        if (historyItem.image != null)
            icon.GetComponent<Image>().sprite = historyItem.image;

        if (historyItem is MapObject mapObject)
        {
            if (mapObject.RequiredAction != null)
            {
                CreatePreviousHistory(mapObject.RequiredAction, parent);
            }

            if (mapObject.createdFrom.Count >1)
            {
                GameObject branch = Instantiate(historyBranch, historyUI.transform);
                foreach (HistoryItem previousHistory in mapObject.createdFrom)
                {
                    GameObject row = Instantiate(historyRow, branch.transform);
                    
                    CreatePreviousHistory(previousHistory, row.transform);
                }
            }

            else
            {

                CreatePreviousHistory(mapObject.createdFrom[0],parent);
            }
        }
    }
}
