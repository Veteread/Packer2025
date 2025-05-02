using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void Scene(int Index)
    {
        SceneManager.LoadScene(Index);
    }   

    public void Restart()
    {
    	SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
