using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>   
{
    private int _currentScene;

    private bool isGameOver;

    [SerializeField] private PlayerLives _playerLives;
    [SerializeField] private GameObject _menu;

    #region Properties
    public int CurrentScene {  get { return _currentScene; } private set { _currentScene = value; } }   
    public bool IsGameOver {  get { return isGameOver; } private set { isGameOver = value; } }
    public GameObject Menu { get { return _menu; } }
    #endregion



    void OnEnable()
    {
        EnemyCollision.OnEnemyCollision += GameOver;
    }

    void OnDisable()
    {
        EnemyCollision.OnEnemyCollision -= GameOver;
    }

    void Start()
    {
        isGameOver = false;
        _currentScene = SceneManager.GetActiveScene().buildIndex;
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        StartCoroutine(CheckPlayerLivesRoutine());  
    }

    public void OpenMenu()
    {
        if (Menu.activeInHierarchy)     // If the Menu gameobject is active - Turn off and resume game speed
        {
            Menu.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            Menu.SetActive(true);       // Else, turn on gameobject and pause the game speed
            Time.timeScale = 0;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(_currentScene);
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }

    // Check at the end of the frame to allow for the lives to update
    IEnumerator CheckPlayerLivesRoutine()
    {
        yield return new WaitForEndOfFrame();

        if (_playerLives.CurrentPlayerLives == 0)
        {
            IsGameOver = true;
            Time.timeScale = 0f;
        }
    }
}
