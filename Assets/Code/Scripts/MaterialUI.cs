using NUnit.Framework;
using TMPro;
using UnityEngine;

public class MaterialUI : MonoBehaviour
{
    public GameObject MaterialUi;
    public GameObject MaterialButtonPrefab;

    void Awake()
    {
        MaterialSO[] materialSO = Resources.LoadAll<MaterialSO>("Scriptable Objects/Materials");

        for (int i = 0; i < materialSO.Length; i++)
        {
            Material material = new Material(materialSO[i]);
            GameObject button = Instantiate(MaterialButtonPrefab, MaterialUi.transform);
            TextMeshProUGUI text = button.GetComponent<TextMeshProUGUI>();
            text.text = material.Name;
            MaterialButton materialButton = button.GetComponentInChildren<MaterialButton>();
            materialButton.material = material;
        }                
    }
}
