using UnityEngine;
using System.Collections.Generic;

public class ModuleDatabase : MonoBehaviour
{
    public static ModuleDatabase Instance { get; private set; }

    public List<ModuleData2D> allModules;
    private Dictionary<string, ModuleData2D> moduleDict = new Dictionary<string, ModuleData2D>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDatabase()
    {
        // Инициализация списка если он null
        if (allModules == null)
        {
            allModules = new List<ModuleData2D>();
            Debug.LogWarning("allModules was null - initialized new list");
        }

        moduleDict.Clear();
        foreach (ModuleData2D module in allModules)
        {
            if (module != null)
            {
                if (!moduleDict.ContainsKey(module.id))
                {
                    moduleDict.Add(module.id, module);
                }
                else
                {
                    Debug.LogWarning($"Duplicate module ID: {module.id}");
                }
            }
        }
    }

    public ModuleData2D GetModuleById(string id)
    {
        return moduleDict.ContainsKey(id) ? moduleDict[id] : null;
    }

    public List<ModuleData2D> GetAvailableModules()
    {
        List<ModuleData2D> available = new List<ModuleData2D>();

        if (allModules == null)
        {
            Debug.LogError("allModules is null!");
            return available;
        }

        foreach (var module in allModules)
        {
            if (module != null)
            {
                if (PlayerPrefs.GetInt("ModuleUnlocked_" + module.id, 0) == 1)
                {
                    available.Add(module);
                }
            }
        }
        return available;
    }
}