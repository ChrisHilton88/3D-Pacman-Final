using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider), typeof(Animator))]
public class BlinkyBehaviour : MonoBehaviour
{
    private enum EnemyState
    {
        Scatter,
        Chase,
        Frightened
    }
    private EnemyState _currentState;

    private int _maxSpeed = 10;

    private const float _speedIncrement = 0.02f;       // (10% - 5% / 240) = 5/240. Or, (maximum allowed speed - starting speed / total pellets)

    private readonly Vector3 _startingPos = new Vector3(0.5f, 0, 8.5f);

    NavMeshAgent _agent;

    [SerializeField] private Transform _player;
    [SerializeField] private Transform _scatterPos;



    void OnEnable()
    {
        ItemCollection.OnItemCollected += PelletCollected;
        EnemyCollision.OnEnemyCollision += RestartPosition;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.transform.position = _startingPos;
        _currentState = EnemyState.Scatter;
    }

    void FixedUpdate()
    {
        Debug.Log("Current State: " + _currentState);   

        if(_currentState == EnemyState.Scatter)
        {
            _agent.destination = _scatterPos.position;
            // Move directly to the top right scatter position
            // Loop around waypoints in respective corner
        }
        else if(_currentState == EnemyState.Chase)
        {
            // Chase the Player
            _agent.destination = _player.position;
        }
        else if(_currentState == EnemyState.Frightened)
        {

        }
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

    #region Events
    void PelletCollected(int value)
    {
        IncrementAgentSpeed();
    }

    void RestartPosition()
    {
        _agent.Warp(_startingPos);
    }
    #endregion

    void OnDisable()
    {
        ItemCollection.OnItemCollected -= PelletCollected; 
        EnemyCollision.OnEnemyCollision -= RestartPosition; 
    }
}
