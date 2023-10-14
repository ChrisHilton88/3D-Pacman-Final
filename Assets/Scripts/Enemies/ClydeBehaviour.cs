using UnityEngine;
using UnityEngine.AI;

public class ClydeBehaviour : MonoBehaviour
{
    private enum EnemyState
    {
        Scatter,
        Chase,
        Frightened
    }
    private EnemyState _currentState;

    private int _movePelletCount;
    private int _clydeCurrentScatterPosition;       // Scatter mode waypoint incrementer
    private int _clydeCurrentChasePosition;

    private float _maxDistance = 8;
    private float _minSpeed = 5;
    private float _minTunnelSpeed = 2.5f;
    private const float _speedIncrement = 0.02f;       // (10% - 5% / 240) = 5/240. Or, (maximum allowed speed - starting speed / total pellets)

    private bool _clydeCanMove;

    private readonly Vector3 _clydeStartingPos = new Vector3(6f, 0, -0.25f);

    NavMeshAgent _agent;
    Animator _animator;

    [SerializeField] private Transform _playerTargetPos;
    [SerializeField] private Transform[] _clydeScatterPositions = new Transform[4];
    [SerializeField] private Transform[] _clydeChasePositions = new Transform[4];

    #region Properties
    public int MovePelletCount { get { return _movePelletCount; } private set { _movePelletCount = value; } }
    public bool ClydeCanMove { get { return _clydeCanMove; } private set { _clydeCanMove = value; } }
    public int ClydeCurrentScatterPosition { get { return _clydeCurrentScatterPosition; } private set { _clydeCurrentScatterPosition = value; } }
    public int ClydeCurrentChasePosition { get { return _clydeCurrentChasePosition; } private set { _clydeCurrentChasePosition = value; } }
    #endregion



    void OnEnable()
    {
        EnemyCollision.OnEnemyCollision += RestartPosition;
        EnemyStateManager.OnNewState += SetNewState;
        RoundManager.OnRoundStart += RoundCompleted;
    }

    void OnDisable()
    {
        EnemyCollision.OnEnemyCollision -= RestartPosition;
        EnemyStateManager.OnNewState -= SetNewState;
        RoundManager.OnRoundStart -= RoundCompleted;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();   
        MovePelletCount = 80;       // 1/3 of total pellet count
        ClydeCanMove = false;
        _agent.Warp(_clydeStartingPos);
        ClydeCurrentScatterPosition = 0;
        ClydeCurrentChasePosition = 0;
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
                if (ClydeCanMove && _agent.hasPath)
                {
                    _agent.isStopped = false;
                    Debug.DrawLine(transform.position, _clydeScatterPositions[ClydeCurrentScatterPosition].position, Color.yellow);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextScatterDestination();
                    }
                }
                break;

            case EnemyState.Chase:
                if (Vector2.Distance(transform.position, _playerTargetPos.position) > _maxDistance)     // If distance between Clyde and pacman is greater than 8 tiles
                {
                    _agent.destination = _playerTargetPos.position;     // Same target as Blinky - Pacmans current tile
                    Debug.DrawLine(transform.position, _playerTargetPos.position, Color.yellow);        // Should be to Pacman
                }
                else
                {
                    _agent.destination = _clydeChasePositions[ClydeCurrentChasePosition].position;
                    // Equal to the waypoint loop down the bottom of the map
                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextChaseDestination();
                    }

                    Debug.DrawLine(transform.position, _clydeChasePositions[ClydeCurrentChasePosition].position, Color.yellow);
                }
                break;

            case EnemyState.Frightened:
                break;

            default:
                _agent.isStopped = true;
                break;
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
    void CalculateNextScatterDestination()
    {
        if (ClydeCurrentScatterPosition >= _clydeScatterPositions.Length - 1)
        {
            ClydeCurrentScatterPosition = 0;
        }
        else
        {
            ClydeCurrentScatterPosition++;
        }

        _agent.destination = _clydeScatterPositions[ClydeCurrentScatterPosition].position;
    }

    void CalculateNextChaseDestination()
    {
        if (ClydeCurrentChasePosition >= _clydeChasePositions.Length - 1)
        {
            ClydeCurrentChasePosition = 0;
        }
        else
        {
            ClydeCurrentChasePosition++;
        }

        _agent.destination = _clydeChasePositions[ClydeCurrentChasePosition].position;
    }

    // Called when the total pellets collected equals 80
    public void StartMovement()
    {
        Debug.Log("Clyde Pellet Count: " + PelletManager.Instance.PelletTally);
        _agent.destination = _clydeScatterPositions[ClydeCurrentScatterPosition].position;
        ClydeCanMove = true;
    }


    #region Events
    // Handles cycling through Chase & Scatter states
    void SetNewState()
    {
        if (_currentState == EnemyState.Chase)
        {
            _currentState = EnemyState.Scatter;
            Debug.Log("Clyde Current State: " + _currentState);

            if (_animator != null)
            {
                _agent.destination = _clydeScatterPositions[ClydeCurrentScatterPosition].position;          // We can have this here because the boxes are static
                _animator.SetTrigger("ToScatter");
                _agent.isStopped = false;
            }
            else
                Debug.Log("Animator is NULL 1 in SetNewState() - ClydeBehaviour");
        }
        else if (_currentState == EnemyState.Scatter)
        {
            _currentState = EnemyState.Chase;
            Debug.Log("Clyde Current State: " + _currentState);

            if (_animator != null)
            {
                _animator.SetTrigger("ToChase");
            }
            else
                Debug.Log("Animator is NULL 2 in SetNewState() - ClydeBehaviour");
        }
    }

    // Event that handles the successful completion of a round
    void RoundCompleted()
    {
        _agent.Warp(_clydeStartingPos);
        ClydeCurrentScatterPosition = 0;
        _agent.speed = _minSpeed;
        _currentState = EnemyState.Scatter;
    }

    // Event that handles resetting the enemies position during a round when the player dies
    void RestartPosition()
    {
        _agent.Warp(_clydeStartingPos);
    }
    #endregion
}
