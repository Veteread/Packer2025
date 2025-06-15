using UnityEngine;

public class PlayerBlockDropper : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private BlockSpawnSystem spawnSystem;
    [SerializeField] private BlockThrowSystem throwSystem;
    [SerializeField] private GameProgressTracker progressTracker;
    [SerializeField] private BlockSelectionUI selectionUI;
    [SerializeField] private Camera gameCamera;

    private Block currentBlock;
    private float lastCheckTime;
    private bool isWaitingForThrow; // Новое состояние

    private void Start()
    {
        if (selectionUI != null)
        {
            selectionUI.OnBlockSelected += OnBlockSelected;
        }
        else
        {
            Debug.LogError("BlockSelectionUI not assigned!");
        }
    }

    

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            HandleInput();
        }

        if (Time.time - lastCheckTime > 0.5f)
        {
            progressTracker.CheckCompletion();
            lastCheckTime = Time.time;
        }
    }

    private void OnBlockSelected(GameObject blockPrefab)
    {
        if (currentBlock != null)
        {
            Destroy(currentBlock.gameObject);
        }
        spawnSystem.ShowSpawnPoints();
        isWaitingForThrow = false; // Сброс состояния
    }

    private void HandleInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        Vector2 touchPos = gameCamera.ScreenToWorldPoint(touch.position);
        Vector2 screenPos = touch.position;

        // Выбор точки спавна
        if (currentBlock == null && touch.phase == TouchPhase.Began)
        {
            if (spawnSystem.TrySelectPoint(screenPos, gameCamera))
            {
                GameObject prefab = selectionUI.GetSelectedPrefab();
                if (prefab != null)
                {
                    currentBlock = spawnSystem.SpawnBlock(prefab, spawnSystem.SelectedPoint, this);
                }
            }
            return;
        }

        // Обработка броска
        if (currentBlock != null)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (CurrencySystem.Instance.CanAfford(currentBlock.GetCost()))
                    {
                        currentBlock.MarkAsCounted(); // Помечаем как "оплаченный"
                        CurrencySystem.Instance.SpendCurrency(currentBlock.GetCost());
                        throwSystem.StartAiming(screenPos);
                    }
                    break;

                case TouchPhase.Moved:
                    throwSystem.UpdateAim(screenPos);
                    throwSystem.ChargePower();
                    break;

                case TouchPhase.Ended:
                    if (throwSystem.IsThrowing)
                    {
                        CurrencySystem.Instance.SpendCurrency(currentBlock.GetCost());
                        currentBlock.MarkAsCounted();
                        throwSystem.ExecuteThrow(currentBlock);
                        currentBlock = null;
                    }
                    break;
            }
        }
    }

    public void RemoveBlock(Block block)
    {
        if (currentBlock == block)
        {
            currentBlock = null;
        }
    }
}