// Исправленный ModuleCreator.cs
using UnityEngine;

public class ModuleCreator : MonoBehaviour
{
    void Start()
    {
        // Убедимся, что база данных существует
        if (ModuleDatabase.Instance == null)
        {
            Debug.LogError("ModuleDatabase instance is missing!");
            return;
        }

        // Создаем тестовый модуль
        ModuleData2D testModule = ScriptableObject.CreateInstance<ModuleData2D>();
        testModule.id = "test_module";
        testModule.displayName = "Test Module";
        testModule.color = Color.red;
        testModule.cost = 50;
        testModule.type = ModuleType.Decoration;

        // Создаем спрайт программно
        testModule.sprite = CreateBasicSprite();

        // Добавляем в базу данных
        ModuleDatabase.Instance.allModules.Add(testModule);

        // Разблокируем модуль
        PlayerPrefs.SetInt("ModuleUnlocked_test_module", 1);
    }

    private Sprite CreateBasicSprite()
    {
        // Создаем простой квадратный спрайт
        Texture2D texture = new Texture2D(64, 64);
        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                texture.SetPixel(x, y, Color.red);
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 64, 64), Vector2.one * 0.5f);
    }
}