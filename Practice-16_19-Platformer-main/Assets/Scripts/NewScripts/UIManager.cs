using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Duel UI")]
    [SerializeField] private GameObject duelInvitationPanel;
    [SerializeField] private TMP_Text invitationText;

    [Header("Resource UI")]
    [SerializeField] private Transform resourceContainer;
    [SerializeField] private ResourceDisplay resourcePrefab;

    [Header("Loading Screen")]
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Slider loadingBar;

    private PlayerController localPlayer;
    private Dictionary<string, ResourceDisplay> resourceDisplays = new Dictionary<string, ResourceDisplay>();

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

    public void SetLocalPlayer(PlayerController player) => localPlayer = player;

    public void ShowDuelInvitation(uint challengerId)
    {
        duelInvitationPanel.SetActive(true);
        invitationText.text = $"Player {challengerId} invites you to a duel!";
    }

    public void ShowDuelResult(bool isWinner)
    {
        // Implement duel result UI
    }

    public void UpdateResourceDisplay(Dictionary<string, int> resources)
    {
        ClearResourceDisplays();

        foreach (var resource in resources)
        {
            ResourceDisplay display = Instantiate(resourcePrefab, resourceContainer);
            display.Initialize(resource.Key, resource.Value);
            resourceDisplays.Add(resource.Key, display);
        }
    }

    public void UpdateContractsDisplay(List<ContractSystem.ContractData> contracts)
    {
        // Implement contract list UI
    }

    public void ShowLoadingScreen(bool show, string message = "", float progress = 0)
    {
        if (loadingText)
        {
            loadingText.gameObject.SetActive(show);
            loadingText.text = message;
        }
        if (loadingBar)
        {
            loadingBar.gameObject.SetActive(show);
            loadingBar.value = progress;
        }
    }

    private void ClearResourceDisplays()
    {
        foreach (var display in resourceDisplays.Values)
            Destroy(display.gameObject);
        resourceDisplays.Clear();
    }

    public void CreateContract(ContractSystem.ContractResource[] resources, int reward)
    {
        ContractSystem.ContractData contractData = new ContractSystem.ContractData
        {
            creatorId = localPlayer.netId,
            requiredResources = resources,
            reward = reward
        };
        localPlayer.CmdCreateContract(JsonUtility.ToJson(contractData));
    }
}