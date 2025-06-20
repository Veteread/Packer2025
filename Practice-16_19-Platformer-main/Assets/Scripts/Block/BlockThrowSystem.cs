using Cysharp.Threading.Tasks;
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
    public GameObject powerUI;
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
    }

    public void UpdateAim(Vector2 currentPos)
    {
        if (!IsThrowing) return;

        Vector2 swipeVector = currentPos - startSwipePos;
        currentAngle = CalculateThrowAngle(swipeVector);

        // Âèçóàëèçàöèÿ
        if (directionArrow != null)
        {
            directionArrow.rotation = Quaternion.Euler(0, 0, currentAngle);
        }

        // Àâòîçàïóñê çàðÿäà
        if (!isCharging && swipeVector.magnitude > 20f)
        {
            StartCharging();
        }
    }

    public async UniTask ExecuteThrowAsync(Block block)
    {
        if (block == null) return;

        // Получаем вектор силы броска
        Vector2 force = CalculateThrowForce();

        // Применяем силу броска
        Rigidbody2D rb = block.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(force, ForceMode2D.Impulse);
        }

        // Ждем физического обновления
        await UniTask.WaitForFixedUpdate();

        ResetSystem();
    }

    private Vector2 CalculateThrowForce()
    {
        // Преобразуем угол в радианы
        float angleRad = currentAngle * Mathf.Deg2Rad;

        // Создаем вектор направления
        return new Vector2(
            Mathf.Cos(angleRad),
            Mathf.Sin(angleRad)
        ) * currentPower;
    }

    public void StartCharging()
    {
        isCharging = true;
        currentPower = 0f;
    }

    private float CalculateThrowAngle(Vector2 swipeVector)
    {
        // Ïîëó÷àåì ñûðîé óãîë â äèàïàçîíå [-180, 180]
        float rawAngle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;

        // Íîðìàëèçóåì óãîë â äèàïàçîí [0, 360]
        float normalizedAngle = (rawAngle + 360) % 360;

        // Êîððåêòèðóåì óãëû áîëüøå 180
        if (normalizedAngle > 180)
        {
            normalizedAngle -= 360;
        }

        // Îãðàíè÷èâàåì çàäàííûìè ïðåäåëàìè
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

        // Ïðåîáðàçóåì óãîë â ðàäèàíû
        float angleRad = currentAngle * Mathf.Deg2Rad;

        // Ñîçäàåì âåêòîð íàïðàâëåíèÿ
        Vector2 force = new Vector2(
            Mathf.Cos(angleRad),
            Mathf.Sin(angleRad)
        ) * currentPower;

        // Ïðèìåíÿåì ñèëó
        block.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

        ResetSystem();
    }

    public void ResetSystem()
    {
        IsThrowing = false;
        isCharging = false;
        currentPower = 0f;
    }
}