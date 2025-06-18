using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class ContractSystem : NetworkBehaviour
{
    public static ContractSystem Instance { get; private set; }  // Добавлено статическое свойство

    [System.Serializable]
    public struct ContractResource
    {
        public string id;
        public int amount;
    }

    [System.Serializable]
    public struct ContractData
    {
        public uint creatorId;
        public ContractResource[] requiredResources;
        public int reward;
    }

    [SyncVar(hook = nameof(OnContractsChanged))]
    private string contractsJson;

    private List<ContractData> contracts = new List<ContractData>();

    private void Awake()
    {
        // Инициализация синглтона
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

    [Command]
    public void CmdCreateContract(string contractJson)
    {
        ContractData contractData = JsonUtility.FromJson<ContractData>(contractJson);
        PlayerInventory creatorInventory = NetworkServer.spawned[contractData.creatorId].GetComponent<PlayerInventory>();

        if (creatorInventory.HasResources(contractData.requiredResources))
        {
            contracts.Add(contractData);
            UpdateSyncVar();
        }
    }

    [Command]
    public void CmdAcceptContract(int contractId, uint acceptorId)
    {
        ContractData contract = contracts[contractId];
        PlayerInventory acceptorInventory = NetworkServer.spawned[acceptorId].GetComponent<PlayerInventory>();

        if (acceptorInventory.HasResources(contract.requiredResources))
        {
            foreach (var resource in contract.requiredResources)
                acceptorInventory.AddResource(resource.id, -resource.amount);

            PlayerInventory creatorInventory = NetworkServer.spawned[contract.creatorId].GetComponent<PlayerInventory>();
            creatorInventory.AddResource("currency", contract.reward);

            contracts.RemoveAt(contractId);
            UpdateSyncVar();
        }
    }

    private void UpdateSyncVar() => contractsJson = JsonUtility.ToJson(new SerializableContractList(contracts));

    private void OnContractsChanged(string oldJson, string newJson)
    {
        contracts = JsonUtility.FromJson<SerializableContractList>(newJson).contracts;
        if (UIManager.Instance != null) UIManager.Instance.UpdateContractsDisplay(contracts);
    }

    [System.Serializable]
    private class SerializableContractList
    {
        public List<ContractData> contracts;
        public SerializableContractList(List<ContractData> contracts) => this.contracts = contracts;
    }
}