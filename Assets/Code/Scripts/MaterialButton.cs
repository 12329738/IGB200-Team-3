using UnityEngine;

public class MaterialButton : MonoBehaviour
{
    [HideInInspector]
    public Material material;
    
    public void SetActiveMaterial()
    {
        GameManager.instance.SetCurrentMaterial(material);

    }
}
