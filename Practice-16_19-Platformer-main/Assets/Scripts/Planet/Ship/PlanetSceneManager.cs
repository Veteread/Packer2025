using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetSceneManager : MonoBehaviour
{
    [SerializeField] private Ship2D playerShip;
    [SerializeField] private Transform planet;
    [SerializeField] private string packingSceneName = "ResourcePackingScene";

    public void StartResourcePacking()
    {
        // Сохраняем позицию перед переходом
        PlayerPrefs.SetFloat("ShipPosX", playerShip.transform.position.x);
        PlayerPrefs.SetFloat("ShipPosY", playerShip.transform.position.y);

        SceneManager.LoadScene(packingSceneName);
    }

    public void ReturnFromPacking(int collectedResources)
    {
        // Восстанавливаем позицию
        float x = PlayerPrefs.GetFloat("ShipPosX", planet.position.x + 5f);
        float y = PlayerPrefs.GetFloat("ShipPosY", planet.position.y);
        playerShip.transform.position = new Vector2(x, y);
    }
    public void FinishPacking()
    {
        // Расчёт собранных ресурсов
        int resources = CalculateResources();

        // Сохранение ресурсов
        PlayerInventory.Instance.AddResource("materials", resources);

        // Возврат на орбиту
        SceneManager.LoadScene("PlanetOrbitScene");
    }

    private int CalculateResources()
    {
        // Ваша логика подсчёта
        return Random.Range(10, 50);
    }
}