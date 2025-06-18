using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class LevelSelectionMenu : MonoBehaviour
{
    //[SerializeField] private Transform levelsContainer;
    //[SerializeField] private GameObject levelButtonPrefab;
    //[SerializeField] private Sprite lockedLevelSprite;

    //private async void Start()
    //{
    //    await LevelManager.Instance.InitializeAsync();
    //    GenerateLevelButtons();
    //}

    //private void GenerateLevelButtons()
    //{
    //    foreach (Transform child in levelsContainer)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    IReadOnlyList<LevelData> allLevels = LevelManager.Instance.GetAllLevels();

    //    for (int i = 0; i < allLevels.Count; i++)
    //    {
    //        LevelData level = allLevels[i];
    //        GameObject buttonObj = Instantiate(levelButtonPrefab, levelsContainer);
    //        LevelButton levelButton = buttonObj.GetComponent<LevelButton>();

    //        bool isUnlocked = LevelManager.Instance.IsLevelUnlocked(level.levelID);
    //        levelButton.Setup(level, isUnlocked);

    //        Button btn = buttonObj.GetComponent<Button>();
    //        if (isUnlocked)
    //        {
    //            btn.onClick.AddListener(() => LoadLevel(level.levelID));
    //        }
    //        else
    //        {
    //            btn.interactable = false;
    //        }
    //    }
    //}

    //private void LoadLevel(int levelID)
    //{
    //    LevelManager.Instance.LoadLevelAsync(levelID).Forget();
    //}
}