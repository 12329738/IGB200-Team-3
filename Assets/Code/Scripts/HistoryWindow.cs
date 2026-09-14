using LitMotion;
using LitMotion.Animation;
using LitMotion.Animation.Components;
using LitMotion.Extensions;
using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class HistoryWindow : MonoBehaviour
{
    public GameObject objectIcon;
    public GameObject historyUI;
    public GameObject arrow;
    public GameObject historyBranch;
    public GameObject historyRow;
    public LitMotionAnimation closeAnimation;
    bool closing = false;

    public void OnClick()
    {
        if (!closing)
        {
            closing = true;
            closeAnimation.Play();
            
        }   
    }

    public void Update()
    {
        if (closing && !closeAnimation.IsPlaying)
        {
            Destroy(this.gameObject);
            MapUI.instance.blocker.SetActive(false);
        }
        
    }

    internal void CreateHistory(MapObject mapObject)
    {
        CreatePreviousHistory(mapObject as HistoryItem, historyUI.transform);
        
    }

    internal void CreatePreviousHistory(HistoryItem historyItem, Transform parent, Material? requiredStoredMaterial = null)
    {
        
        GameObject icon = Instantiate(objectIcon, parent);

        TextMeshProUGUI text = icon.GetComponentInChildren<TextMeshProUGUI>();
        if (MapObjectDatabase.instance.KnownRecipeDictionary.ContainsKey(historyItem.Name))
        {
            text.text = historyItem.Name;
            if (requiredStoredMaterial != null)
                text.text += $" + \n 1 recycled {requiredStoredMaterial.Name}";

            if (historyItem.image != null)
            {
                Image image = icon.GetComponent<Image>();
                image.sprite = historyItem.image;
                image.color = new Color(image.color.r, image.color.g, image.color.b, 255);
                
            }
                
        }
        else
        {
            text.text = "???";
        }
        
        if (historyItem is not Material)
            Instantiate(arrow, parent);
        if (historyItem is MapObject mapObject)
        {

            if (mapObject.RequiredAction != null)
            {
                CreatePreviousHistory(mapObject.RequiredAction, parent, mapObject.RequiredStoredMaterial);
            }

            if (mapObject.createdFrom.Count >1)
            {
                GameObject branch = Instantiate(historyBranch, parent.transform);
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
