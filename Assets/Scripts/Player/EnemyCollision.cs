using System;
using UnityEngine;

// Responsible for holding a reference to a collision event
public class EnemyCollision : MonoBehaviour
{
    private string _stateName = "Frightened";

    public static Action OnEnemyCollision;

    
    AudioSource _audioSource;
    Animator _animator;

    [SerializeField] private AudioClip _deathClip;
    [SerializeField] private AudioClip _eatClip;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private PlayerLives _playerLives;


    void Start()
    {
        _audioSource = GetComponent<AudioSource>(); 
    }

    void OnTriggerEnter(Collider other)         
    {
        if (other.CompareTag("Enemy"))
        {
            _animator = other.gameObject.GetComponent<Animator>();  

            bool isStatePlaying = IsAnimatorStatePlaying(_animator, _stateName, 0);

            if(isStatePlaying)
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

    bool IsAnimatorStatePlaying(Animator animator, string stateName, int layerIndex)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        return stateInfo.IsName(stateName);
    }
}
