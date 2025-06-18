using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }
    public System.Action OnInventoryChanged;

    [System.Serializable]
    public struct ResourceItem
    {
        public string id;
        public int amount;
    }

    private List<ResourceItem> resources = new List<ResourceItem>();

    private void Awake()
    {
        // Реализация синглтона
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddResource(string resourceId, int amount)
    {
        int index = resources.FindIndex(r => r.id == resourceId);
        if (index >= 0)
        {
            ResourceItem item = resources[index];
            item.amount += amount;
            resources[index] = item;
        }
        else
        {
            resources.Add(new ResourceItem { id = resourceId, amount = amount });
        }
        SaveInventory();
        if (OnInventoryChanged != null)
        {
            OnInventoryChanged();
        }
    }

    public bool HasResources(ContractSystem.ContractResource[] requiredResources)
    {
        Dictionary<string, int> resourceDict = GetAllResources();

        foreach (var required in requiredResources)
        {
            if (!resourceDict.TryGetValue(required.id, out int amount) || amount < required.amount)
                return false;
        }

        return true;
    }

    public int GetResourceCount(string resourceId)
    {
        int index = resources.FindIndex(r => r.id == resourceId);
        return index >= 0 ? resources[index].amount : 0;
    }

    public Dictionary<string, int> GetAllResources()
    {
        Dictionary<string, int> resourceDict = new Dictionary<string, int>();
        foreach (var item in resources)
        {
            resourceDict[item.id] = item.amount;
        }
        return resourceDict;
    }

    private void SaveInventory()
    {
        List<ResourceSaveData> saveData = new List<ResourceSaveData>();
        foreach (var item in resources)
        {
            saveData.Add(new ResourceSaveData { id = item.id, amount = item.amount });
        }

        PlayerPrefs.SetString("PlayerInventory", JsonUtility.ToJson(new InventoryData { resources = saveData }));
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        if (PlayerPrefs.HasKey("PlayerInventory"))
        {
            InventoryData data = JsonUtility.FromJson<InventoryData>(PlayerPrefs.GetString("PlayerInventory"));
            resources.Clear();

            foreach (var item in data.resources)
            {
                resources.Add(new ResourceItem { id = item.id, amount = item.amount });
            }
        }
    }

    [System.Serializable]
    private class InventoryData
    {
        public List<ResourceSaveData> resources = new List<ResourceSaveData>();
    }

    [System.Serializable]
    private class ResourceSaveData
    {
        public string id;
        public int amount;
    }
}