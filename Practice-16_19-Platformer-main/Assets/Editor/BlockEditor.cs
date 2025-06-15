#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Block block = (Block)target;

        if (GUILayout.Button("Update Visual from Shape"))
        {
            UpdateBlockVisual(block);
        }
    }

    private void UpdateBlockVisual(Block block)
    {
        // Удаляем старые дочерние объекты
        while (block.transform.childCount > 0)
        {
            DestroyImmediate(block.transform.GetChild(0).gameObject);
        }

        if (block.Shape == null) return;

        // Создаем новые визуальные элементы
        foreach (Vector2Int cell in block.Shape.Cells)
        {
            GameObject cellObj = new GameObject("BlockCell");
            cellObj.transform.SetParent(block.transform);
            cellObj.transform.localPosition = new Vector3(cell.x, cell.y, 0);

            SpriteRenderer sr = cellObj.AddComponent<SpriteRenderer>();
            sr.sprite = block.GetComponentInChildren<SpriteRenderer>()?.sprite; // Берем спрайт из компонента
            sr.sortingOrder = 0;
        }
    }
}
#endif