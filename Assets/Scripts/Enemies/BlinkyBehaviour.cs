using System.Collections;
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
    [SerializeField] private int _currentPosition;       // Scatter mode waypoint incrementer

    private const float _speedIncrement = 0.02f;       // (10% - 5% / 240) = 5/240. Or, (maximum allowed speed - starting speed / total pellets)

    private readonly Vector3 _startingPos = new Vector3(0.5f, 0, 8.5f);

    NavMeshAgent _agent;

    [SerializeField] private Transform _player;
    [SerializeField] private Transform _scatterPos;
    [SerializeField] private Transform[] _scatterPositions = new Transform[4];

    #region Properties
    public int CurrentPosition { get { return _currentPosition; } private set {  _currentPosition = value; } }
    #endregion



    void OnEnable()
    {
        ItemCollection.OnItemCollected += PelletCollected;
        EnemyCollision.OnEnemyCollision += RestartPosition;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.Warp(_startingPos);
        CurrentPosition = 0;
        _agent.destination = _scatterPositions[CurrentPosition].position;   
    }

    void FixedUpdate()
    {
        if (_currentState == EnemyState.Scatter && _agent.hasPath)
        {
            Debug.Log("Agent Position: " + _agent.transform.position);
            Debug.Log("Agent Destination: " + _agent.destination);
            Debug.Log("Agent Remaining Distance: " + _agent.remainingDistance);

            if (_agent.remainingDistance < 1.5f)        // Agent should be moving to element 1
            {
                CalculateNextDestination();     // If 2 >= 4, False - increase to 3
            }
        }
        else if (_currentState == EnemyState.Chase)      // destination is player position
        {
            _agent.destination = _player.position;
        }
        else if (_currentState == EnemyState.Frightened)
        {

        }
        else
            Debug.Log("Invalid State");
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

    void CalculateNextDestination()
    {
        Debug.Log("test");
        if(CurrentPosition >= _scatterPositions.Length -1)
        {
            CurrentPosition = 0;
        }
        else
        {
            CurrentPosition++;
        }

        _agent.destination = _scatterPositions[_currentPosition].position;      // Element 2
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
