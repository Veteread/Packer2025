using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipCustomizationUI2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Ship2D ship;
    [SerializeField] private Transform moduleButtonsContainer;
    [SerializeField] private Button[] slotButtons;
    [SerializeField] private Button colorButton;
    [SerializeField] private Button packButton;

    [Header("Prefabs")]
    [SerializeField] private ModuleButton2D moduleButtonPrefab;

    private int selectedSlotIndex = -1;

    private void Start()
    {
        // Проверка ссылок
        if (ship == null)
            ship = FindObjectOfType<Ship2D>();

        if (ship == null)
        {
            Debug.LogError("Ship2D reference is missing!");
            return;
        }

        // Проверка кнопок
        if (slotButtons == null || slotButtons.Length == 0)
        {
            Debug.LogWarning("Slot buttons not assigned!");
        }
        else
        {
            for (int i = 0; i < slotButtons.Length; i++)
            {
                int index = i;
                slotButtons[i].onClick.AddListener(() => SelectSlot(index));
            }
        }

        colorButton?.onClick.AddListener(ChangeColor);
        packButton?.onClick.AddListener(StartResourcePacking);

        LoadAvailableModules();
    }

    private void SelectSlot(int slotIndex)
    {
        selectedSlotIndex = slotIndex;
        HighlightSlot(slotIndex);
    }

    private void HighlightSlot(int slotIndex)
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            slotButtons[i].image.color = (i == slotIndex)
                ? Color.yellow
                : Color.white;
        }
    }

    private void LoadAvailableModules()
    {
        // Проверка контейнера
        if (moduleButtonsContainer == null)
        {
            Debug.LogError("Module buttons container is not assigned!");
            return;
        }

        // Очистка контейнера
        foreach (Transform child in moduleButtonsContainer)
            Destroy(child.gameObject);

        // Проверка базы данных
        if (ModuleDatabase.Instance == null)
        {
            Debug.LogError("ModuleDatabase instance is missing!");
            return;
        }

        // Загрузка модулей
        var availableModules = ModuleDatabase.Instance.GetAvailableModules();
        if (availableModules == null)
        {
            Debug.LogWarning("Available modules list is null!");
            return;
        }

        // Проверка префаба
        if (moduleButtonPrefab == null)
        {
            Debug.LogError("Module button prefab is not assigned!");
            return;
        }

        foreach (ModuleData2D moduleData in availableModules)
        {
            if (moduleData == null)
            {
                Debug.LogWarning("Found null module in available modules list!");
                continue;
            }

            ModuleButton2D button = Instantiate(moduleButtonPrefab, moduleButtonsContainer);
            button.Initialize(moduleData, () => AttachModule(moduleData));
        }
    }

    private void AttachModule(ModuleData2D moduleData)
    {
        if (selectedSlotIndex == -1 || selectedSlotIndex >= ship.slots.Count)
        {
            Debug.LogWarning("No slot selected!");
            return;
        }

        ModuleSlot slot = ship.slots[selectedSlotIndex];
        ship.AttachModule(slot, moduleData);
    }

    private void ChangeColor()
    {
        ship.ChangeColor(new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
        ));
    }

    private void StartResourcePacking()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("ResourcePackingScene");
    }
}