using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    [System.Serializable]
    public class TargetBlock
    {
        public BlockShape shape;
        public Vector2Int gridPosition;
        public int rotationIndex;
    }

    [Header("Основные настройки")]
    public string levelName;
    public int levelID;
    public Sprite background;
   
    [Header("Целевые блоки")]
    public List<TargetBlock> targetBlocks = new List<TargetBlock>();

    [Header("Параметры точности")]
    [Range(0.5f, 1f)] public float requiredAccuracy = 0.85f;

    [Header("Визуализация")]
    public Color ghostColor = new Color(1f, 1f, 1f, 0.3f);
    
    [Header("Условия победы")]
    public WinCondition winCondition;
    [Range(0.1f, 1f)] public float targetFillPercentage = 0.7f;
    public int targetBlocksCount = 20;
    
    [Header("Физика")]
    [Range(0.5f, 5f)] public float fallSpeed = 1f;
    [Range(0.1f, 2f)] public float rotationSpeed = 1f;
    
    [Header("Особые условия")]
    public bool useTimeLimit;
    public float timeLimit = 120f;
    
    public enum WinCondition
    {
        FillPercentage,
        BlocksCount
    }
}