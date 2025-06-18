using UnityEngine;
using UnityEngine.SceneManagement;

public class PackingSceneManager : MonoBehaviour
{
    public void FinishPacking(int collectedResources)
    {
        // ������� �������� ����� �������
        PlanetSceneManager planetManager = FindObjectOfType<PlanetSceneManager>();

        if (planetManager != null)
        {
            planetManager.ReturnFromPacking(collectedResources);
        }
        else
        {
            // ���� ��������� ��� (��������, ��� ������������ ��������� �����)
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.AddResource("materials", collectedResources);
            }
            SceneManager.LoadScene("PlanetOrbitScene");
        }
    }
    private int CalculateResources()
    {
        // ���� ������ ��������
        return Random.Range(10, 50);
    }
}