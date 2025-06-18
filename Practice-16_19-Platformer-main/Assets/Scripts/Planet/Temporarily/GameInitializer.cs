using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        // Разблокируем базовые модули
        PlayerPrefs.SetInt("ModuleUnlocked_basic_weapon", 1);
        PlayerPrefs.SetInt("ModuleUnlocked_basic_engine", 1);
        PlayerPrefs.Save();

        // Инициализируем инвентарь
        if (PlayerInventory.Instance == null)
        {
            GameObject inventoryObj = new GameObject("PlayerInventory");
            inventoryObj.AddComponent<PlayerInventory>();
        }
    }
}
