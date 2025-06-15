using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private List<LevelData> allLevels;
    [SerializeField] private GhostVisualizer ghostVisualizer;
    [SerializeField] private Color matchColor = Color.green;

    private LevelData currentLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= allLevels.Count) return;

        currentLevel = allLevels[levelIndex];
        ghostVisualizer.VisualizeGhosts(currentLevel);
    }

    public void CheckAllBlocksAgainstGhosts()
    {
        var allBlocks = FindObjectsOfType<Block>();
        bool anyBlockMoving = false;

        // Проверяем, есть ли движущиеся блоки
        foreach (Block block in allBlocks)
        {
            Rigidbody2D rb = block.GetComponent<Rigidbody2D>();
            if (rb != null && rb.velocity.sqrMagnitude > 0.01f)
            {
                anyBlockMoving = true;
                break;
            }
        }

        if (anyBlockMoving) return;

        // Сбрасываем состояние совпадения и восстанавливаем цвета
        foreach (Block block in allBlocks)
        {
            if (block.IsMatched)
            {
                ghostVisualizer.RestoreOriginalColors(block);
                block.IsMatched = false;
                HighlightBlock(block, false); // Сбросить подсветку
            }
        }

        int matchesFound = 0;
        List<GhostVisualizer.GhostData> unmatchedGhosts = new List<GhostVisualizer.GhostData>(ghostVisualizer.GhostDataList);

        // Проверяем совпадения для каждого блока
        foreach (Block block in allBlocks)
        {
            if (block.IsMatched) continue;

            GhostVisualizer.GhostData matchedGhost = null;

            // Проверяем блок со всеми призраками
            foreach (var ghost in unmatchedGhosts)
            {
                Vector2Int[] playerCells = block.GetCurrentCells();
                if (AreCellsMatch(playerCells, ghost.occupiedCells))
                {
                    matchedGhost = ghost;
                    break;
                }
            }

            if (matchedGhost != null)
            {
                block.IsMatched = true;
                ghostVisualizer.SaveOriginalColors(block);
                HighlightBlock(block, true);
                matchesFound++;
                unmatchedGhosts.Remove(matchedGhost);

                Debug.Log($"Блок '{block.name}' совпал с призраком в позиции ({matchedGhost.gridPosition.x},{matchedGhost.gridPosition.y})");
            }
        }

        // Подсвечиваем несовпавшие призраки (опционально)
        foreach (var ghost in unmatchedGhosts)
        {
            ghost.meshRenderer.material.color = new Color(1, 0, 0, 0.5f); // Красный для несовпавших
        }

        if (matchesFound >= ghostVisualizer.GhostDataList.Count)
        {
            Debug.Log($"Уровень завершен! Совпадений: {matchesFound}/{ghostVisualizer.GhostDataList.Count}");
            LevelCompleted();
        }
        else
        {
            Debug.Log($"Частичное совпадение: {matchesFound}/{ghostVisualizer.GhostDataList.Count}");
        }
    }

    private bool AreCellsMatch(Vector2Int[] cellsA, Vector2Int[] cellsB)
    {
        if (cellsA.Length != cellsB.Length) return false;

        // Создаем хеш-сеты для сравнения
        HashSet<Vector2Int> setA = new HashSet<Vector2Int>(cellsA);
        HashSet<Vector2Int> setB = new HashSet<Vector2Int>(cellsB);

        return setA.SetEquals(setB);
    }

    private void DebugGhostAndBlockPositions()
    {
        if (ghostVisualizer == null) return;

        foreach (var ghost in ghostVisualizer.GhostDataList)
        {
            Debug.Log($"Ghost at ({ghost.gridPosition.x},{ghost.gridPosition.y}), " +
                      $"Rotation: {ghost.rotationIndex * 90}°, " +
                      $"Cells: {string.Join(", ", ghost.occupiedCells.Select(c => $"({c.x},{c.y})"))}");
        }

        var allBlocks = FindObjectsOfType<Block>();
        foreach (var block in allBlocks)
        {
            Vector2Int[] cells = block.GetCurrentCells();
            Debug.Log($"Block at ({block.GridPosition.x},{block.GridPosition.y}), " +
                      $"Rotation: {block.GetRotationIndex() * 90}°, " +
                      $"Cells: {string.Join(", ", cells.Select(c => $"({c.x},{c.y})"))}");
        }
    }

    private void HighlightBlock(Block block, bool isMatch)
    {
        foreach (Transform child in block.transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // Используем плавное изменение цвета
                sr.color = isMatch ? matchColor : Color.Lerp(sr.color, Color.white, 0.5f);
            }
        }
    }

    private void LevelCompleted()
    {
        // Логика завершения уровня
    }
}