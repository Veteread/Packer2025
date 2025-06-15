using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Block : MonoBehaviour
{
    [SerializeField] private BlockType type = BlockType.Standard;
    private PlayerBlockDropper controller;
    private Rigidbody2D rb;
    private bool isPlaced;
    [SerializeField] private int cost = 10;
    public bool WasCounted { get; private set; }

    public void Initialize(PlayerBlockDropper controller, bool withPhysics)
    {
        this.controller = controller;
        rb = GetComponent<Rigidbody2D>();
        ConfigurePhysics();

        if (withPhysics) EnablePhysics();
        else DisablePhysics();
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
        isPlaced = false;
    }

    public void DisablePhysics()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
    }

    public void EnablePhysicsForThrow()
    {
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1f;
            rb.angularDamping = 0.05f;
            rb.linearDamping = 0.1f;
        }
    }

    private void OnDestroy()
    {
        if (!WasCounted)
        {
            CurrencySystem.Instance?.Refund(cost);
            Debug.Log($"Возвращено {cost} валюты за блок");
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