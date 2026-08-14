using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    [HideInInspector]
    public Material CurrentMaterial;
    [HideInInspector]
    public Action CurrentAction;
    public 
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
}
