using UnityEngine;

public class PlayerLives : MonoBehaviour
{
    private int _maxLives = 4, _minLives = 1;
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

    // Updates current lives for both decrease/increase
    void LoseLife()
    {
        if (CurrentPlayerLives > _minLives)
        {
            CurrentPlayerLives--;
        }
        else if (CurrentPlayerLives == 1)
        {
            CurrentPlayerLives--;
            // Death
        }
    }

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
