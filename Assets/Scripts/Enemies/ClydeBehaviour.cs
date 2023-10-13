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

    private int _maxDistance = 8;
    private int _movePelletCount;      
    private bool _clydeCanMove;

    private readonly Vector3 _startingPos = new Vector3(6f, 0, -0.25f);

    NavMeshAgent _agent;

    [SerializeField] private int _clydeCurrentPosition;       // Scatter mode waypoint incrementer
    [SerializeField] private Transform _playerTargetPos;
    [SerializeField] private Transform[] _clydeScatterPositions = new Transform[4];

    #region Properties
    public int MovePelletCount { get { return _movePelletCount; } private set { _movePelletCount = value; } }
    public bool ClydeCanMove { get { return _clydeCanMove; } private set { _clydeCanMove = value; } }
    public int ClydeCurrentPosition { get { return _clydeCurrentPosition; } private set { _clydeCurrentPosition = value; } }
    #endregion


    void OnEnable()
    {
        EnemyCollision.OnEnemyCollision += RestartPosition;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        MovePelletCount = 80;       // 1/3 of total pellet count
        ClydeCanMove = false;
        _agent.Warp(_startingPos);
        ClydeCurrentPosition = 0;
    }

    void FixedUpdate()
    {
        SwitchStates();
    }

    void SwitchStates()
    {
        switch (_currentState)
        {
            case EnemyState.Scatter:
                if (ClydeCanMove && _agent.hasPath)
                {
                    _agent.isStopped = false;
                    Debug.DrawLine(transform.position, _clydeScatterPositions[ClydeCurrentPosition].position, Color.magenta);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                }
                break;

            case EnemyState.Chase:
                if (Vector2.Distance(_playerTargetPos.position, transform.position) > _maxDistance)
                {
                    _agent.destination = _playerTargetPos.position;
                    Debug.DrawLine(transform.position, _playerTargetPos.position, Color.magenta);
                }
                else
                {
                    // Equal to the waypoint loop down the bottom of the map
                    // Use a bool to set this active path - Keep calculating distance and if it changes, set to false and chase Player
                }
                break;

            case EnemyState.Frightened:
                break;

            default:
                _agent.isStopped = true;
                break;
        }
    }

    // Scatter mode waypoint system
    void CalculateNextDestination()
    {
        if (ClydeCurrentPosition >= _clydeScatterPositions.Length - 1)
        {
            ClydeCurrentPosition = 0;
        }
        else
        {
            ClydeCurrentPosition++;
        }

        _agent.destination = _clydeScatterPositions[ClydeCurrentPosition].position;  
    }

    public void StartMovement()
    {
        _agent.destination = _clydeScatterPositions[ClydeCurrentPosition].position;
        ClydeCanMove = true;
    }

    #region Events
    void RestartPosition()
    {
        _agent.Warp(_startingPos);
    }
    #endregion

    void OnDisable()
    {
        EnemyCollision.OnEnemyCollision -= RestartPosition;
    }
}
