using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalFormUi : MonoBehaviour
{
    public GameObject FinalFormUI;
    public FinalFormButton FinalFormIconPrefab;
    public Dictionary<string, FinalFormButton> finalFormButtonDictionary;

    void Start()
    {
        //gameObject.SetActive(false);
        finalFormButtonDictionary = new();
        foreach (MapObject mapObject in MapObjectDatabase.instance.MapObjectDictionary.Values)
        {

            FinalFormButton button = Instantiate(FinalFormIconPrefab, FinalFormUI.transform);
            button.image.sprite = mapObject.image;
            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            //text.text = mapObject.Name;
            button.mapObject = mapObject;
            finalFormButtonDictionary.Add(mapObject.Name, button);
                      
        }
    }

    public void MarkItemAsComplete(string name)
    {
        finalFormButtonDictionary[name].SetComplete();
    }
}
