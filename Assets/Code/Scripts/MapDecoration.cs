using LitMotion.Animation;
using System;
using Unity.ProjectAuditor.Editor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MapDecoration : MonoBehaviour
{
    public SpriteRenderer image;
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propertyBlock;
    //public LitMotionAnimation lmAnimation;
    [SerializeField] private InputAction mouseClick;
    [HideInInspector] public bool IsMoving = false;
    public LayerMask obstacleLayer;
    public PolygonCollider2D decorationArea;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;
    }
    private void Start()
    {
        SetHighlight(false);
    }
    private void Update()
    {
        Move();
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

    private Vector3 mouseOffset;

    private Vector3 GetMousePosition()
    {
        Vector3 mouseInput = Mouse.current.position.ReadValue();

        mouseInput.z = transform.position.z -
                       Camera.main.transform.position.z;

        Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(mouseInput);

        return mouseInWorld;
    }

    private void MouseClickAction(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out MapDecoration knob))
            {
                mouseOffset = knob.transform.position - GetMousePosition();

                knob.IsMoving = true;
            }
        }
    }

    private void MouseReleaseAction(InputAction.CallbackContext context)
    {
        IsMoving = false;
    }

    private void Move()
    {
        if (IsMoving)
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();

            Vector3 halfExtents = boxCollider.size * 0.5f;
            Vector3 newPosition = GetMousePosition() + mouseOffset;
            Collider[] colliders = Physics.OverlapBox(
                newPosition,
                halfExtents,
                transform.rotation,
                obstacleLayer
            );
            if (!decorationArea.OverlapPoint(GetMousePosition()))
                return;


            if (colliders.Length == 0 || (colliders.Length == 1 && colliders[0] == boxCollider))
                transform.position = GetMousePosition() + mouseOffset;
        }
    }

}
