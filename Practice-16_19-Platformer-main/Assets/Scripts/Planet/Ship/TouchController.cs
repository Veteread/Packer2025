using UnityEngine;

public class TouchController : MonoBehaviour
{
    [SerializeField] private ShipCamera2D shipCamera;
    [SerializeField] private float zoomSensitivity = 0.1f;
    [SerializeField] private float dragSensitivity = 0.01f;

    private Vector2 lastTouchPosition;
    private float initialTouchDistance;

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            HandleSingleTouch();
        }
        else if (Input.touchCount == 2)
        {
            HandlePinchZoom();
        }
    }

    private void HandleSingleTouch()
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            lastTouchPosition = touch.position;
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            Vector2 delta = touch.position - lastTouchPosition;
            transform.Rotate(0, 0, delta.x * dragSensitivity);
            lastTouchPosition = touch.position;
        }
    }

    private void HandlePinchZoom()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        if (touch2.phase == TouchPhase.Began)
        {
            initialTouchDistance = Vector2.Distance(touch1.position, touch2.position);
        }
        else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
        {
            float currentDistance = Vector2.Distance(touch1.position, touch2.position);
            float delta = currentDistance - initialTouchDistance;

            shipCamera.Zoom(delta * zoomSensitivity);

            initialTouchDistance = currentDistance;
        }
    }
}