using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Block : MonoBehaviour
{
    [SerializeField] private BlockType type = BlockType.Standard; // Установлен стандартный тип по умолчанию
    private PlayerBlockDropper gameController;
    private Rigidbody2D rb;
    private bool isPlaced;

    public void Initialize(PlayerBlockDropper controller, bool withPhysics)
    {
        gameController = controller;
        rb = GetComponent<Rigidbody2D>();

        if (withPhysics)
        {
            EnablePhysics();
        }
        else
        {
            DisablePhysics();
        }

        ConfigurePhysicsBasedOnType();
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

    private void ConfigurePhysicsBasedOnType()
    {
        switch (type)
        {
            case BlockType.Heavy:
                rb.mass = 5f;
                break;
            case BlockType.Bouncy:
                var bouncyMat = new PhysicsMaterial2D("Bouncy")
                {
                    bounciness = 0.8f,
                    friction = 0.1f
                };
                rb.sharedMaterial = bouncyMat;
                break;
            case BlockType.Sticky:
                var stickyMat = new PhysicsMaterial2D("Sticky")
                {
                    bounciness = 0.1f,
                    friction = 0.9f
                };
                rb.sharedMaterial = stickyMat;
                break;
            default:
                rb.mass = 1f;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPlaced) return;

        if (!collision.collider.CompareTag("Block"))
        {
            rb.linearVelocity = Vector2.zero;
            isPlaced = true;
        }
    }

    private void OnDestroy()
    {
        if (gameController != null)
        {
            gameController.RemoveBlock(this);
        }
    }
}