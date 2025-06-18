using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameObject playerBasePrefab;
    [SerializeField] private GameObject duelManagerPrefab;
    [SerializeField] private GameObject contractSystemPrefab;

    public static MyNetworkManager Instance { get; private set; }

    private new void Awake()
    {
        base.Awake();

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

    public override void OnStartServer()
    {
        base.OnStartServer();

        GameObject duelManager = Instantiate(duelManagerPrefab);
        NetworkServer.Spawn(duelManager);

        GameObject contractSystem = Instantiate(contractSystemPrefab);
        NetworkServer.Spawn(contractSystem);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerBasePrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public void ChangeSceneForAll(string sceneName) => ServerChangeScene(sceneName);

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (sceneName == "DuelArena" && DuelSystem.Instance != null)
        {
            DuelSystem.Instance.OnDuelSceneLoaded();
        }
    }
}