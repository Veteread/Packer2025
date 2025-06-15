using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Block : MonoBehaviour
{
    [SerializeField] private BlockType type = BlockType.Standard;
    private PlayerBlockDropper controller;
    private Rigidbody2D rb;
    public GameObject OriginalPrefab { get; set; }

    [SerializeField] private int cost = 10;
    public int idBlock;
    public bool WasCounted { get; private set; }
    public bool IsMatched { get;  set; } = false;
    
    [SerializeField] private BlockShape shape;
    public BlockShape Shape => shape;
    private Grid basketGrid;

    private void Awake()
    {
        // Найти Grid в сцене
        basketGrid = FindObjectOfType<Grid>();
    }
    public Vector2Int[] GetCurrentCells()
    {
        if (basketGrid == null)
        {
            basketGrid = FindObjectOfType<Grid>();
            if (basketGrid == null) return new Vector2Int[0];
        }

        Vector3Int baseCell = basketGrid.WorldToCell(transform.position);
        Vector2Int[] shapeCells = shape.GetRotatedCells(GetRotationIndex());

        // Рассчитываем центр формы для правильной нормализации
        Vector2 center = Vector2.zero;
        foreach (Vector2Int cell in shapeCells)
        {
            center.x += cell.x;
            center.y += cell.y;
        }
        center /= shapeCells.Length;

        // Смещаем к целым координатам
        int offsetX = Mathf.RoundToInt(center.x);
        int offsetY = Mathf.RoundToInt(center.y);

        Vector2Int[] cells = new Vector2Int[shapeCells.Length];
        for (int i = 0; i < shapeCells.Length; i++)
        {
            cells[i] = new Vector2Int(
                baseCell.x + shapeCells[i].x - offsetX,
                baseCell.y + shapeCells[i].y - offsetY
            );
        }

        return cells;
    }

    public Vector2Int GridPosition
    {
        get
        {
            Vector3Int cellPos = basketGrid.WorldToCell(transform.position);
            return new Vector2Int(cellPos.x, cellPos.y);
        }
    }

    public void Initialize(PlayerBlockDropper controller, bool withPhysics)
    {
        this.controller = controller;
        rb = GetComponent<Rigidbody2D>();
        ConfigurePhysics();

        if (withPhysics) EnablePhysics();
        else DisablePhysics();
    }

    public int GetRotationIndex()
    {
        float angle = transform.rotation.eulerAngles.z;
        angle = angle % 360;
        if (angle < 0) angle += 360;
        return Mathf.FloorToInt((angle + 45) / 90) % 4;
    }


    public Vector2Int[] GetRotatedCells(int rotationIndex)
    {
        if (shape == null)
        {
            Debug.LogError("Block shape is not assigned!");
            return new Vector2Int[0];
        }

        // Используем метод из BlockShape
        return shape.GetRotatedCells(rotationIndex);
    }

    private void ConfigurePhysics()
    {
        switch (type)
        {
            case BlockType.Heavy:
                rb.mass = 5f;
                break;
            case BlockType.Bouncy:
                rb.sharedMaterial = new PhysicsMaterial2D
                {
                    bounciness = 0.8f,
                    friction = 0.1f
                };
                break;
            case BlockType.Sticky:
                rb.sharedMaterial = new PhysicsMaterial2D
                {
                    bounciness = 0.1f,
                    friction = 0.9f
                };
                break;
        }
    }

    public void EnablePhysics()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void DisablePhysics()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
    }

    public void EnablePhysicsForThrow()
    {
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void OnDestroy()
    {
        if (!WasCounted)
        {
            CurrencySystem.Instance?.Refund(cost);
            Debug.Log($"Âîçâðàùåíî {cost} âàëþòû çà áëîê");
        }
    }
    public void MarkAsCounted()
    {
        WasCounted = true;
    }

    public int GetCost()
    {
        return cost;
    }
}