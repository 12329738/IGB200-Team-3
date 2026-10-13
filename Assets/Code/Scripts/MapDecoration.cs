using LitMotion.Animation;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapDecoration : MonoBehaviour
{
    public SpriteRenderer image;
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propertyBlock;
    
    [HideInInspector] public bool IsMoving = false;
    public LayerMask obstacleLayer;
    public PolygonCollider2D decorationArea;
    public bool movable = true;
    public MapObject mapObject;
    SphereCollider sphereCollider;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        sphereCollider = GetComponent<SphereCollider>();
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;
    }
    private void Start()
    {
        SetHighlight(false);
        image.sortingOrder = (int)(-spriteRenderer.bounds.min.y * 100);
    }
    private void Update()
    {
        Move();
    }
    

    public void SetHighlight(bool highlighted)
    {
        spriteRenderer.GetPropertyBlock(propertyBlock);

        if (highlighted)
        {
            UnityEngine.Material material = spriteRenderer.sharedMaterial;

            propertyBlock.SetColor(
                "_OutlineColor",
                material.GetColor("_OutlineColor")
            );

            propertyBlock.SetFloat(
                "_OutlineWidth",
                material.GetFloat("_OutlineWidth")
            );
        }
        else
        {
            propertyBlock.SetFloat("_OutlineWidth", 0f);
        }

        spriteRenderer.SetPropertyBlock(propertyBlock);
    }

    public void SetVisible(bool visible)
    {
        Color color = Color.white;
        color.a = visible ? 1f : 0f;
        image.color = color;
    }

    public Vector3 mouseOffset;

    private Vector3 GetMousePosition()
    {
        Vector3 mouseInput = Mouse.current.position.ReadValue();

        mouseInput.z = transform.position.z -
                       Camera.main.transform.position.z;

        Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(mouseInput);

        return mouseInWorld;
    }


    private void Move()
    {
        if (!IsMoving)
            return;

        Vector3 newPosition = GetMousePosition() + mouseOffset;

        if (!IsDecorationInsideIsland(newPosition, sphereCollider))
            return;

        if (!decorationArea.OverlapPoint(newPosition))
            return;

        Vector3 originalPosition = transform.position;
        transform.position = newPosition;

        bool blocked = false;

        Collider[] colliders = Physics.OverlapSphere(
            sphereCollider.bounds.center,
            sphereCollider.bounds.extents.magnitude,
            obstacleLayer
        );

        foreach (Collider collider in colliders)
        {
            if (collider == sphereCollider)
                continue;

            if (Physics.ComputePenetration(
                sphereCollider,
                transform.position,
                transform.rotation,
                collider,
                collider.transform.position,
                collider.transform.rotation,
                out _,
                out _))
            {
                blocked = true;
                break;
            }
        }

        if (blocked)
            transform.position = originalPosition;

        image.sortingOrder = (int)(-spriteRenderer.bounds.min.y * 100);
    }


    private bool IsDecorationInsideIsland(
    Vector3 position,
    SphereCollider sphereCollider)
    {
        float radius = sphereCollider.radius;

        float scale = Mathf.Max(
            sphereCollider.transform.lossyScale.x,
            sphereCollider.transform.lossyScale.y);

        radius *= scale;

        Vector2 center = new Vector2(position.x, position.y);

        const int pointCount = 16;

        if (!decorationArea.OverlapPoint(center))
            return false;

        for (int i = 0; i < pointCount; i++)
        {
            float angle = (i / (float)pointCount) * Mathf.PI * 2f;

            Vector2 point = center + new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * radius;

            if (!decorationArea.OverlapPoint(point))
                return false;
        }

        return true;
    }

}
