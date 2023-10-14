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

    private float _minSpeed = 5f;
    private float _minTunnelSpeed = 2.5f;
    private float _maxSpeed = 10f;

    private const float _speedIncrement = 0.02f;

    private bool _pinkyCanMove;
    public bool PinkyCanMove { get { return _pinkyCanMove; } private set { _pinkyCanMove = value; } }

    private readonly Vector3 _pinkyStartingPos = new Vector3(0.25f, 0, 0);

    NavMeshAgent _agent;
    Animator _animator;

    [SerializeField] private int _pinkyCurrentPosition;       // Scatter mode waypoint incrementer
    [SerializeField] private Transform _playerTargetPos;
    [SerializeField] private Transform[] _pinkyScatterPositions = new Transform[4];

    #region Properties
    public int PinkyCurrentPosition { get { return _pinkyCurrentPosition; } private set { _pinkyCurrentPosition = value; } }
    #endregion

    // Blinky starts directly above the exit
    // As soon as Blinky moves out of the doorway, Pinky can leave
    // Pinky speed is the same as Blinky & Inky, which means that this AI can also subscribe to the pellet collecting event

    void OnEnable()
    {
        ItemCollection.OnItemCollected += PelletCollected;
        EnemyCollision.OnEnemyCollision += RestartPosition;
        EnemyStateManager.OnNewState += SetNewState;
        RoundManager.OnRoundStart += RoundCompleted;
    }

    void OnDisable()
    {
        ItemCollection.OnItemCollected -= PelletCollected;
        EnemyCollision.OnEnemyCollision -= RestartPosition;
        EnemyStateManager.OnNewState -= SetNewState;
        RoundManager.OnRoundStart -= RoundCompleted;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();   
        PinkyCanMove = false;
        _agent.Warp(_pinkyStartingPos);
        PinkyCurrentPosition = 0;
    }

    void FixedUpdate()
    {
        CheckState();
    }

    void CheckState()
    {
        switch (_currentState)
        {
            case EnemyState.Scatter:
                if (PinkyCanMove && _agent.hasPath)
                {
                    _agent.isStopped = false;
                    Debug.DrawLine(transform.position, _pinkyScatterPositions[PinkyCurrentPosition].position, Color.green);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                }
                break;

            case EnemyState.Chase:
                _agent.destination = _playerTargetPos.position;
                Debug.DrawLine(transform.position, _playerTargetPos.position, Color.green);  
                break;

            case EnemyState.Frightened:
                break;

            default:
                _agent.isStopped = true;
                break;
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

    // Decrement agent speed whilst standing in OnTriggerStay in Tunnel
    public void DecrementAgentSpeed()
    {
        if (_agent.speed >= _minTunnelSpeed)
        {
            _agent.speed -= _speedIncrement;
        }
        else
        {
            _agent.speed = _minTunnelSpeed;
        }
    }

    // Scatter mode waypoint system
    void CalculateNextDestination()
    {
        if (PinkyCurrentPosition >= _pinkyScatterPositions.Length - 1)
        {
            PinkyCurrentPosition = 0;
        }
        else
        {
            PinkyCurrentPosition++;
        }

        _agent.destination = _pinkyScatterPositions[PinkyCurrentPosition].position;      
    }

    // Once Blinky has moved outside of his start box - Set destination for Pinky to start moving
    public void StartMoving()
    {
        _agent.destination = _pinkyScatterPositions[PinkyCurrentPosition].position;
        PinkyCanMove = true;
    }


    #region Events
    // Event that handles incrementing agent speed when a pellet is collected
    void PelletCollected(int value)
    {
        IncrementAgentSpeed();
    }

    // Handles cycling through Chase & Scatter states
    void SetNewState()
    {
        if (_currentState == EnemyState.Chase)
        {
            _currentState = EnemyState.Scatter;
            Debug.Log("Pinky Current State: " + _currentState);

            if (_animator != null)
            {
                _agent.destination = _pinkyScatterPositions[_pinkyCurrentPosition].position;          // We can have this here because the boxes are static
                _animator.SetTrigger("ToScatter");
                _agent.isStopped = false;
            }
            else
                Debug.Log("Animator is NULL 1 in SetNewState() - PinkyBehaviour");
        }
        else if (_currentState == EnemyState.Scatter)
        {
            _currentState = EnemyState.Chase;
            Debug.Log("Pinky Current State: " + _currentState);

            if (_animator != null)
            {
                _animator.SetTrigger("ToChase");
                PinkyCurrentPosition = 0;
            }
            else
                Debug.Log("Animator is NULL 2 in SetNewState() - PinkyBehaviour");
        }
    }

    // Event that handles the successful completion of a round
    void RoundCompleted()
    {
        _agent.Warp(_pinkyStartingPos);
        PinkyCurrentPosition = 0;
        _agent.speed = _minSpeed;
        _currentState = EnemyState.Scatter;
    }

    // Event that handles resetting the enemies position during a round when the player dies
    void RestartPosition()
    {
        _agent.Warp(_pinkyStartingPos);
    }
    #endregion
}
