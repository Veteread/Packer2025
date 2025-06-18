using Mirror;
using UnityEngine;

public class SpaceshipController : NetworkBehaviour
{
    [SyncVar] private uint ownerId;
    [SyncVar] private float health = 100f;

    public void SetOwner(uint playerId)
    {
        ownerId = playerId;
    }

    [Server]
    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            // Завершаем дуэль
            //DuelSystem.Instance.EndDuel(ownerId == DuelSystem.Instance.Player1Id ?
            //    DuelSystem.Instance.Player2Id :
            //    DuelSystem.Instance.Player1Id);

            NetworkServer.Destroy(gameObject);
        }
    }

    // Выстрел с проверкой владельца
    [Command]
    public void CmdFire()
    {
        if (isServer)
        {
            // Логика выстрела
            // ...
        }
    }
}