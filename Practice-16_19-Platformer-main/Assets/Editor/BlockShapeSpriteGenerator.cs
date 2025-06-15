#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

public class BlockShapeSpriteCreator : EditorWindow
{
    private BlockShape shape;
    private Color cellColor = Color.green;
    private int textureSize = 128;

    [MenuItem("Tools/Shape Sprite Creator")]
    public static void ShowWindow()
    {
        GetWindow<BlockShapeSpriteCreator>("Block Shape Sprite Creator");
    }

    private void OnGUI()
    {
        shape = (BlockShape)EditorGUILayout.ObjectField("Block Shape", shape, typeof(BlockShape), false);
        cellColor = EditorGUILayout.ColorField("Cell Color", cellColor);
        textureSize = EditorGUILayout.IntField("Texture Size", textureSize);

        if (GUILayout.Button("Create Sprite") && shape != null)
        {
            CreateSprite();
        }
    }

    private void CreateSprite()
    {
        int minX = shape.Cells.Min(c => c.x);
        int maxX = shape.Cells.Max(c => c.x);
        int minY = shape.Cells.Min(c => c.y);
        int maxY = shape.Cells.Max(c => c.y);

        int widthCells = maxX - minX + 1;
        int heightCells = maxY - minY + 1;

        int pixelsPerCell = textureSize / Mathf.Max(widthCells, heightCells);

        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
        ClearTexture(texture, new Color(0, 0, 0, 0));

        foreach (Vector2Int cell in shape.Cells)
        {
            int xPos = (cell.x - minX) * pixelsPerCell;
            int yPos = (cell.y - minY) * pixelsPerCell;
            DrawCell(texture, xPos, yPos, pixelsPerCell, cellColor);
        }

        texture.Apply();

        string path = EditorUtility.SaveFilePanelInProject(
            "Save Shape Sprite",
            $"{shape.name}_sprite",
            "png",
            "Save the generated sprite");

        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
            AssetDatabase.Refresh();

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = pixelsPerCell;
                importer.mipmapEnabled = false;
                importer.spritePivot = new Vector2(0, 0); // Bottom-left pivot
                importer.SaveAndReimport();

                // Назначаем спрайт в BlockShape
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                SerializedObject so = new SerializedObject(shape);
                so.FindProperty("shapeSprite").objectReferenceValue = sprite;
                so.ApplyModifiedProperties();
            }
        }
    }

    private void ClearTexture(Texture2D texture, Color color)
    {
        Color[] pixels = new Color[texture.width * texture.height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        texture.SetPixels(pixels);
    }

    private void DrawCell(Texture2D texture, int x, int y, int cellSize, Color color)
    {
        int margin = Mathf.Max(1, cellSize / 16);
        int size = cellSize - 2 * margin;

        for (int i = x + margin; i < x + cellSize - margin; i++)
        {
            for (int j = y + margin; j < y + cellSize - margin; j++)
            {
                if (i < texture.width && j < texture.height)
                {
                    texture.SetPixel(i, j, color);
                }
            }
        }
    }
}
#endif