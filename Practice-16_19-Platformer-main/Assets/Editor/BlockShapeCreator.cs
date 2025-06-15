#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class BlockShapeCreator
{
    [MenuItem("Assets/Create/Block/Shape", false, 1)]
    public static void CreateBlockShape()
    {
        // Создаем папку, если ее нет
        string folderPath = "Assets/BlockShapes";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        // Создаем новый ассет
        BlockShape shape = ScriptableObject.CreateInstance<BlockShape>();

        // Генерируем уникальное имя
        string path = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/NewBlockShape.asset");

        // Создаем и сохраняем ассет
        AssetDatabase.CreateAsset(shape, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Выделяем созданный ассет
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = shape;
    }
}
#endif