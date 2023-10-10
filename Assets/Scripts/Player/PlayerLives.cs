using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLives : MonoBehaviour
{
    private int _maxLives = 4, _minLives = 1;
    private int _startingLives = 3;
    private int _currentPlayerLives;
    public int CurrentPlayerLives
    {
        get { return _currentPlayerLives; }
        private set { _currentPlayerLives = value; }
    }

    

    void OnEnable()
    {
        EnemyCollision.OnEnemyCollision += LoseLife;
    }

    void Start()
    {
        CurrentPlayerLives = _startingLives;
        Debug.Log("Total Starting Lives: " + CurrentPlayerLives);
    }

    // Updates current lives for both decrease/increase
    void LoseLife()
    {
        if (CurrentPlayerLives > _minLives)
        {
            CurrentPlayerLives--;
            Debug.Log("I got hit! New Life Count: " + CurrentPlayerLives);
        }
        else if (CurrentPlayerLives == 1)
        {
            CurrentPlayerLives--;
            Debug.Log("I should be Dead: " + CurrentPlayerLives);
            // Death
        }
    }

    public void GainLife()
    {
        if (CurrentPlayerLives < _maxLives)
        {
            CurrentPlayerLives++;
            Debug.Log("Gained a Life! New lives count: " + CurrentPlayerLives);
        }
        else
            return;
    }

    void OnDisable()
    {
        EnemyCollision.OnEnemyCollision -= LoseLife; 
    }
}
