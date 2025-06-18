using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GhostVisualizer : MonoBehaviour
{
    [SerializeField] private Grid basketGrid;
    [SerializeField] private Transform ghostsContainer;
    [SerializeField] private Material ghostMaterial;
    [SerializeField] private List<GhostData> ghostDataList = new List<GhostData>();

    private Dictionary<Block, Color[]> originalBlockColors = new Dictionary<Block, Color[]>();
    public List<GhostData> GhostDataList => ghostDataList;
    public class GhostData
    {
        public Vector2Int gridPosition;
        public BlockShape shape;
        public int rotationIndex;
        public GameObject ghostObject;
        public MeshRenderer meshRenderer;
        public Vector2Int[] occupiedCells;
    }

    public async UniTask VisualizeGhostsAsync(LevelData levelData)
    {
        await ClearGhostsAsync();

        var tasks = new List<UniTask>();
        foreach (var target in levelData.targetBlocks)
        {
            tasks.Add(CreateGhostAsync(target));
        }

        await UniTask.WhenAll(tasks);
    }

    private async UniTask CreateGhostAsync(LevelData.TargetBlock target)
    {
        // Асинхронная загрузка префаба призрака
        var loadOp = Addressables.LoadAssetAsync<GameObject>("GhostPrefab");
        await loadOp.ToUniTask();

        if (loadOp.Status == AsyncOperationStatus.Succeeded)
        {
            var ghostObj = Instantiate(loadOp.Result, ghostsContainer);
            // ... остальная логика создания призрака ...

            await UniTask.Yield(); // Для распределения нагрузки
        }
    }

    private async UniTask ClearGhostsAsync()
    {
        foreach (Transform child in ghostsContainer)
        {
            Destroy(child.gameObject);
        }
        ghostDataList.Clear();

        await UniTask.Yield();
    }

    public void VisualizeGhosts(LevelData levelData)
    {
        ClearGhosts();
        foreach (var target in levelData.targetBlocks)
        {
            CreateGhost(target);
        }
    }

    private void CreateGhost(LevelData.TargetBlock target)
    {
        if (target == null || target.shape == null || basketGrid == null)
        {
            Debug.LogError("Invalid ghost creation parameters");
            return;
        }

        // Создаем объект призрака
        GameObject ghostObj = new GameObject($"Ghost_{target.shape.name}");
        ghostObj.transform.SetParent(ghostsContainer);

        // Получаем повернутые клетки
        Vector2Int[] rotatedCells = target.shape.GetRotatedCells(target.rotationIndex);

        if (rotatedCells.Length == 0)
        {
            Destroy(ghostObj);
            return;
        }

        // Рассчитываем минимальные координаты для нормализации
        int minX = rotatedCells.Min(c => c.x);
        int minY = rotatedCells.Min(c => c.y);

        // Нормализуем координаты (делаем их положительными)
        Vector2Int[] normalizedCells = new Vector2Int[rotatedCells.Length];
        for (int i = 0; i < rotatedCells.Length; i++)
        {
            normalizedCells[i] = new Vector2Int(
                rotatedCells[i].x - minX,
                rotatedCells[i].y - minY
            );
        }

        // Позиция в сетке с учетом нормализации
        Vector3 worldPos = basketGrid.CellToWorld(new Vector3Int(
            target.gridPosition.x,
            target.gridPosition.y,
            0
        ));

        ghostObj.transform.position = worldPos;

        // Создаем меш для визуализации
        MeshFilter meshFilter = ghostObj.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = ghostObj.AddComponent<MeshRenderer>();
        meshRenderer.material = ghostMaterial;

        // Создаем меш из нормализованных клеток
        CreateGhostMesh(meshFilter, normalizedCells);

        // Рассчитываем занятые клетки в глобальных координатах сетки
        Vector2Int[] occupiedCells = new Vector2Int[rotatedCells.Length];
        for (int i = 0; i < rotatedCells.Length; i++)
        {
            occupiedCells[i] = new Vector2Int(
                target.gridPosition.x + normalizedCells[i].x,
                target.gridPosition.y + normalizedCells[i].y
            );
        }

        // Сохраняем данные призрака
        ghostDataList.Add(new GhostData
        {
            gridPosition = target.gridPosition,
            shape = target.shape,
            rotationIndex = target.rotationIndex,
            ghostObject = ghostObj,
            occupiedCells = occupiedCells,
            meshRenderer = meshRenderer
        });

        // Отладочный вывод
        Debug.Log($"Created ghost at {target.gridPosition} with rotation {target.rotationIndex}");
        Debug.Log($"Occupied cells: {string.Join(", ", occupiedCells)}");
    }

    //private void CreateGhostMeshFromCells(MeshFilter meshFilter, Vector2Int[] cells, Vector2 center)
    //{
    //    if (cells.Length == 0) return;

    //    Mesh mesh = new Mesh();
    //    List<Vector3> vertices = new List<Vector3>();
    //    List<int> triangles = new List<int>();
    //    List<Vector2> uvs = new List<Vector2>();

    //    float cellSize = basketGrid.cellSize.x;
    //    int triIndex = 0;

    //    foreach (Vector2Int cell in cells)
    //    {
    //        // Смещаем клетки относительно центра
    //        Vector2 centeredCell = new Vector2(cell.x - center.x, cell.y - center.y);

    //        // Рассчитываем вершины для одной клетки
    //        Vector3 bottomLeft = new Vector3(centeredCell.x * cellSize, centeredCell.y * cellSize, 0);
    //        Vector3 bottomRight = bottomLeft + new Vector3(cellSize, 0, 0);
    //        Vector3 topLeft = bottomLeft + new Vector3(0, cellSize, 0);
    //        Vector3 topRight = bottomLeft + new Vector3(cellSize, cellSize, 0);

    //        // Добавляем вершины
    //        vertices.Add(bottomLeft);
    //        vertices.Add(bottomRight);
    //        vertices.Add(topLeft);
    //        vertices.Add(topRight);

    //        // Добавляем треугольники
    //        triangles.Add(triIndex);
    //        triangles.Add(triIndex + 1);
    //        triangles.Add(triIndex + 2);

    //        triangles.Add(triIndex + 1);
    //        triangles.Add(triIndex + 3);
    //        triangles.Add(triIndex + 2);

    //        // UV координаты
    //        uvs.Add(new Vector2(0, 0));
    //        uvs.Add(new Vector2(1, 0));
    //        uvs.Add(new Vector2(0, 1));
    //        uvs.Add(new Vector2(1, 1));

    //        triIndex += 4;
    //    }

    //    mesh.vertices = vertices.ToArray();
    //    mesh.triangles = triangles.ToArray();
    //    mesh.uv = uvs.ToArray();
    //    mesh.RecalculateNormals();
    //    mesh.RecalculateBounds();

    //    meshFilter.mesh = mesh;
    //}

    private void CreateGhostMesh(MeshFilter meshFilter, Vector2Int[] cells)
    {
        if (cells.Length == 0) return;

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float cellSize = basketGrid.cellSize.x;
        int triIndex = 0;

        foreach (Vector2Int cell in cells)
        {
            Vector3 bottomLeft = new Vector3(cell.x * cellSize, cell.y * cellSize, 0);
            Vector3 bottomRight = bottomLeft + new Vector3(cellSize, 0, 0);
            Vector3 topLeft = bottomLeft + new Vector3(0, cellSize, 0);
            Vector3 topRight = bottomLeft + new Vector3(cellSize, cellSize, 0);

            vertices.Add(bottomLeft);
            vertices.Add(bottomRight);
            vertices.Add(topLeft);
            vertices.Add(topRight);

            triangles.Add(triIndex);
            triangles.Add(triIndex + 2);
            triangles.Add(triIndex + 1);

            triangles.Add(triIndex + 1);
            triangles.Add(triIndex + 2);
            triangles.Add(triIndex + 3);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            triIndex += 4;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    public void HighlightGhost(GhostData ghost, Color color)
    {
        if (ghost != null && ghost.meshRenderer != null)
        {
            ghost.meshRenderer.material.color = color;
        }
    }

    //private Vector2 CalculateShapeCenter(Vector2Int[] cells)
    //{
    //    if (cells.Length == 0) return Vector2.zero;

    //    float sumX = 0;
    //    float sumY = 0;

    //    foreach (Vector2Int cell in cells)
    //    {
    //        sumX += cell.x;
    //        sumY += cell.y;
    //    }

    //    return new Vector2(sumX / cells.Length, sumY / cells.Length);
    //}

    //private void ClearTexture(Texture2D texture, Color color)
    //{
    //    Color[] pixels = new Color[texture.width * texture.height];
    //    for (int i = 0; i < pixels.Length; i++)
    //    {
    //        pixels[i] = color;
    //    }
    //    texture.SetPixels(pixels);
    //}

    //private void DrawCell(Texture2D texture, int x, int y, Color color)
    //{
    //    int size = 30; // Размер клетки (с зазором)

    //    for (int i = x + 1; i < x + size; i++)
    //    {
    //        for (int j = y + 1; j < y + size; j++)
    //        {
    //            if (i >= 0 && i < texture.width && j >= 0 && j < texture.height)
    //            {
    //                texture.SetPixel(i, j, color);
    //            }
    //        }
    //    }
    //}

    public bool CheckBlockMatch(Block playerBlock)
    {
        if (playerBlock == null) return false;

        Vector2Int[] playerCells = playerBlock.GetCurrentCells();
        int playerRotation = playerBlock.GetRotationIndex();

        foreach (GhostData ghost in ghostDataList)
        {
            // Проверяем форму и поворот
            if (playerBlock.Shape != ghost.shape) continue;
            if (playerRotation != ghost.rotationIndex) continue;

            // Проверяем позиционирование
            Vector2Int[] ghostCells = ghost.occupiedCells;

            if (AreCellsMatch(playerCells, ghostCells))
            {
                return true;
            }
        }
        return false;
    }

    // Проверяет, совпадают ли два набора клеток
    private bool AreCellsMatch(Vector2Int[] cellsA, Vector2Int[] cellsB)
    {
        if (cellsA.Length != cellsB.Length) return false;

        // Создаем копии массивов и сортируем их для сравнения
        Vector2Int[] sortedA = cellsA.OrderBy(c => c.x).ThenBy(c => c.y).ToArray();
        Vector2Int[] sortedB = cellsB.OrderBy(c => c.x).ThenBy(c => c.y).ToArray();

        for (int i = 0; i < sortedA.Length; i++)
        {
            if (sortedA[i] != sortedB[i]) return false;
        }
        return true;
    }

   

    //private Vector2 CalculateCenteringOffset(BlockShape shape)
    //{
    //    // Находим границы формы
    //    int minX = shape.Cells.Min(c => c.x);
    //    int maxX = shape.Cells.Max(c => c.x);
    //    int minY = shape.Cells.Min(c => c.y);
    //    int maxY = shape.Cells.Max(c => c.y);

    //    // Рассчитываем центр
    //    float centerX = (minX + maxX) / 2f;
    //    float centerY = (minY + maxY) / 2f;

    //    // Смещение для центрирования относительно (0,0)
    //    return new Vector2(-centerX, -centerY);
    //}

    //private Vector2 CalculateRotationOffset(BlockShape shape, int rotationIndex)
    //{
    //    if (shape.Cells.Count == 0) return Vector2.zero;

    //    // Находим минимальные координаты
    //    int minX = shape.Cells.Min(c => c.x);
    //    int minY = shape.Cells.Min(c => c.y);

    //    Vector2 offset = Vector2.zero;

    //    switch (rotationIndex)
    //    {
    //        case 1: // 90°
    //            offset = new Vector2(minY, -minX);
    //            break;
    //        case 2: // 180°
    //            offset = new Vector2(-minX, -minY);
    //            break;
    //        case 3: // 270°
    //            offset = new Vector2(-minY, minX);
    //            break;
    //    }

    //    return offset * basketGrid.cellSize.x;
    //}
    
    public void SaveOriginalColors(Block block)
    {
        if (originalBlockColors.ContainsKey(block)) return;

        List<Color> colors = new List<Color>();
        foreach (Transform child in block.transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null) colors.Add(sr.color);
        }
        originalBlockColors[block] = colors.ToArray();
    }

    public void RestoreOriginalColors(Block block)
    {
        if (!originalBlockColors.ContainsKey(block)) return;

        Color[] colors = originalBlockColors[block];
        int index = 0;
        foreach (Transform child in block.transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null && index < colors.Length)
            {
                sr.color = colors[index++];
            }
        }
    }

    private Vector2Int GetGridPosition(Vector3 worldPos)
    {
        Vector3Int cellPos = basketGrid.WorldToCell(worldPos);
        return new Vector2Int(cellPos.x, cellPos.y);
    }

    public void ClearGhosts()
    {
        foreach (Transform child in ghostsContainer)
        {
            Destroy(child.gameObject);
        }
        ghostDataList.Clear();
        originalBlockColors.Clear();
    }

     
    public int GhostCount => ghostDataList.Count;
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        foreach (var ghost in ghostDataList)
        {
            Gizmos.color = Color.cyan;
            foreach (var cell in ghost.occupiedCells)
            {
                Vector3 worldPos = basketGrid.CellToWorld(new Vector3Int(cell.x, cell.y, 0));
                Gizmos.DrawWireCube(worldPos, basketGrid.cellSize * 0.9f);

#if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    worldPos + Vector3.up * 0.2f,
                    $"{cell.x},{cell.y}",
                    new GUIStyle { normal = new GUIStyleState { textColor = Color.white }, fontSize = 12 }
                );
#endif
            }
        }

        var blocks = FindObjectsOfType<Block>();
        foreach (var block in blocks)
        {
            Gizmos.color = block.IsMatched ? Color.green : Color.yellow;
            var cells = block.GetCurrentCells();
            foreach (var cell in cells)
            {
                Vector3 worldPos = basketGrid.CellToWorld(new Vector3Int(cell.x, cell.y, 0));
               // Gizmos.DrawWireSphere(worldPos, 0.2f);

#if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    worldPos + Vector3.up * 0.3f,
                    $"{cell.x},{cell.y}",
                    new GUIStyle { normal = new GUIStyleState { textColor = Color.yellow }, fontSize = 12 }
                );
#endif
            }
        }
    }
}