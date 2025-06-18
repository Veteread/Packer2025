using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        // ������������ ������� ������
        PlayerPrefs.SetInt("ModuleUnlocked_basic_weapon", 1);
        PlayerPrefs.SetInt("ModuleUnlocked_basic_engine", 1);
        PlayerPrefs.Save();

        // �������������� ���������
        if (PlayerInventory.Instance == null)
        {
            GameObject inventoryObj = new GameObject("PlayerInventory");
            inventoryObj.AddComponent<PlayerInventory>();
        }
    }
}
