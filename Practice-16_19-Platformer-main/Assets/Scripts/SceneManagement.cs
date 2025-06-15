using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void Scene(int Index)
    {
        CurrencySystem.Instance?.SaveCurrency();
        SceneManager.LoadScene(Index);
    }   

    public void Restart()
    {
        CurrencySystem.Instance?.SaveCurrency();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
