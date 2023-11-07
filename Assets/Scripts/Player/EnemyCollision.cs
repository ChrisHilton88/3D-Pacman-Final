using System;
using UnityEngine;

// Responsible for holding a reference to a collision event with the Enemy
public class EnemyCollision : MonoBehaviour
{
    public static Action OnEnemyCollision;
    
    AudioSource _audioSource;

    [SerializeField] private AudioClip _deathClip;
    [SerializeField] private AudioClip _eatClip;


    void Start()
    {
        _audioSource = GetComponent<AudioSource>(); 
    }

    void OnTriggerEnter(Collider other)         
    {
        if (other.CompareTag("Enemy"))
        {
            if(EnemyStateManager.Instance.FrightenedStateActive)        // Check if Frightened state is active
            {
                _audioSource.clip = _eatClip;       
            }
            else
            {
                _audioSource.clip = _deathClip;
            }

            _audioSource.Play();
            OnEnemyCollision?.Invoke();
        }
    }
}
