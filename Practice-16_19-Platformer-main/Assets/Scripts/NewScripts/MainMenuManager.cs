using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject multiplayerPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Network References")]
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private MyNetworkManager networkManager;

    private void Start()
    {
        ShowMainPanel();
        ipInputField.text = "localhost"; // Default IP
    }

    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        multiplayerPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void ShowMultiplayerPanel()
    {
        mainPanel.SetActive(false);
        multiplayerPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void ShowSettingsPanel()
    {
        mainPanel.SetActive(false);
        multiplayerPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void StartSinglePlayer()
    {
        SceneManager.LoadScene("PlanetScene");
    }

    public void StartHost()
    {
        networkManager.StartHost();
    }

    public void JoinServer()
    {
        networkManager.networkAddress = ipInputField.text;
        networkManager.StartClient();
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}