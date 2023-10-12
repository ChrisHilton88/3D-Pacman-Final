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

    private readonly Vector3 _blinkyStartingPos = new Vector3(0.5f, 0, 8.5f);

    NavMeshAgent _agent;

    [SerializeField] private int _blinkyCurrentPosition;       // Scatter mode waypoint incrementer
    [SerializeField] private Transform _playerTargetPos;
    [SerializeField] private Transform[] _blinkyScatterPositions = new Transform[4];

    #region Properties
    public int BlinkyCurrentPosition { get { return _blinkyCurrentPosition; } private set {  _blinkyCurrentPosition = value; } }
    #endregion



    void OnEnable()
    {
        ItemCollection.OnItemCollected += PelletCollected;
        EnemyCollision.OnEnemyCollision += RestartPosition;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.Warp(_blinkyStartingPos);
        BlinkyCurrentPosition = 0;
        _agent.destination = _blinkyScatterPositions[BlinkyCurrentPosition].position;   
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
                if (_agent.hasPath)
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

    // Scatter mode waypoint system
    void CalculateNextDestination()
    {
        if(BlinkyCurrentPosition >= _blinkyScatterPositions.Length -1)
        {
            BlinkyCurrentPosition = 0;
        }
        else
        {
            BlinkyCurrentPosition++;
        }

        Debug.Log("Current Pos: " + BlinkyCurrentPosition);
        _agent.destination = _blinkyScatterPositions[BlinkyCurrentPosition].position;
        Debug.Log("Current Pos: " + BlinkyCurrentPosition);

    }

    #region Events
    void PelletCollected(int value)
    {
        IncrementAgentSpeed();
    }

    void RestartPosition()
    {
        _agent.Warp(_blinkyStartingPos);
    }
    #endregion

    void OnDisable()
    {
        ItemCollection.OnItemCollected -= PelletCollected; 
        EnemyCollision.OnEnemyCollision -= RestartPosition; 
    }
}
