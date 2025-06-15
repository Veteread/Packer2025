#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class BlockCreator
{
    [MenuItem("Tools/Create Block Prefabs")]
    static void CreateBlockPrefabs()
    {
        // Создаем папку для префабов
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Blocks"))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Blocks");
        }

        // Находим все BlockShape в проекте
        var shapeGUIDs = AssetDatabase.FindAssets("t:BlockShape");
        var shapes = new List<BlockShape>();

        foreach (var guid in shapeGUIDs)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var shape = AssetDatabase.LoadAssetAtPath<BlockShape>(path);
            if (shape != null)
            {
                shapes.Add(shape);
            }
        }

        foreach (var shape in shapes)
        {
            CreateBlockPrefab(shape);
        }

        AssetDatabase.Refresh();
        Debug.Log($"Created {shapes.Count} block prefabs");
    }

    static void CreateBlockPrefab(BlockShape shape)
    {
        GameObject prefabObj = new GameObject(shape.name);
        Block block = prefabObj.AddComponent<Block>();

        // Устанавливаем форму через сериализованное поле
        SerializedObject so = new SerializedObject(block);
        so.FindProperty("shape").objectReferenceValue = shape;
        so.ApplyModifiedProperties();

        // Создаем визуальные элементы
        Vector2 offset = CalculateCenteringOffset(shape);
        float cellSize = 0.16f; // Размер клетки

        foreach (Vector2Int cell in shape.Cells)
        {
            GameObject cellObj = new GameObject("Cell");
            cellObj.transform.SetParent(prefabObj.transform);
            cellObj.transform.localPosition = new Vector3(
                (cell.x + offset.x) * cellSize,
                (cell.y + offset.y) * cellSize,
                0
            );

            SpriteRenderer sr = cellObj.AddComponent<SpriteRenderer>();
            sr.sprite = GetDefaultSprite();
            sr.sortingOrder = 0;
        }

        // Сохраняем префаб
        string path = $"Assets/Prefabs/Blocks/{shape.name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(prefabObj, path);
        Object.DestroyImmediate(prefabObj);
    }

    static Sprite GetDefaultSprite()
    {
        // Ищем первый спрайт в папке Sprites
        var spriteGUIDs = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Sprites" });
        if (spriteGUIDs.Length > 0)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(
                AssetDatabase.GUIDToAssetPath(spriteGUIDs[0]));
        }
        return null;
    }

    static Vector2 CalculateCenteringOffset(BlockShape shape)
    {
        if (shape.Cells.Count == 0) return Vector2.zero;

        int minX = shape.Cells.Min(c => c.x);
        int maxX = shape.Cells.Max(c => c.x);
        int minY = shape.Cells.Min(c => c.y);
        int maxY = shape.Cells.Max(c => c.y);

        return new Vector2(-(minX + maxX) / 2f, -(minY + maxY) / 2f);
    }
}
#endif