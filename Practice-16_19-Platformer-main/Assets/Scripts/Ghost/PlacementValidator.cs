using System.Linq;
using UnityEngine;

public class PlacementValidator : MonoBehaviour
{
    [SerializeField] private Grid basketGrid;
    [SerializeField] private int gridWidth = 6;
    [SerializeField] private int gridHeight = 6;
    
    private bool[,] targetGrid;
    private bool[,] playerGrid;
    
    public void InitializeValidation(LevelData levelData)
    {
        targetGrid = new bool[gridWidth, gridHeight];
        playerGrid = new bool[gridWidth, gridHeight];

        // Заполняем целевую сетку
        foreach (var target in levelData.targetBlocks)
        {
            // Получаем клетки непосредственно из формы
            Vector2Int[] cells = target.shape.GetRotatedCells(target.rotationIndex);
            int minX = cells.Min(c => c.x);
            int minY = cells.Min(c => c.y);
            foreach (Vector2Int cell in cells)
            {
                int x = target.gridPosition.x + cell.x;
                int y = target.gridPosition.y + cell.y;

                if (IsInGrid(x, y))
                {
                    targetGrid[x, y] = true;
                }
            }
        }
    }
    
    public float CheckAccuracy()
    {
        int correctCells = 0;
        int totalTargetCells = 0;
        
        // Считаем совпадения
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (targetGrid[x, y])
                {
                    totalTargetCells++;
                    if (playerGrid[x, y])
                    {
                        correctCells++;
                    }
                }
            }
        }
        
        return totalTargetCells > 0 ? (float)correctCells / totalTargetCells : 0f;
    }

    public void UpdatePlayerGrid(Block block)
    {
        // Очищаем предыдущее состояние
        playerGrid = new bool[gridWidth, gridHeight];

        // Обновляем новое состояние
        Vector2Int[] cells = block.GetCurrentCells();
        foreach (Vector2Int cell in cells)
        {
            if (IsInGrid(cell.x, cell.y))
            {
                playerGrid[cell.x, cell.y] = true;
            }
        }
    }

    private bool IsInGrid(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }
}
