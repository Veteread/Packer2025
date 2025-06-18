using UnityEngine;
using UnityEngine.SceneManagement;

public class PackingSceneManager : MonoBehaviour
{
    public void FinishPacking(int collectedResources)
    {
        // Находим менеджер сцены планеты
        PlanetSceneManager planetManager = FindObjectOfType<PlanetSceneManager>();

        if (planetManager != null)
        {
            planetManager.ReturnFromPacking(collectedResources);
        }
        else
        {
            // Если менеджера нет (например, при тестировании отдельной сцены)
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.AddResource("materials", collectedResources);
            }
            SceneManager.LoadScene("PlanetOrbitScene");
        }
    }
    private int CalculateResources()
    {
        // Ваша логика подсчёта
        return Random.Range(10, 50);
    }
}