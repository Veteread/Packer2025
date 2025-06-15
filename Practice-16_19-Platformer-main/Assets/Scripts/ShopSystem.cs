using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public struct ShopItem
{
    public string name;
    public string description;
    public int price;
    public Sprite icon;
    public GameObject needActive;
}

public class ShopSystem : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private List<ShopItem> items = new List<ShopItem>();

    [Header("UI References")]
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text itemPriceText;
    [SerializeField] private Button buyButton;

    private ShopItem selectedItem;
    private int intPrice;
    private GameObject activeItem;

    private void Start()
    {
        CreateItemButtons();
        infoPanel.SetActive(false);
        buyButton.onClick.AddListener(BuyItem);
    }

    private void CreateItemButtons()
    {
        foreach (var item in items)
        {
            GameObject buttonGO = Instantiate(itemButtonPrefab, itemsContainer);
            ItemButton itemButton = buttonGO.GetComponent<ItemButton>();
            
            if (itemButton != null)
            {
                itemButton.Initialize(item.icon, () => OnItemSelected(item));
            }
        }
    }

    private void OnItemSelected(ShopItem item)
    {
        selectedItem = item;
        infoPanel.SetActive(true);
        
        itemNameText.text = item.name;
        itemDescriptionText.text = item.description;
        itemPriceText.text = $"Price: {item.price}";
        intPrice = item.price;
        activeItem = item.needActive;
    }

    private void BuyItem()
    {
        if (selectedItem.price <= intPrice) // Реализуйте свою логику валюты
        {
            Debug.Log($"Purchased: {selectedItem.name}");
            CurrencySystem.Instance.CookedInShop(intPrice);
            if(activeItem!=null)
            {
            	activeItem.SetActive(true);
            }            
            infoPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }
}