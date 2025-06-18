using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PackingManager : MonoBehaviour
{
    public void ReturnToOrbit()
    {
        // Сохраняем собранные ресурсы
      // PlayerInventory.Instance.SaveResources();
        SceneManager.LoadScene("PlanetOrbitScene");
    }

    public void OnResourcePacked(string resourceId, int amount)
    {
        //PlayerInventory.Instance.AddResource(resourceId, amount);
    }
}
