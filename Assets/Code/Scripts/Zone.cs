using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Zone : MonoBehaviour
{
    public ZoneEnum zone;
    public MapObject currentObject;
    public Image icon;
    public TextMeshProUGUI text;

    public void CreateMapObject()
    {
        if (GameManager.instance.CurrentMaterial != null)
        {
            if (currentObject == null)
            {
                MapObjectDatabase.instance.ZoneDictionary.TryGetValue((zone, GameManager.instance.CurrentMaterial.Name), out MapObject mapObject);
                if (mapObject != null)
                {
                    currentObject = mapObject;
                    text.text = mapObject.Name;
                }
            }
            
            if (currentObject != null)
            {
                MapObjectDatabase.instance.CombinationDictionary.TryGetValue((GameManager.instance.CurrentMaterial.Name, currentObject.Name), out MapObject mapObject);
                if (mapObject != null)
                {
                    currentObject = mapObject;
                    text.text = mapObject.Name;
                }

            }
        }

    }
}
