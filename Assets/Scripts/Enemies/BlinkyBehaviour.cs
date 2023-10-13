using System.Runtime.CompilerServices;
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

    private float _minSpeed = 5;
    private float _minTunnelSpeed = 2.5f;
    private float _maxSpeed = 10;

    private const float _speedIncrement = 0.02f;       // (10% - 5% / 240) = 5/240. Or, (maximum allowed speed - starting speed / total pellets)

    private readonly Vector3 _blinkyStartingPos = new Vector3(0.5f, 0, 8.5f);

    NavMeshAgent _agent;
    Animator _animator;

    [SerializeField] private int _blinkyCurrentPosition;       // Scatter mode waypoint incrementer
    [SerializeField] private GameObject _triggerCube;          // Trigger cube for Pinky to start moving
    [SerializeField] private Transform _playerTargetPos;
    [SerializeField] private Transform[] _blinkyScatterPositions = new Transform[4];

    #region Properties
    public int BlinkyCurrentPosition { get { return _blinkyCurrentPosition; } private set {  _blinkyCurrentPosition = value; } }
    #endregion



    void OnEnable()
    {
        ItemCollection.OnItemCollected += PelletCollected;
        EnemyCollision.OnEnemyCollision += RestartPosition;
        EnemyStateManager.OnNewState += SetNewState;
        RoundManager.OnRoundStart += RoundCompleted;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();   
        _agent.Warp(_blinkyStartingPos);
        BlinkyCurrentPosition = 0;
        _agent.destination = _blinkyScatterPositions[BlinkyCurrentPosition].position;   
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
                if (_agent.hasPath)     // If agent is currently moving to it's destination
                {
                    Debug.DrawLine(transform.position, _blinkyScatterPositions[BlinkyCurrentPosition].position, Color.red);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                }
                break;

            case EnemyState.Chase:
                _agent.SetDestination(_playerTargetPos.position);
                Debug.DrawLine(transform.position, _playerTargetPos.position, Color.red);       // The line is correct when changing states
                break;

            case EnemyState.Frightened:
                break;

            default:
                _agent.isStopped = true;
                break;
        }
    }

    // Cycle through these 2 states
    void SetNewState()
    {
        if(_currentState == EnemyState.Chase)
        {
            _currentState = EnemyState.Scatter;

            if (_animator != null)
            {
                _agent.destination = _blinkyScatterPositions[_blinkyCurrentPosition].position;          // We can have this here because the boxes are static
                _animator.SetTrigger("ToScatter");
                _agent.isStopped = false;
            }
            else
                Debug.Log("Animator is NULL in SetNewState() - BlinkyBehaviour");
        }
        else if(_currentState == EnemyState.Scatter)
        {
            _currentState = EnemyState.Chase;
            if (_animator != null)
            {
                _animator.SetTrigger("ToChase");
                BlinkyCurrentPosition = 0;
            }
            else
                Debug.Log("Animator is NULL in SetNewState() - BlinkyBehaviour");
        }
    }

    public void DecrementSpeed()
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
        if(BlinkyCurrentPosition >= _blinkyScatterPositions.Length -1)
        {
            BlinkyCurrentPosition = 0;
        }
        else
        {
            BlinkyCurrentPosition++;
        }

        _agent.destination = _blinkyScatterPositions[BlinkyCurrentPosition].position;
    }

    #region Events

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

    void PelletCollected(int value)
    {
        IncrementAgentSpeed();
    }

    // Handles the successful completion of a round
    void RoundCompleted()
    {
        _agent.Warp(_blinkyStartingPos);
        BlinkyCurrentPosition = 0;
        _agent.speed = _minSpeed;
        _currentState = EnemyState.Scatter;
        _triggerCube.SetActive(true);
    }

    // Handles only resetting the enemies position during a round when the player dies
    void RestartPosition()
    {
        _agent.Warp(_blinkyStartingPos);
    }
    #endregion

    void OnDisable()
    {
        ItemCollection.OnItemCollected -= PelletCollected; 
        EnemyCollision.OnEnemyCollision -= RestartPosition;
        EnemyStateManager.OnNewState -= SetNewState;
    }
}
