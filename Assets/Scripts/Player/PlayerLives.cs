using UnityEngine;

public class PlayerLives : MonoBehaviour
{
    private int _maxLives = 4, _minLives = -1;
    private int _startingLives = 3;
    private int _currentPlayerLives;

    #region Properties
    public int CurrentPlayerLives
    {
        get { return _currentPlayerLives; }
        private set { _currentPlayerLives = value; }
    }
    #endregion


    void OnEnable()
    {
        EnemyCollision.OnEnemyCollision += LoseLife;
    }

    void Start()
    {
        CurrentPlayerLives = _startingLives;
    }

    // Remove a life
    void LoseLife()
    {
        if (CurrentPlayerLives > _minLives)
        {
            CurrentPlayerLives--;
            Debug.Log("Current Lives: " + CurrentPlayerLives);
        }

        if (CurrentPlayerLives < 0)
        {
            Debug.Log("Death");
            Debug.Log("Exiting");
        }
    }

    // Can only can 1 life after collecting 10000 total score
    public void GainLife()
    {
        if (CurrentPlayerLives < _maxLives)
        {
            CurrentPlayerLives++;
        }
        else
            return;
    }

    void OnDisable()
    {
        EnemyCollision.OnEnemyCollision -= LoseLife; 
    }
}
