using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider), typeof(Animator))]
public class PinkyBehaviour : MonoBehaviour
{
    private enum EnemyState
    {
        Scatter,
        Chase,
        Frightened
    }
    private EnemyState _currentState;

    private int _maxSpeed = 10;

    private const float _speedIncrement = 0.02f;

    private bool _pinkyCanMove;
    public bool PinkyCanMove { get { return _pinkyCanMove; } private set { _pinkyCanMove = value; } }

    private readonly Vector3 _startingPos = new Vector3(0.25f, 0, 0);

    NavMeshAgent _agent;

    [SerializeField] private int _currentPosition;       // Scatter mode waypoint incrementer
    [SerializeField] private Transform _playerTargetPos;
    [SerializeField] private Transform[] _scatterPositions = new Transform[4];

    #region Properties
    public int CurrentPosition { get { return _currentPosition; } private set { _currentPosition = value; } }
    #endregion

    // Blinky starts directly above the exit
    // As soon as Blinky moves out of the doorway, Pinky can leave
    // Pinky speed is the same as Blinky & Inky, which means that this AI can also subscribe to the pellet collecting event

    void OnEnable()
    {
        ItemCollection.OnItemCollected += PelletCollected;
        EnemyCollision.OnEnemyCollision += RestartPosition;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        PinkyCanMove = false;
        _agent.Warp(_startingPos);
        CurrentPosition = 0;
    }

    void FixedUpdate()
    {
        SwitchStates();
    }

    void SwitchStates()
    {
        Debug.Log("Pinky State: " + _currentState);

        switch (_currentState)
        {
            case EnemyState.Scatter:
                Debug.Log("Pinky CanMove: " + PinkyCanMove);
                if (PinkyCanMove && _agent.hasPath)
                {
                    _agent.isStopped = false;

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                }
                break;

            case EnemyState.Chase:
                _agent.destination = _playerTargetPos.position;
                break;

            case EnemyState.Frightened:
                break;

            default:
                _agent.isStopped = true;
                break;
        }
    }

    // TODO - When Reset level takes place, reset enemy speed
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

    // Scatter mode waypoint system
    void CalculateNextDestination()
    {
        if (CurrentPosition >= _scatterPositions.Length - 1)
            CurrentPosition = 0;
        else
            CurrentPosition++;

        _agent.destination = _scatterPositions[_currentPosition].position;      
    }

    // Once Blinky has moved outside of his start box - Set destination for Pinky to start moving
    public void StartMoving()
    {
        _agent.destination = _scatterPositions[CurrentPosition].position;
        PinkyCanMove = true;
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
