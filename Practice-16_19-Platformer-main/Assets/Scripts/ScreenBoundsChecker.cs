using UnityEngine;

public class ScreenBoundsChecker : MonoBehaviour
{
    [SerializeField] private float checkInterval = 0.5f;
    private Block block;
    private float timer;

    private void Awake()
    {
        block = GetComponent<Block>();
        if (block == null) block = GetComponentInParent<Block>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckIfOutsideScreen();
        }
    }

    private void CheckIfOutsideScreen()
    {
        if (Camera.main == null) return;

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        bool isOutside = viewportPos.x < -0.1f || viewportPos.x > 1.1f ||
                       viewportPos.y < -0.1f || viewportPos.y > 1.1f;

        if (isOutside)
        {
            // ¬озвращаем стоимость перед уничтожением
            if (block != null && !block.WasCounted)
            {
                CurrencySystem.Instance?.Refund(block.GetCost());
            }
            Destroy(gameObject);
        }
    }
}