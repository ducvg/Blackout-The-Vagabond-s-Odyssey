using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapUI : MonoBehaviour, IUIScreen
{
    [SerializeField] private float panSpeed = 1f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 100f;
    [SerializeField] private Camera mapCamera;
    [SerializeField] private GameObject minimap, map;

    private void Awake()
    {
        Close(); //big map closed by default
    }

    public void Open()
    {
        map.SetActive(true);
        minimap.SetActive(false);
        InputManager.Instance.playerInput.MapUI.Enable();
    }

    public void Close()
    {
        map.SetActive(false);
        minimap.SetActive(true);
        InputManager.Instance.playerInput.MapUI.Disable();
    }

    private void OnEnable()
    {
        InputManager.Instance.playerInput.Ingame.Togglemap.performed += Toggle;
        InputManager.Instance.playerInput.Ingame.Togglemap.Enable();
        InputManager.Instance.playerInput.MapUI.Zoommap.performed += Zoom;
        InputManager.Instance.playerInput.MapUI.Zoommap.Enable();
        InputManager.Instance.playerInput.MapUI.Panmap.performed += Pan;
        InputManager.Instance.playerInput.MapUI.Panmap.Enable();
    }

    private void OnDisable()
    {
        InputManager.Instance.playerInput.Ingame.Togglemap.performed -= Toggle;
        InputManager.Instance.playerInput.Ingame.Togglemap.Disable();
        InputManager.Instance.playerInput.MapUI.Zoommap.performed -= Zoom;
        InputManager.Instance.playerInput.MapUI.Zoommap.Disable();
        InputManager.Instance.playerInput.MapUI.Panmap.performed -= Pan;
        InputManager.Instance.playerInput.MapUI.Panmap.Disable();
    }

    private void Toggle(InputAction.CallbackContext context)
    {
        UIManager.Instance.ToggleScreen(this);
    }

    private void Zoom(InputAction.CallbackContext context)
    {
        float zoom = mapCamera.orthographicSize;
        zoom -= context.ReadValue<float>() * zoomSpeed;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        mapCamera.orthographicSize = zoom;
    }

    private void Pan(InputAction.CallbackContext context)
    {
        if (Mouse.current.leftButton.isPressed) // fixed left click hold to pan
        {
            Vector2 mouseDelta = context.ReadValue<Vector2>(); // mouse pos change each frame
            Vector3 move = new Vector3(-mouseDelta.x, -mouseDelta.y, 0) * panSpeed; 
            mapCamera.transform.position += move;
        }
    }
}