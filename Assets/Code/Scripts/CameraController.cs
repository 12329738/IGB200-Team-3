using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    public float maxZoomOut = 10f;
    public float maxZoomIn = -1f;

    public InputAction scrollIn;
    public InputAction scrollOut;
    public Camera mainCamera;
    public Vector3 defaultCameraPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultCameraPosition = mainCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        scrollIn.Enable();
        scrollOut.Enable();
        scrollIn.performed += OnScrollIn;
        scrollOut.performed += OnScrollOut;
    }

    private void OnScrollOut(InputAction.CallbackContext context)
    {
        if (mainCamera.transform.position.z > maxZoomOut)
        {
            mainCamera.transform.position += new Vector3(0,0,-1);
        }
    }

    void OnScrollIn(InputAction.CallbackContext context)
    {
        if(mainCamera.transform.position.z < maxZoomIn)
        {
            mainCamera.transform.position += new Vector3(0, 0, 1);
        }
    }
}
