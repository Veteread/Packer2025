using UnityEngine;
using UnityEngine.UI;
using System;

public class BlockSelectionUI : MonoBehaviour
{
    [System.Serializable]
    public class BlockData
    {
        public string name;
        public Sprite icon;
        public GameObject prefab;
    }

    public BlockData[] availableBlocks;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject blockButtonPrefab;

    public event Action<GameObject> OnBlockSelected;
    private int selectedIndex = 0; // По умолчанию выбран первый блок

    private void Start()
    {
        if (availableBlocks == null || availableBlocks.Length == 0)
        {
            Debug.LogError("No blocks assigned in BlockSelectionUI!");
            return;
        }

        foreach (var block in availableBlocks)
        {
            if (block.prefab == null)
            {
                Debug.LogError("Block prefab is null in BlockSelectionUI!");
                continue;
            }

            GameObject buttonObj = Instantiate(blockButtonPrefab, contentParent);
            buttonObj.GetComponent<Image>().sprite = block.icon;
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnBlockSelected?.Invoke(block.prefab);
                SetSelected(Array.IndexOf(availableBlocks, block));
            });
        }
        SetSelected(0); // Выбираем первый блок по умолчанию
    }

    public GameObject GetSelectedPrefab()
    {
        if (selectedIndex >= 0 && selectedIndex < availableBlocks.Length)
            return availableBlocks[selectedIndex].prefab;

        Debug.LogError("Invalid selected index!");
        return null;
    }

    private void SetSelected(int index)
    {
        selectedIndex = index;
        // Визуальное выделение выбранной кнопки
        for (int i = 0; i < contentParent.childCount; i++)
        {
            var btn = contentParent.GetChild(i).GetComponent<Button>();
            btn.image.color = (i == index) ? Color.yellow : Color.white;
        }
    }
}