using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    [HideInInspector]
    public Material CurrentMaterial;
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
    }
}
