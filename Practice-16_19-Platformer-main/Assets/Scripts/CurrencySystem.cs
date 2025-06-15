using UnityEngine;
using UnityEngine.UI;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance { get; private set; }

    [SerializeField] private Text currencyText;
    private int currentCurrency;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCurrency();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public bool CanAfford(int amount)
    {
        return currentCurrency >= amount;
    }

    public bool SpendCurrency(int amount)
    {
        if (CanAfford(amount))
        {
            currentCurrency -= amount;
            SaveCurrency();
            UpdateUI();
            return true;
        }
        return false;
    }

    public void CookedInShop(int amount)
    {
        currentCurrency -= amount;
            SaveCurrency();
            UpdateUI();
    }

    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        SaveCurrency();
        UpdateUI();
    }

    public void Refund(int amount)
    {
        AddCurrency(amount);
        Debug.Log($"¬озвращено {amount} валюты");
    }

    private void LoadCurrency()
    {
        currentCurrency = PlayerPrefs.GetInt("PlayerCurrency", 100); // 100 - стартовый баланс
    }

    public void SaveCurrency()
    {
        PlayerPrefs.SetInt("PlayerCurrency", currentCurrency);
    }

    private void UpdateUI()
    {
        if (currencyText != null)
        {
            currencyText.text = currentCurrency.ToString();
        }
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("PlayerCurrency");
        LoadCurrency();
        UpdateUI();
    }
}