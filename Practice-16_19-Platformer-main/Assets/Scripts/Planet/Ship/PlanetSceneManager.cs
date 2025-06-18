using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetSceneManager : MonoBehaviour
{
    [SerializeField] private Ship2D playerShip;
    [SerializeField] private Transform planet;
    [SerializeField] private string packingSceneName = "ResourcePackingScene";

    public void StartResourcePacking()
    {
        // ��������� ������� ����� ���������
        PlayerPrefs.SetFloat("ShipPosX", playerShip.transform.position.x);
        PlayerPrefs.SetFloat("ShipPosY", playerShip.transform.position.y);

        SceneManager.LoadScene(packingSceneName);
    }

    public void ReturnFromPacking(int collectedResources)
    {
        // ��������������� �������
        float x = PlayerPrefs.GetFloat("ShipPosX", planet.position.x + 5f);
        float y = PlayerPrefs.GetFloat("ShipPosY", planet.position.y);
        playerShip.transform.position = new Vector2(x, y);
    }
    public void FinishPacking()
    {
        // ������ ��������� ��������
        int resources = CalculateResources();

        // ���������� ��������
        PlayerInventory.Instance.AddResource("materials", resources);

        // ������� �� ������
        SceneManager.LoadScene("PlanetOrbitScene");
    }

    private int CalculateResources()
    {
        // ���� ������ ��������
        return Random.Range(10, 50);
    }
}