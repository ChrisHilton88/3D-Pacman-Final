using System.Collections;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>   
{
    private bool isGameOver;

    [SerializeField] private PlayerLives _playerLives;

    #region Properties
    public bool IsGameOver {  get { return isGameOver; } set { isGameOver = value; } }
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
    }

    public void GameOver()
    {
        StartCoroutine(CheckPlayerLivesRoutine());  
        Debug.Log("testing GameOver");
    }

    // Check at the end of the frame to allow for the lives to update
    IEnumerator CheckPlayerLivesRoutine()
    {
        yield return new WaitForEndOfFrame();

        if (_playerLives.CurrentPlayerLives == 0)
        {
            IsGameOver = true;
            Time.timeScale = 0f;
            Debug.Log("Game Over = True");
        }
    }
}
