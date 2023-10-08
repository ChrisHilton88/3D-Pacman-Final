using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider), typeof(Animator))]
public class BlinkyBehaviour : MonoBehaviour
{
    private int _maxSpeed = 10;

    NavMeshAgent _agent;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _homePos;

    private const float _speedIncrement = 0.02f;       // (10% - 5% / 240) = 5/240. Or, (maximum allowed speed - starting speed / total pellets)


    void OnEnable()
    {
        ItemCollection.onItemCollected += PelletCollected;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();  
    }

    void FixedUpdate()
    {
        _agent.destination = _player.position;
    }

    // Increments agents speed everytime a pellet is collected
    void IncrementAgentSpeed()
    {
        if (_agent.speed < _maxSpeed)
            _agent.speed += _speedIncrement;
        else
        {
            _agent.speed = _maxSpeed;
            return;
        }
    }

    // Event
    void PelletCollected(int value)
    {
        IncrementAgentSpeed();
    }

    // TODO - Player position doesn't need to be updated 60 times per second, limit this time.
    // Start Coroutine when game starts
    //IEnumerator UpdatePlayerPos()
    //{

    //}

    void OnDisable()
    {
        ItemCollection.onItemCollected -= PelletCollected; 
    }
}
