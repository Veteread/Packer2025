using UnityEngine;

public class BlockSpawnSystem : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float selectionRadius = 0.2f;
    [SerializeField] private GameObject highlightPrefab;

    public Transform SelectedPoint { get; private set; }
    public bool pointVisible = false;

    public void ShowSpawnPoints()
    {
        ClearHighlights();
        foreach (var point in spawnPoints)
        {
            Instantiate(highlightPrefab, point.position, Quaternion.identity);
        }
        pointVisible = true;
    }

    public bool TrySelectPoint(Vector2 screenPos, Camera cam)
    {
        if (!pointVisible) return false;
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
            pointVisible = false;
        }
    }

    public Block SpawnBlock(GameObject prefab, Transform spawnPoint, PlayerBlockDropper controller)
    {
        var blockComponent = prefab.GetComponent<Block>();
        if (blockComponent == null || !CurrencySystem.Instance.CanAfford(blockComponent.GetCost()))
        {
            Debug.Log("???????????? ??????? ??? ??????????? ????????? Block");
            return null;
        }

        var newBlock = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        blockComponent = newBlock.GetComponent<Block>();
        blockComponent.Initialize(controller, false);
        blockComponent.OriginalPrefab = prefab;
        newBlock.transform.SetParent(spawnPoint);
        return blockComponent;
    }
}