using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    [System.Serializable]
    public class ShopItem
    {
        public string name;
        public int cost;
        public GameObject blockPrefab;
        public Button buyButton;
    }

    [SerializeField] private ShopItem[] shopItems;
    [SerializeField] private Transform blockSpawnPoint;

    private void Start()
    {
        UpdateShopUI();
    }

    public void PurchaseItem(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= shopItems.Length) return;

        ShopItem item = shopItems[itemIndex];
        if (CurrencySystem.Instance.SpendCurrency(item.cost))
        {
            Instantiate(item.blockPrefab, blockSpawnPoint.position, Quaternion.identity);
            UpdateShopUI();
        }
    }

    private void UpdateShopUI()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopItems[i].buyButton.interactable =
                CurrencySystem.Instance.CanAfford(shopItems[i].cost);
        }
    }
}