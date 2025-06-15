using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "NewBlockShape", menuName = "Block/Shape")]
public class BlockShape : ScriptableObject
{
    [SerializeField] private List<Vector2Int> cells = new List<Vector2Int>();

    public IReadOnlyList<Vector2Int> Cells => cells;

    public Vector2Int[] GetRotatedCells(int rotationIndex)
    {
        rotationIndex = rotationIndex % 4;
        Vector2Int[] rotatedCells = new Vector2Int[cells.Count];

        for (int i = 0; i < cells.Count; i++)
        {
            Vector2Int cell = cells[i];
            switch (rotationIndex)
            {
                case 1: // 90° по часовой
                    rotatedCells[i] = new Vector2Int(-cell.y, cell.x);
                    break;
                case 2: // 180°
                    rotatedCells[i] = new Vector2Int(-cell.x, -cell.y);
                    break;
                case 3: // 270° по часовной
                    rotatedCells[i] = new Vector2Int(cell.y, -cell.x);
                    break;
                default: // 0°
                    rotatedCells[i] = cell;
                    break;
            }
        }
        return rotatedCells;
    }

    private Vector2 CalculateShapeCenter()
    {
        if (cells.Count == 0) return Vector2.zero;

        float sumX = 0;
        float sumY = 0;

        foreach (Vector2Int cell in cells)
        {
            sumX += cell.x;
            sumY += cell.y;
        }

        return new Vector2(sumX / cells.Count, sumY / cells.Count);
    }

#if UNITY_EDITOR
    [SerializeField] private Texture2D previewTexture;
#endif
}