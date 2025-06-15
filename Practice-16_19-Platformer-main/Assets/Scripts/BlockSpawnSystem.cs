using UnityEngine;

public class BlockSpawnSystem : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float selectionRadius = 1f;
    [SerializeField] private GameObject highlightPrefab;

    public Transform SelectedPoint { get; private set; }

    public void ShowSpawnPoints()
    {
        ClearHighlights();
        foreach (var point in spawnPoints)
        {
            Instantiate(highlightPrefab, point.position, Quaternion.identity);
        }
    }

    public bool TrySelectPoint(Vector2 screenPos, Camera cam)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

        foreach (var point in spawnPoints)
        {
            if (Vector2.Distance(worldPos, point.position) <= selectionRadius)
            {
                SelectedPoint = point;
                ClearHighlights();
                return true;
            }
        }
        return false;
    }

    private void ClearHighlights()
    {
        var highlights = GameObject.FindGameObjectsWithTag("SpawnPointHighlight");
        foreach (var highlight in highlights)
        {
            Destroy(highlight);
        }
    }

    public Block SpawnBlock(GameObject prefab, Transform spawnPoint, PlayerBlockDropper controller)
    {
        var blockComponent = prefab.GetComponent<Block>();
        if (blockComponent == null || !CurrencySystem.Instance.CanAfford(blockComponent.GetCost()))
        {
            Debug.Log("Недостаточно средств или отсутствует компонент Block");
            return null;
        }

        var newBlock = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        blockComponent = newBlock.GetComponent<Block>();
        blockComponent.Initialize(controller, false);
        return blockComponent;
    }
}