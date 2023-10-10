using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLives : MonoBehaviour
{
    private int _maxLives = 3, _minLives = 1;
    private int _currentPlayerLives;
    public int CurrentPlayerLives
    {
        get { return _currentPlayerLives; }
        private set { _currentPlayerLives = value; }
    }

    



    void OnEnable()
    {
        EnemyCollision.OnEnemyCollision += UpdateLives;
    }

    void Start()
    {
        CurrentPlayerLives = _maxLives;
        Debug.Log("Total Starting Lives: " + CurrentPlayerLives);
    }

    // Updates current lives for both decrease/increase
    void UpdateLives()
    {
        if (CurrentPlayerLives > _minLives)
        {
            CurrentPlayerLives--;
            Debug.Log("New Life Count: " + CurrentPlayerLives);
        }
        else if (CurrentPlayerLives == 1)
        {
            CurrentPlayerLives--;
            Debug.Log("Should be Dead: " + CurrentPlayerLives);
            // Death
        }
    }

    void OnDisable()
    {
        EnemyCollision.OnEnemyCollision -= UpdateLives; 
    }
}
