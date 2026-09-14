using UnityEngine;
using UnityEngine.InputSystem;

public class MaterialDragController : MonoBehaviour
{
    public static MaterialDragController Instance { get; private set; }

    private Material draggedMaterial;
    private Zone currentTarget;

    private bool isDragging;
    private Vector2 startPosition;
    private bool pendingDrag;
    private const float dragThreshold = 10f;
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (draggedMaterial == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (pendingDrag)
        {
            float distance = Vector2.Distance(
                startPosition,
                mousePosition
            );

            if (distance < dragThreshold)
                return;

            pendingDrag = false;
            isDragging = true;
        }

        if (!isDragging)
            return;

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            EndDrag(mousePosition);
            return;
        }

        UpdateDrag(mousePosition);
    }


    public void StartDrag(Material material)
    {
        draggedMaterial = material;
        startPosition = Mouse.current.position.ReadValue();
        pendingDrag = true;
        isDragging = false;
    }

    private void UpdateDrag(Vector2 screenPosition)
    {
        Zone target = GetTarget(screenPosition);

        if (target == currentTarget)
            return;

        currentTarget = target;
    }

    private void EndDrag(Vector2 screenPosition)
    {
        Zone target = GetTarget(screenPosition);

        if (target != null)
        {
            target.HandleClick();
                
        }

        ClearDrag();
    }

    private Zone GetTarget(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return null;

        return hit.collider.GetComponent<Zone>();
    }

    private void ClearDrag()
    {
        GameManager.instance.ResetCurrentAction();
        draggedMaterial = null;
        currentTarget = null;
        isDragging = false;
    }
}
