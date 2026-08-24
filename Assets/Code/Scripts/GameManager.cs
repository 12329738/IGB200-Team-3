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
    public InputTracker inputTracker;
    public MapObject lastChangedObject;
    public Zone lastChangedObjectZone;
    void Awake()
    {

        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

    }
    void Update()
    {
        if (inputTracker.TimeSinceLastInput > inputTracker.ResetTimer)
        {
            ResetScene();
        }
    }
    public void SetCurrentMaterial(Material material)
    {
        CurrentAction = null;
        CurrentMaterial = material;
        
    }

    public void SetCurrentAction(Action action)
    {
        CurrentMaterial = null;
        CurrentAction = action; 
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
