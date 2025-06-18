using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text countText;

    public void Initialize(string resourceName, int amount)
    {
        nameText.text = resourceName;
        countText.text = amount.ToString();
    }

    // Добавляем метод UpdateAmount
    public void UpdateAmount(int newAmount)
    {
        countText.text = newAmount.ToString();
    }
}