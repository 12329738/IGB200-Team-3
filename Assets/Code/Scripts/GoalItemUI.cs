using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoalItemUI : MonoBehaviour
{
    public GameObject GoalItemUi;
    public FinalFormButton FinalFormIconPrefab;
    public int itemNumber = 2;
    public Dictionary<string, FinalFormButton> finalFormButtons;
    public TextMeshProUGUI text;
    void Start()
    {
        

        List<MapObject> finalForms = new();
        foreach (MapObject mapObject in MapObjectDatabase.instance.MapObjectDictionary.Values)
        {
            if (mapObject.isFinalForm)
            {
                finalForms.Add(mapObject);
            }
        }

        HashSet<int> chosenObjects = new();

        while (chosenObjects.Count < itemNumber)
        {
            int random = Random.Range(0, finalForms.Count);

            chosenObjects.Add(random);
        }
        finalFormButtons = new();
        foreach (int index in chosenObjects)
        {
            MapObject mapObject = finalForms[index];
            GameManager.instance.goalItems.Add(mapObject.Name);

            FinalFormButton button = Instantiate(FinalFormIconPrefab, GoalItemUi.transform);

            button.image.sprite = mapObject.image;

            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.text = mapObject.Name;

            button.mapObject = mapObject;
            finalFormButtons[mapObject.Name] = button;
        }
    }

    public void CreateFinalItems()
    {
        text.text = "See what other items you can make!";
        foreach (MapObject mapObject in MapObjectDatabase.instance.MapObjectDictionary.Values)
        {
            if (mapObject.isFinalForm)
            {
                FinalFormButton button = Instantiate(FinalFormIconPrefab, GoalItemUi.transform);
                button.image.sprite = mapObject.image;
                TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
                text.text = mapObject.Name;
                button.mapObject = mapObject;
            }
        }
    }
}
