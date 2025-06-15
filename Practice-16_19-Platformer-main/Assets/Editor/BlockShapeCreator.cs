#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class BlockShapeCreator
{
    [MenuItem("Assets/Create/Block/Shape", false, 1)]
    public static void CreateBlockShape()
    {
        // ������� �����, ���� �� ���
        string folderPath = "Assets/BlockShapes";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        // ������� ����� �����
        BlockShape shape = ScriptableObject.CreateInstance<BlockShape>();

        // ���������� ���������� ���
        string path = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/NewBlockShape.asset");

        // ������� � ��������� �����
        AssetDatabase.CreateAsset(shape, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // �������� ��������� �����
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = shape;
    }
}
#endif