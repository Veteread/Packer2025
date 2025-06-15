#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlockShape))]
public class BlockShapeEditor : Editor
{
    private const float CELL_SIZE = 0.16f;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BlockShape shape = (BlockShape)target;

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);

        // Показываем повороты
        for (int rotation = 0; rotation < 4; rotation++)
        {
            EditorGUILayout.LabelField($"Rotation: {rotation * 90}°");

            Vector2Int[] cells = shape.GetRotatedCells(rotation);
            foreach (Vector2Int cell in cells)
            {
                EditorGUILayout.LabelField($"  ({cell.x}, {cell.y})");
            }
        }

        // Визуальная превьюшка
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Visual Preview:");
        Rect previewRect = GUILayoutUtility.GetRect(200, 200);
        DrawShapePreview(previewRect, shape);
    }

    private void DrawShapePreview(Rect area, BlockShape shape)
    {
        // Сетка
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                Rect cellRect = new Rect(
                    area.x + x * CELL_SIZE * 20,
                    area.y + y * CELL_SIZE * 20,
                    CELL_SIZE * 20,
                    CELL_SIZE * 20
                );
                EditorGUI.DrawRect(cellRect, new Color(0.2f, 0.2f, 0.2f, 0.1f));
            }
        }

        // Клетки формы
        foreach (Vector2Int cell in shape.Cells)
        {
            Rect cellRect = new Rect(
                area.x + (cell.x + 1) * CELL_SIZE * 20,
                area.y + (cell.y + 1) * CELL_SIZE * 20,
                CELL_SIZE * 20,
                CELL_SIZE * 20
            );
            EditorGUI.DrawRect(cellRect, Color.green);
        }
    }
}
#endif