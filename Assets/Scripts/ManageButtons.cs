using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManageButtons : MonoBehaviour
{
    public static ManageButtons Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        
    }
    public void StartGame() => SceneManager.LoadScene("Chapter_1");
    public void QuitApplication() => Application.Quit();
    public void RestartButton() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void GoToMainMenu() => SceneManager.LoadScene("Chapter_Start");

    private void SetUPButtons()
    {
        if (SceneManager.GetActiveScene().name == "Chapter_Start")
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
            {
                if (canvas.transform.Find("StartButton").TryGetComponent<Button>(out var startBTN))
                {
                    startBTN.onClick.RemoveAllListeners();
                    startBTN.onClick.AddListener(()=>StartGame());
                }
                if (canvas.transform.Find("QuitButton").TryGetComponent<Button>(out var quitBTN))
                {
                    quitBTN.onClick.RemoveAllListeners();
                    quitBTN.onClick.AddListener(() => StartGame());
                }

            }
        }
        GameObject panels = GameObject.Find("Panels");
        if (panels != null)
        {
            if (panels.transform.Find("LosePanel/RestartButton").TryGetComponent<Button>(out var button))
            {
                Debug.Log(button.name);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(()=>RestartButton());
            }
            if (panels.transform.Find("LosePanel/MainMenuButton").TryGetComponent<Button>(out var mainMenuButton))
            {
                mainMenuButton.onClick.RemoveAllListeners();
                mainMenuButton.onClick.AddListener(() => GoToMainMenu());
            }
            if (panels.transform.Find("WinPanel/RestartButton").TryGetComponent<Button>(out var winButton))
            {
                winButton.onClick.RemoveAllListeners();
                winButton.onClick.AddListener(() => RestartButton());
            }
            if (panels.transform.Find("WinPanel/MainMenuButton").TryGetComponent<Button>(out var winMainMenuBTN))
            {
                winMainMenuBTN.onClick.RemoveAllListeners();
                winMainMenuBTN.onClick.AddListener(()=>GoToMainMenu());
            }
        }


    }

    private void OnSceneLoad(Scene scene ,LoadSceneMode mode)
    {
        SetUPButtons();
    }
    private void OnEnable()=> SceneManager.sceneLoaded += OnSceneLoad;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoad;        

    
}
