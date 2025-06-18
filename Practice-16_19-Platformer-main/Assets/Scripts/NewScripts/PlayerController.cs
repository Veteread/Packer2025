using Mirror;
using UnityEngine;
using Cinemachine;

public class PlayerController : NetworkBehaviour
{
    [SyncVar] private string playerName;
    private CinemachineVirtualCamera playerCamera;

    public override void OnStartLocalPlayer()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Player_" + Random.Range(1000, 9999));
        SetupCinemachineCamera();
        UIManager.Instance.SetLocalPlayer(this);
    }

    private void SetupCinemachineCamera()
    {
        playerCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (playerCamera != null)
        {
            playerCamera.Follow = transform;
            playerCamera.LookAt = transform;
        }
    }

    [Command]
    public void CmdStartDuel(uint targetPlayerId) => DuelSystem.Instance.InitiateDuel(netId, targetPlayerId);

    [Command]
    public void CmdCreateContract(string contractJson) => ContractSystem.Instance.CmdCreateContract(contractJson);
}