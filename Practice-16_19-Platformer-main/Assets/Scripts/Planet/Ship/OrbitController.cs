using UnityEngine;

public class OrbitController : MonoBehaviour
{
    [SerializeField] private Transform planet;
    public float orbitRadius = 5f;
    [SerializeField] private float orbitSpeed = 30f;
    [SerializeField] private bool clockwise = true;

    private float currentAngle;

    private void Start()
    {
        // Начальная позиция на орбите
        currentAngle = Random.Range(0f, 360f);
        UpdatePosition();
    }

    private void Update()
    {
        currentAngle += (clockwise ? -1 : 1) * orbitSpeed * Time.deltaTime;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector2 pos = new Vector2(
            Mathf.Cos(rad) * orbitRadius,
            Mathf.Sin(rad) * orbitRadius
        );

        transform.position = (Vector2)planet.position + pos;

        // Поворот корабля по касательной к орбите
        float tangentAngle = currentAngle + (clockwise ? 90 : -90);
        transform.rotation = Quaternion.Euler(0, 0, tangentAngle);
    }
}