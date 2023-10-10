using System;
using UnityEngine;

// Responsible for holding a reference to a collision event
public class EnemyCollision : MonoBehaviour
{
    public static Action OnEnemyCollision;

    [SerializeField] private PlayerLives _playerLives;


    void OnTriggerEnter(Collider other)         // Only looking for Enemy collisions
    {
        if (other.CompareTag("Enemy"))
        {
            OnEnemyCollision?.Invoke();
        }
    }
}
