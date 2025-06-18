using Mirror;
using UnityEngine;
using System.Collections;

public class DuelSystem : NetworkBehaviour
{
    public static DuelSystem Instance { get; private set; }

    [SerializeField] private GameObject spaceshipPrefab;

    [SyncVar] public uint Player1Id;
    [SyncVar] public uint Player2Id;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Server]
    public void InitiateDuel(uint challengerId, uint opponentId)
    {
        Player1Id = challengerId;
        Player2Id = opponentId;
        TargetShowDuelInvite(challengerId, opponentId);
    }

    [TargetRpc]
    private void TargetShowDuelInvite(uint challengerId, uint opponentId)
    {
        if (NetworkClient.localPlayer.netId == opponentId)
            UIManager.Instance.ShowDuelInvitation(challengerId);
    }

    [Command]
    public void CmdAcceptDuel(uint acceptingPlayerId)
    {
        if (acceptingPlayerId == Player2Id)
        {
            MyNetworkManager.Instance.ChangeSceneForAll("DuelArena");
        }
    }

    public void OnDuelSceneLoaded() => StartCoroutine(SpawnShipsAfterDelay());

    private IEnumerator SpawnShipsAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        NetworkIdentity player1 = NetworkServer.spawned[Player1Id];
        NetworkIdentity player2 = NetworkServer.spawned[Player2Id];

        if (player1 == null || player2 == null) yield break;

        Vector3 pos1 = new Vector3(-10, 0, 0);
        Vector3 pos2 = new Vector3(10, 0, 0);

        GameObject ship1 = Instantiate(spaceshipPrefab, pos1, Quaternion.identity);
        NetworkServer.Spawn(ship1, player1.connectionToClient);

        GameObject ship2 = Instantiate(spaceshipPrefab, pos2, Quaternion.identity);
        NetworkServer.Spawn(ship2, player2.connectionToClient);

        player1.gameObject.SetActive(false);
        player2.gameObject.SetActive(false);
    }

    [Server]
    public void EndDuel(uint winnerId)
    {
        NetworkIdentity player1 = NetworkServer.spawned[Player1Id];
        NetworkIdentity player2 = NetworkServer.spawned[Player2Id];

        if (player1 != null) player1.gameObject.SetActive(true);
        if (player2 != null) player2.gameObject.SetActive(true);

        MyNetworkManager.Instance.ChangeSceneForAll("MainScene");

        TargetDuelEnded(player1.connectionToClient, winnerId == Player1Id);
        TargetDuelEnded(player2.connectionToClient, winnerId == Player2Id);
    }

    [TargetRpc]
    private void TargetDuelEnded(NetworkConnection target, bool isWinner)
    {
        UIManager.Instance.ShowDuelResult(isWinner);
    }
}