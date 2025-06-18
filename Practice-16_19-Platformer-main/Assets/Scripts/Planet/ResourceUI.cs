using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private Transform resourceContainer;
    [SerializeField] private ResourceDisplay resourceDisplayPrefab;

    private Dictionary<string, ResourceDisplay> displays = new Dictionary<string, ResourceDisplay>();

    private void Start()
    {
        UpdateResourceDisplay();

        // Подписываемся на обновления инвентаря
        PlayerInventory.Instance.OnInventoryChanged += UpdateResourceDisplay;
    }

    private void OnDestroy()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged -= UpdateResourceDisplay;
        }
    }

    public void UpdateResourceDisplay()
    {
        if (PlayerInventory.Instance == null) return;

        Dictionary<string, int> resources = PlayerInventory.Instance.GetAllResources();

        // Обновляем или создаем отображение
        foreach (var resource in resources)
        {
            if (displays.ContainsKey(resource.Key))
            {
                displays[resource.Key].UpdateAmount(resource.Value);
            }
            else
            {
                ResourceDisplay display = Instantiate(resourceDisplayPrefab, resourceContainer);
                display.Initialize(resource.Key, resource.Value);
                displays.Add(resource.Key, display);
            }
        }

        // Удаляем старые ресурсы
        List<string> toRemove = new List<string>();
        foreach (var display in displays)
        {
            if (!resources.ContainsKey(display.Key))
            {
                Destroy(display.Value.gameObject);
                toRemove.Add(display.Key);
            }
        }

        foreach (string key in toRemove)
        {
            displays.Remove(key);
        }
    }
}