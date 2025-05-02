using UnityEngine;
using UnityEngine.UI;
using System;

public class BlockSelectionUI : MonoBehaviour
{
    private int selectedIndex = -1;

    [System.Serializable]
    public class BlockData
    {
        public string name;
        public Sprite icon;
        public GameObject prefab;
    }

    // Добавляем событие для выбора блока
    public event Action<GameObject> OnBlockSelected;

    public BlockData[] availableBlocks;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject blockButtonPrefab;

    private void Start()
    {
        foreach (var block in availableBlocks)
        {
            GameObject buttonObj = Instantiate(blockButtonPrefab, contentParent);
            buttonObj.GetComponent<Image>().sprite = block.icon;
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnBlockSelected?.Invoke(block.prefab);
            });
        }
    }

    public void SetSelected(int index)
    {
        selectedIndex = index;
        for (int i = 0; i < contentParent.childCount; i++)
        {
            var btn = contentParent.GetChild(i).GetComponent<Button>();
            btn.image.color = i == index ? Color.yellow : Color.white;
        }
    }

    public void ResetSelection()
    {
        // Дополнительная логика сброса выделения, если нужно
    }
}