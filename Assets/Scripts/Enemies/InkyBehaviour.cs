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
    public int StartRandomValue {  get { return _startRandomValue; } private set { _startRandomValue = value; } }   
    private int _minStartValue = 30, _maxStartValue = 40;     // 30% & 40% of total pellet count (240)
    private int _maxSpeed = 10;

    private const float _speedIncrement = 0.02f;       // (10% - 5% / 240) = 5/240. Or, (maximum allowed speed - starting speed / total pellets)

    private bool _inkyCanMove;
    public bool InkyCanMove { get { return _inkyCanMove; }  private set { _inkyCanMove = value; } } 

    private readonly Vector3 _startingPos = new Vector3(-5f, 0, 0f);

    NavMeshAgent _agent;

    [SerializeField] private int _inkyCurrentPosition;
    [SerializeField] private Transform _playerTargetPos;
    [SerializeField] private Transform _blinkyPos;
    [SerializeField] private Transform[] _inkyScatterPositions = new Transform[4];

    #region Properties
    public int InkyCurrentPosition { get { return _inkyCurrentPosition; } private set { _inkyCurrentPosition = value; } }
    #endregion


    void OnEnable()
    {
        ItemCollection.OnItemCollected += PelletCollected;
        EnemyCollision.OnEnemyCollision += RestartPosition;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _minStartValue = (240 * _minStartValue) / 100;      
        _maxStartValue = (240 * _maxStartValue) / 100;
        _startRandomValue = RandomNumber(_minStartValue, _maxStartValue);
        InkyCanMove = false;
        _agent.Warp(_startingPos);
        InkyCurrentPosition = 0;
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
                if (InkyCanMove && _agent.hasPath)
                {
                    _agent.isStopped = false;

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                }
                break;

            case EnemyState.Chase:
                Vector3 targetTile = (_blinkyPos.position - _playerTargetPos.position) * 2.0f;
                _agent.destination = targetTile;
                //Debug.Log(targetTile);
                //Debug.DrawLine(transform.position, targetTile * 2.0f, Color.blue);
                break;

            case EnemyState.Frightened:
                break;

            default:
                _agent.isStopped = true;
                break;
        }
    }

    void CalculateNextDestination()
    {
        if (InkyCurrentPosition >= _inkyScatterPositions.Length - 1)
            InkyCurrentPosition = 0;
        else
            InkyCurrentPosition++;

        _agent.destination = _inkyScatterPositions[InkyCurrentPosition].position;
    }

    // Called when the total pellets collected equals the random number
    public void StartMovement()
    {
        _agent.destination = _inkyScatterPositions[InkyCurrentPosition].position;
        InkyCanMove = true;
    }

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

    int RandomNumber(int min, int max)
    {
        int newValue = Random.Range(min, max + 1);   
        return newValue;
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
