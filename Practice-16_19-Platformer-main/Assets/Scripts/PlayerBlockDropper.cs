using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerBlockDropper : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField][Range(0, 1)] private float completionThreshold = 0.7f;
    [SerializeField] private float blockSpawnHeight = 10f;

    [Header("Throw Settings")]
    [SerializeField] private float minThrowAngle = 15f;
    [SerializeField] private float maxThrowAngle = 165f;
    [SerializeField] private float maxThrowPower = 20f;
    [SerializeField] private float powerIncreaseSpeed = 5f;

    [Header("Boundary Settings")]
    public Collider2D playAreaTrigger;
    [SerializeField] private Collider2D floorCollider;
    [SerializeField] private Collider2D wallColliderLeft;
    [SerializeField] private Collider2D wallColliderRight;

    [Header("UI References")]
    [SerializeField] private GameObject finishButton;
    [SerializeField] private BlockSelectionUI selectionUI;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Image progressBar;
    [SerializeField] private GameObject throwUI;
    [SerializeField] private Image angleArrow;
    [SerializeField] private Image powerBar;

    [Header("Effects")]
    [SerializeField] private ParticleSystem chargeEffect;
    [SerializeField] private AudioSource throwSound;

    private List<Block> activeBlocks = new List<Block>();
    private bool levelCompleted;
    private Block selectedBlock;
    private float lastCheckTime;

    private bool isAiming;
    private float currentAngle;
    private float currentPower;
    private Vector2 startTouchPosition;
    private Vector2 throwDirection;

    private void Start()
    {
        finishButton.SetActive(false);
        levelCompleted = false;
        activeBlocks.Clear();
        throwUI.SetActive(false);

        selectionUI.OnBlockSelected += SetSelectedBlock;

        if (playAreaTrigger == null)
        {
            playAreaTrigger = GetComponent<Collider2D>();
            if (playAreaTrigger == null)
            {
                Debug.LogError("PlayArea collider not assigned!");
            }
        }

        Application.targetFrameRate = 60;
        Physics2D.simulationMode = SimulationMode2D.Update;
    }

    private void Update()
    {
        if (isAiming)
        {
            HandleAiming();
            return;
        }

        if (selectedBlock != null)
        {
            UpdateBlockPreview();
        }

        if (Time.time - lastCheckTime > 0.5f)
        {
            CheckLevelCompletion();
            lastCheckTime = Time.time;
        }
    }

    private void UpdateProgressUI(float percentage)
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = percentage;
        }
    }

    public void SetSelectedBlock(GameObject blockPrefab)
    {
        if (selectedBlock != null)
        {
            Destroy(selectedBlock.gameObject);
        }

        Vector2 spawnPosition = GetSpawnPosition(Input.mousePosition);
        GameObject newBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        selectedBlock = newBlock.GetComponent<Block>();
        selectedBlock.Initialize(this, false);
    }

    public void StartAiming()
    {
        if (selectedBlock == null) return;

        isAiming = true;
        currentAngle = 45f;
        currentPower = 0f;
        throwUI.SetActive(true);
        chargeEffect.Play();
    }

    private void HandleAiming()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                startTouchPosition = touch.position;
                break;

            case TouchPhase.Moved:
                Vector2 delta = touch.position - startTouchPosition;

                currentAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
                currentAngle = Mathf.Clamp(currentAngle, minThrowAngle, maxThrowAngle);

                currentPower += powerIncreaseSpeed * Time.deltaTime;
                currentPower = Mathf.Clamp(currentPower, 0f, maxThrowPower);
                break;

            case TouchPhase.Ended:
                ExecuteThrow();
                break;
        }

        UpdateAimUI();

        if (currentPower >= maxThrowPower)
        {
            ExecuteThrow();
        }
    }

    private void UpdateAimUI()
    {
        angleArrow.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        powerBar.fillAmount = currentPower / maxThrowPower;

        if (currentPower > maxThrowPower * 0.8f)
        {
            chargeEffect.startColor = Color.red;
        }
        else
        {
            chargeEffect.startColor = Color.yellow;
        }
    }

    private void ExecuteThrow()
    {
        if (!isAiming || selectedBlock == null) return;

        float angleRad = currentAngle * Mathf.Deg2Rad;
        throwDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

        Rigidbody2D rb = selectedBlock.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(throwDirection * currentPower, ForceMode2D.Impulse);

        throwSound.Play();
        chargeEffect.Stop();

        isAiming = false;
        throwUI.SetActive(false);
        activeBlocks.Add(selectedBlock);
        selectedBlock = null;

        selectionUI.gameObject.SetActive(true);
    }

    public void CancelThrow()
    {
        if (!isAiming) return;

        isAiming = false;
        throwUI.SetActive(false);
        chargeEffect.Stop();
    }

    private void UpdateBlockPreview()
    {
        if (selectedBlock == null) return;

        Vector2 mousePos = gameCamera.ScreenToWorldPoint(Input.mousePosition);
        selectedBlock.transform.position = new Vector2(mousePos.x, mousePos.y + 1f);

        bool canPlace = playAreaTrigger.OverlapPoint(mousePos);
        SetBlockColor(selectedBlock, canPlace ? Color.white : new Color(1f, 0.5f, 0.5f, 0.7f));
    }

    private Vector2 GetSpawnPosition(Vector3 screenPosition)
    {
        Vector3 worldPos = gameCamera.ScreenToWorldPoint(screenPosition);
        Bounds bounds = playAreaTrigger.bounds;
        float clampedX = Mathf.Clamp(worldPos.x, bounds.min.x, bounds.max.x);
        return new Vector2(clampedX, bounds.max.y + blockSpawnHeight);
    }

    private void SetBlockColor(Block block, Color color)
    {
        foreach (var renderer in block.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.color = color;
        }
    }

    private void CheckLevelCompletion()
    {
        float fillPercentage = CalculateFilledPercentage();
        UpdateProgressUI(fillPercentage);

        if (fillPercentage >= completionThreshold && !finishButton.activeSelf)
        {
            finishButton.SetActive(true);
        }
    }

    private float CalculateFilledPercentage()
    {
        if (playAreaTrigger == null) return 0f;

        Bounds bounds = playAreaTrigger.bounds;
        float totalArea = bounds.size.x * bounds.size.y;
        float filledArea = 0f;

        Collider2D[] colliders = Physics2D.OverlapAreaAll(bounds.min, bounds.max);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag("Tetris") && collider != playAreaTrigger)
            {
                if (collider is BoxCollider2D box)
                {
                    filledArea += box.size.x * box.size.y;
                }
                else if (collider is PolygonCollider2D poly)
                {
                    filledArea += CalculatePolygonArea(poly.points);
                }
            }
        }

        return Mathf.Clamp01(filledArea / totalArea);
    }

    private float CalculatePolygonArea(Vector2[] points)
    {
        float area = 0f;
        int j = points.Length - 1;
        for (int i = 0; i < points.Length; j = i++)
        {
            area += (points[j].x + points[i].x) * (points[j].y - points[i].y);
        }
        return Mathf.Abs(area / 2f);
    }

    public void OnFinishButtonClicked()
    {
        finishButton.SetActive(false);
        levelCompleted = false;

        if (selectedBlock != null)
        {
            Destroy(selectedBlock.gameObject);
            selectedBlock = null;
        }
    }

    public void RemoveBlock(Block block)
    {
        activeBlocks.Remove(block);
    }
}