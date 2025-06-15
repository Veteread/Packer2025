using UnityEngine;
using UnityEngine.UI;

public class BlockThrowSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float minAngle = -180f;
    [SerializeField] private float maxAngle = 180f;
    [SerializeField] private float maxPower = 20f;
    [SerializeField] private float chargeSpeed = 5f;

    [Header("UI")]
    [SerializeField] private Image powerBar;
    [SerializeField] private GameObject powerUI;
    [SerializeField] private Transform directionArrow;

    private Vector2 startSwipePos;
    private Vector2 currentSwipePos;
    private float currentPower;
    private float currentAngle;
    private bool isCharging;

    public bool IsThrowing { get; private set; }
    public Vector2 ThrowDirection { get; private set; }

    public void StartAiming(Vector2 screenPos)
    {
        IsThrowing = true;
        isCharging = false;
        currentPower = 0f;
        startSwipePos = screenPos;
        powerUI.SetActive(true);
    }

    public void UpdateAim(Vector2 currentPos)
    {
        if (!IsThrowing) return;

        Vector2 swipeVector = currentPos - startSwipePos;
        currentAngle = CalculateThrowAngle(swipeVector);

        // Визуализация
        if (directionArrow != null)
        {
            directionArrow.rotation = Quaternion.Euler(0, 0, currentAngle);
        }

        // Автозапуск заряда
        if (!isCharging && swipeVector.magnitude > 20f)
        {
            StartCharging();
        }
    }

    public void StartCharging()
    {
        isCharging = true;
        currentPower = 0f;
    }

    private float CalculateThrowAngle(Vector2 swipeVector)
    {
        // Получаем сырой угол в диапазоне [-180, 180]
        float rawAngle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;

        // Нормализуем угол в диапазон [0, 360]
        float normalizedAngle = (rawAngle + 360) % 360;

        // Корректируем углы больше 180
        if (normalizedAngle > 180)
        {
            normalizedAngle -= 360;
        }

        // Ограничиваем заданными пределами
        return Mathf.Clamp(normalizedAngle, minAngle, maxAngle);
    }
    public void ChargePower()
    {
        if (!isCharging) return;

        currentPower = Mathf.Min(currentPower + chargeSpeed * Time.deltaTime, maxPower);
        powerBar.fillAmount = currentPower / maxPower;
    }

    public void ExecuteThrow(Block block)
    {
        if (!IsThrowing || block == null) return;

        block.EnablePhysicsForThrow();

        // Преобразуем угол в радианы
        float angleRad = currentAngle * Mathf.Deg2Rad;

        // Создаем вектор направления
        Vector2 force = new Vector2(
            Mathf.Cos(angleRad),
            Mathf.Sin(angleRad)
        ) * currentPower;

        // Применяем силу
        block.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

        Debug.Log($"Бросок: угол={currentAngle}°, сила={currentPower}");

        ResetSystem();
    }

    private void ResetSystem()
    {
        IsThrowing = false;
        isCharging = false;
        currentPower = 0f;
        powerUI.SetActive(false);
    }
}