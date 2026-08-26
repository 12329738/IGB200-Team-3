using UnityEngine;

public class SpriteHighlight : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private UnityEngine.Material normalMaterial;
    private UnityEngine.Material highlightMaterial;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        normalMaterial = spriteRenderer.material;

        Shader shader = Shader.Find("Custom/SpriteOutlineURP");

        highlightMaterial = new UnityEngine.Material(shader);

        highlightMaterial.SetColor(
            "_OutlineColor",
            Color.yellow
        );

        highlightMaterial.SetFloat(
            "_OutlineWidth",
            2f
        );
    }

    public void SetHighlight(bool highlighted)
    {
        spriteRenderer.material =
            highlighted
                ? highlightMaterial
                : normalMaterial;
    }
}
