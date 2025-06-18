using UnityEngine;
using System.Collections.Generic;

public class Ship2D : MonoBehaviour
{
    [Header("Core Settings")]
    [SerializeField] private Color coreColor = Color.blue;
    [SerializeField] private Vector2 coreSize = new Vector2(1, 1);

    [Header("Module Slots")]
    public List<ModuleSlot> slots = new List<ModuleSlot>();

    private SpriteRenderer coreRenderer;
    private Dictionary<ModuleSlot, GameObject> attachedModules = new Dictionary<ModuleSlot, GameObject>();

    [System.Serializable]
    public class ShipDesign
    {
        public Color coreColor;
        public List<ModuleSaveData> modules = new List<ModuleSaveData>();
    }

    [System.Serializable]
    public class ModuleSaveData
    {
        public int slotIndex;
        public string moduleId;
    }

    private void Start()
    {
        GenerateCore();
        InitializeSlots();
        LoadShipDesign();
    }

    // Исправленный метод GenerateCore в Ship2D.cs
    private void GenerateCore()
    {
        coreRenderer = gameObject.AddComponent<SpriteRenderer>();

        // Создаем текстуру 1x1 пиксель
        Texture2D coreTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        coreTexture.SetPixel(0, 0, coreColor);
        coreTexture.Apply();

        // Устанавливаем спрайт
        coreRenderer.sprite = Sprite.Create(
            coreTexture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f)
        );

        // Используем Sliced режим для масштабирования
        coreRenderer.drawMode = SpriteDrawMode.Sliced;
        coreRenderer.size = coreSize;
    }

    private void InitializeSlots()
    {
        // Очистка перед инициализацией
        slots.Clear();

        // 4 стороны для 2D
        Vector2[] directions = {
            Vector2.up, Vector2.down,
            Vector2.left, Vector2.right
        };

        foreach (Vector2 dir in directions)
        {
            slots.Add(new ModuleSlot(
                dir * (coreSize.magnitude / 2),
                dir
            ));
        }
    }

    public void AttachModule(ModuleSlot slot, ModuleData2D moduleData)
    {
        if (attachedModules.ContainsKey(slot))
        {
            Destroy(attachedModules[slot]);
            attachedModules.Remove(slot);
        }

        GameObject moduleObj = new GameObject(moduleData.displayName);
        moduleObj.transform.SetParent(transform);
        moduleObj.transform.localPosition = slot.position;

        SpriteRenderer renderer = moduleObj.AddComponent<SpriteRenderer>();
        renderer.sprite = moduleData.sprite;
        renderer.color = moduleData.color;
        renderer.sortingOrder = 1;

        // Поворот модуля в соответствии с направлением
        float angle = Mathf.Atan2(slot.direction.y, slot.direction.x) * Mathf.Rad2Deg;
        moduleObj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        attachedModules.Add(slot, moduleObj);
        SaveShipDesign();
    }

    public void ChangeColor(Color newColor)
    {
        coreRenderer.color = newColor;
        SaveShipDesign();
    }

    private void SaveShipDesign()
    {
        ShipDesign design = new ShipDesign();
        design.coreColor = coreRenderer.color;

        foreach (var kvp in attachedModules)
        {
            ModuleSaveData data = new ModuleSaveData();
            data.slotIndex = slots.IndexOf(kvp.Key);
            data.moduleId = kvp.Value.name;
            design.modules.Add(data);
        }

        PlayerPrefs.SetString("ShipDesign", JsonUtility.ToJson(design));
    }

    private void LoadShipDesign()
    {
        if (!PlayerPrefs.HasKey("ShipDesign")) return;

        ShipDesign design = JsonUtility.FromJson<ShipDesign>(
            PlayerPrefs.GetString("ShipDesign")
        );

        coreRenderer.color = design.coreColor;

        foreach (ModuleSaveData data in design.modules)
        {
            if (data.slotIndex >= 0 && data.slotIndex < slots.Count)
            {
                ModuleSlot slot = slots[data.slotIndex];
                ModuleData2D moduleData = ModuleDatabase.Instance.GetModuleById(data.moduleId);
                if (moduleData != null)
                {
                    AttachModule(slot, moduleData);
                }
            }
        }
    }

    private void OnDestroy()
    {
        SpriteManager.Cleanup();
    }
}