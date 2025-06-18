using UnityEngine;

[CreateAssetMenu(fileName = "ModuleData", menuName = "Ship/Module2D")]
public class ModuleData2D : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite sprite;
    public Color color = Color.white;
    public int cost;
    public ModuleType type;
}

public enum ModuleType
{
    Weapon, Engine, Scanner, Storage, Decoration
}