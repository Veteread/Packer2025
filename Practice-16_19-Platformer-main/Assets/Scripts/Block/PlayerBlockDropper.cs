using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static BlockSelectionUI;

public class PlayerBlockDropper : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private BlockSpawnSystem spawnSystem;
    [SerializeField] private BlockThrowSystem throwSystem;
    [SerializeField] private GameProgressTracker progressTracker;
    [SerializeField] private BlockSelectionUI selectionUI;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private BoxCollider2D boxThrowField;

    public Rigidbody2D CurrentBlockRb { get; private set; }

    private Block currentBlock;
    private int countSum;
    private float lastCheckTime;
    private List<BlockData> myBlocks = new List<BlockData>();


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

    public void Rotation()
    {
        if (currentBlock == null) return;
        Transform blockTransform = currentBlock.transform;
        float currentRotation = blockTransform.localRotation.eulerAngles.z;
        blockTransform.localRotation = Quaternion.Euler(0f, 0f, currentRotation + 90f);
    }

    public void SetCurrentBlock(Rigidbody2D blockRb)
    {
        CurrentBlockRb = blockRb;
    }

    private void OnBlockSelected(GameObject blockPrefab)
    {
        spawnSystem.ShowSpawnPoints();
    }

    private void HandleInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        Vector2 touchPos = gameCamera.ScreenToWorldPoint(touch.position);
        Vector2 screenPos = touch.position;

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

        if (currentBlock != null)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (boxThrowField.OverlapPoint(touchPos))
                    {
                        if (CurrencySystem.Instance.CanAfford(currentBlock.GetCost()))
                        {
                            currentBlock.MarkAsCounted();
                            throwSystem.StartAiming(screenPos);
                        }
                    }
                    else
                    {
                        // Âðàùåíèå ïðè òàïå âíå çîíû áðîñêà
                        Rotation();
                    }
                    break;

                case TouchPhase.Moved:
                    if (throwSystem.IsThrowing)
                    {
                        throwSystem.UpdateAim(screenPos);
                        throwSystem.ChargePower();
                    }
                    break;

                case TouchPhase.Ended:
                    RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
                    if (throwSystem.IsThrowing)
                    {
                        CurrencySystem.Instance.SpendCurrency(currentBlock.GetCost());
                        currentBlock.MarkAsCounted();
                        throwSystem.ExecuteThrow(currentBlock);
                        currentBlock.transform.SetParent(null);
                        currentBlock = null;
                    }
                    break;
            }
        }
    }

    public void ResetCurrentBlock()
    {
        if (currentBlock != null)
        {
            currentBlock = null;
            CurrentBlockRb = null;
        }
    }

    //private void AddBlock(int blockId)
    //{
    //    BlockData newBlock = new BlockData { id = blockId, count = 1 };
    //    myBlocks.Add(newBlock);
    //}


    public void RemoveBlock(Block block)
    {
        if (currentBlock == block)
        {
            currentBlock = null;
        }
    }

   
}