using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    [HideInInspector]
    public Material CurrentMaterial;
    [HideInInspector]
    public Action CurrentAction;
    public Dictionary<string, int> materialCounts;
    public StorageUI storageUi;
    void Awake()
    {

        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

    }

    public void SetCurrentMaterial(Material material)
    {
        CurrentMaterial = material;
        CurrentAction = null;
    }

    public void SetCurrentAction(Action action)
    {
        CurrentAction = action;
        CurrentMaterial = null;
    }
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool HasRequiredMatierals(MapObject mapObject)
    {
        if (materialCounts[mapObject.RequiredStoredMaterial.Name] >= mapObject.RequiredStoredMaterialAmount)
            return true;
        else
            return false;
    }

    public void ChangeStoredMaterialAmount(Material material, int amount)
    {
        materialCounts[material.Name] += amount;
        storageUi.ChangeStorageAmount(material.Name);
    }
}
