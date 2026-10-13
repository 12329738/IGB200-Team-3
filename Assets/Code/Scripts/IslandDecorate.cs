using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class IslandDecorate : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public PolygonCollider2D area;
    public GameObject decorationPrefab;
    public GameObject wastePrefab;
    MapDecoration movingDecoration;
    Vector3 mouseOffset;
    [SerializeField] private InputAction mouseClick;
    List<SpriteScript> wasteStack = new();
    public void PlaceItemOnIsland(MapObject mapObject)

    {
        GameObject obj;
        if (mapObject.Name.Contains("Waste"))
        {
            obj = Instantiate(wastePrefab);
            SpriteScript waste = obj.GetComponent<SpriteScript>();
            waste.mapObject = mapObject;
            wasteStack.Add(waste);
        }
        else
            obj = Instantiate(decorationPrefab);
    
        MapDecoration decoration = obj.GetComponent<MapDecoration>();
        decoration.decorationArea = area;

        decoration.mapObject = mapObject;

        decoration.image.sprite = mapObject.image;

        decoration.transform.SetParent(transform);

        SphereCollider sphereCollider = decoration.GetComponent<SphereCollider>();

        Bounds bounds = area.bounds;

        for (int i = 0; i < 100; i++)
        {
            Vector3 extents = sphereCollider.bounds.extents;

            Vector3 position = new Vector3(
                Random.Range(bounds.min.x + extents.x, bounds.max.x - extents.x),
                Random.Range(bounds.min.y + extents.y, bounds.max.y - extents.y),
                0f
            );

            if (!IsDecorationInsideIsland(position, sphereCollider))
                continue;

            Collider[] colliders = Physics.OverlapSphere(
                position,
                sphereCollider.radius,
                obstacleLayer
            );

            if (colliders.Length == 0)
            {
                decoration.transform.position = position;

                break;
            }
        }
    }

    private bool IsDecorationInsideIsland(
    Vector3 position,
    SphereCollider sphereCollider)
    {
        float radius = sphereCollider.radius;

        // Account for the collider's transform scale.
        float scale = Mathf.Max(
            sphereCollider.transform.lossyScale.x,
            sphereCollider.transform.lossyScale.y);

        radius *= scale;

        Vector2 center = new Vector2(position.x, position.y);

        // Check the center and points around the circumference.
        const int pointCount = 16;

        if (!area.OverlapPoint(center))
            return false;

        for (int i = 0; i < pointCount; i++)
        {
            float angle = (i / (float)pointCount) * Mathf.PI * 2f;

            Vector2 point = center + new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * radius;

            if (!area.OverlapPoint(point))
                return false;
        }

        return true;


    }

    private void MouseClickAction(InputAction.CallbackContext context)
    {

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out MapDecoration mapDecoration))
            {
                

                if (mapDecoration.movable)
                {
                    ZoneManager.instance.ToggleRedZones(true);
                    mouseOffset = mapDecoration.transform.position - GetMousePosition();
                    mapDecoration.IsMoving = true;
                    movingDecoration = mapDecoration;
                    mapDecoration.mouseOffset = mouseOffset;
                }
                    
                
            }
        }
    }

    private void MouseReleaseAction(InputAction.CallbackContext context)
    {
        if (movingDecoration != null)
        {
            movingDecoration.IsMoving = false;
            movingDecoration = null;
        }
        ZoneManager.instance.ToggleRedZones(false);
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mouseInput = Mouse.current.position.ReadValue();

        mouseInput.z = transform.position.z -
                       Camera.main.transform.position.z;

        Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(mouseInput);

        return mouseInWorld;
    }

    public void RecycleAllWaste()
    {    
        foreach (SpriteScript waste in wasteStack)  
        {
            waste.RecycleMapObject("Recycle");
        }
        wasteStack.Clear();
    }
    private void OnEnable()
    {
        mouseClick.Enable();
        mouseClick.performed += MouseClickAction;
        mouseClick.canceled += MouseReleaseAction;
    }

    private void OnDisable()
    {
        mouseClick.Disable();
        mouseClick.performed -= MouseClickAction;
        mouseClick.canceled -= MouseReleaseAction;
    }

}
