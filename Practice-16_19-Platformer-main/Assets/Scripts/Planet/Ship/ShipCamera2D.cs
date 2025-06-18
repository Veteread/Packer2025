using UnityEngine;

public class ShipCamera2D : MonoBehaviour
{
    [SerializeField] private Transform ship;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector2 offset = new Vector2(0, 0);
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 10f;

    private Camera cam;
    private float targetZoom;

    private void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = (minZoom + maxZoom) / 2;

        // Автоматическое назначение корабля если не задано
        if (ship == null) ship = FindObjectOfType<Ship2D>()?.transform;
    }

    private void LateUpdate()
    {
        if (ship == null) return;

        // Слежение за кораблем
        Vector3 targetPosition = new Vector3(
            ship.position.x + offset.x,
            ship.position.y + offset.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        // Зум камеры
        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetZoom,
            Time.deltaTime * 3f
        );
    }

    public void Zoom(float amount)
    {
        targetZoom = Mathf.Clamp(targetZoom - amount, minZoom, maxZoom);
    }
}