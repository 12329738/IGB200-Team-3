using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public Stack<(MapObject, Zone)> objectHistory = new();
    public CurrentAction currentAction;
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
        ZoneManager.instance.UnHighlightObject();
        CurrentAction = null;
        CurrentMaterial = material;
        currentAction.image.enabled = true;
        currentAction.image.sprite = CurrentMaterial.image;
        
    }

    public void SetCurrentAction(Action action)
    {
        ZoneManager.instance.UnHighlightObject();
        CurrentMaterial = null;
        CurrentAction = action;
        currentAction.image.enabled = true;
        currentAction.image.sprite = CurrentAction.image;
        

    }

    public void ResetCurrentAction()
    {
        CurrentMaterial = null;
        CurrentAction = null;
        currentAction.image.sprite = null;
        currentAction.image.enabled = false;
        ZoneManager.instance.UnHighlightObject();
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
