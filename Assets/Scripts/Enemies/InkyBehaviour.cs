using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider), typeof(Animator))]
public class InkyBehaviour : MonoBehaviour
{
    private enum EnemyState
    {
        Scatter,
        Chase,
        Frightened
    }
    private EnemyState _currentState;

    private int _startRandomValue;       // Choose a starting value between 30 - 40% of total pellet count. This random value will be used to start moving Inky
    private int _minStartValue = 30, _maxStartValue = 40;     // 30% & 40% of total pellet count (240)
    private float _minSpeed = 5;
    private float _minTunnelSpeed = 2.5f;
    private float _maxSpeed = 10;

    private const float _speedIncrement = 0.02f;       // (10% - 5% / 240) = 5/240. Or, (maximum allowed speed - starting speed / total pellets)

    private bool _inkyCanMove;
    public bool InkyCanMove { get { return _inkyCanMove; }  private set { _inkyCanMove = value; } } 

    private readonly Vector3 _inkyStartingPos = new Vector3(-5f, 0, 0f);

    NavMeshAgent _agent;
    Animator _animator; 

    [SerializeField] private int _inkyCurrentPosition;
    [SerializeField] private Transform _playerTargetPos;
    [SerializeField] private Transform _blinkyPos;
    [SerializeField] private Transform[] _inkyScatterPositions = new Transform[4];

    #region Properties
    public int StartRandomValue { get { return _startRandomValue; } private set { _startRandomValue = value; } }
    public int InkyCurrentPosition { get { return _inkyCurrentPosition; } private set { _inkyCurrentPosition = value; } }
    #endregion


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
        _minStartValue = (240 * _minStartValue) / 100;      
        _maxStartValue = (240 * _maxStartValue) / 100;
        _startRandomValue = RandomNumber(_minStartValue, _maxStartValue);
        InkyCanMove = false;
        _agent.Warp(_inkyStartingPos);
        InkyCurrentPosition = 0;
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
                if (InkyCanMove && _agent.hasPath)
                {
                    _agent.isStopped = false;
                    Debug.DrawLine(transform.position, _inkyScatterPositions[InkyCurrentPosition].position, Color.cyan);
                    
                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                }
                break;

            case EnemyState.Chase:
                
                Vector3 vectorToPlayer = _playerTargetPos.position - _blinkyPos.position;       // Calculate vector from Blinky to Players position
                Vector3 doubledVector = vectorToPlayer * 2.0f;      // Double the length of the vector
                Vector3 inkyTargetPosition = _blinkyPos.position + doubledVector;       // Inky's target = Adding doubled vector to Blinkys position
                _agent.destination = inkyTargetPosition;        // Set destination = inkyTargetPosition
                Debug.DrawLine(transform.position, inkyTargetPosition, Color.cyan);
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
        if (InkyCurrentPosition >= _inkyScatterPositions.Length - 1)
        {
            InkyCurrentPosition = 0;
        }
        else
        {
            InkyCurrentPosition++;
        }

        _agent.destination = _inkyScatterPositions[InkyCurrentPosition].position;
    }

    // Called when the total pellets collected equals the random number
    public void StartMovement()
    {
        _agent.destination = _inkyScatterPositions[InkyCurrentPosition].position;
        InkyCanMove = true;
    }


    int RandomNumber(int min, int max)
    {
        int newValue = Random.Range(min, max + 1);   
        return newValue;
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
            Debug.Log("Inky Current State: " + _currentState);

            if (_animator != null)
            {
                _agent.destination = _inkyScatterPositions[InkyCurrentPosition].position;          // We can have this here because the boxes are static
                _animator.SetTrigger("ToScatter");
                _agent.isStopped = false;
            }
            else
                Debug.Log("Animator is NULL 1 in SetNewState() - InkyBehaviour");
        }
        else if (_currentState == EnemyState.Scatter)
        {
            _currentState = EnemyState.Chase;
            Debug.Log("Inky Current State: " + _currentState);

            if (_animator != null)
            {
                _animator.SetTrigger("ToChase");
                InkyCurrentPosition = 0;
            }
            else
                Debug.Log("Animator is NULL 2 in SetNewState() - InkyBehaviour");
        }
    }

    // Event that handles the successful completion of a round
    void RoundCompleted()
    {
        _agent.Warp(_inkyStartingPos);
        InkyCurrentPosition = 0;
        _agent.speed = _minSpeed;
        _currentState = EnemyState.Scatter;
    }

    // Event that handles resetting the enemies position during a round when the player dies
    void RestartPosition()
    {
        _agent.Warp(_inkyStartingPos);
    }
    #endregion
}
