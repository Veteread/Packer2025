using UnityEngine;

[System.Serializable]
public class ModuleSlot
{
    public Vector2 position;
    public Vector2 direction;

    // Конструктор для удобного создания
    public ModuleSlot(Vector2 pos, Vector2 dir)
    {
        position = pos;
        direction = dir;
    }
}