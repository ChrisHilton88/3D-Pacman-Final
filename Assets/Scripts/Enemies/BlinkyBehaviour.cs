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

    private int _blinkyCurrentPosition;
    private int _randomNumber;

    private float _minSpeed = 5;
    private float _minTunnelSpeed = 2.5f;
    private float _maxSpeed = 10;

    private const float _speedIncrement = 0.02f;       // (10% - 5% / 240) = 5/240. Or, (maximum allowed speed - starting speed / total pellets)

    private readonly Vector3 _blinkyStartingPos = new Vector3(0.5f, 0, 8.5f);

    NavMeshAgent _agent;
    Animator _animator;
    Coroutine _frightenedRoutine;
    WaitForSeconds _frightenedTimer = new WaitForSeconds(6f);

    [SerializeField] private GameObject _triggerCube;          
    [SerializeField] private Transform _pacmanTargetPos;
    [SerializeField] private Transform[] _blinkyScatterPositions = new Transform[4];

    #region Properties
    public int BlinkyCurrentPosition { get { return _blinkyCurrentPosition; } private set {  _blinkyCurrentPosition = value; } }
    #endregion



    void OnEnable()
    {
        EnemyCollision.OnEnemyCollision += RestartPosition;
        EnemyStateManager.OnNewState += SetNewState;
        ItemCollection.OnItemCollected += PelletCollected;
        ItemCollection.OnFrightened += FrightenedState;
        RoundManager.OnRoundEnd += RoundCompleted;
    }

    void OnDisable()
    {
        EnemyCollision.OnEnemyCollision -= RestartPosition;
        EnemyStateManager.OnNewState -= SetNewState;
        ItemCollection.OnItemCollected -= PelletCollected;
        ItemCollection.OnFrightened -= FrightenedState;
        RoundManager.OnRoundEnd -= RoundCompleted;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _frightenedRoutine = null;
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
                if (_agent.hasPath)
                {
                    Debug.DrawLine(transform.position, _blinkyScatterPositions[BlinkyCurrentPosition].position, Color.red);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                }
                break;

            case EnemyState.Chase:
                _agent.destination = _pacmanTargetPos.position;       // Needs to be continually updating for Player position
                Debug.DrawLine(transform.position, _pacmanTargetPos.position, Color.red);       // The line is correct when changing states
                break;

            case EnemyState.Frightened:
                if (_agent.remainingDistance < 1.5f)
                {
                    GenerateRandomFrightenedPosition();
                }

                Debug.DrawLine(transform.position, _agent.destination, Color.red);  
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
        if(BlinkyCurrentPosition >= _blinkyScatterPositions.Length -1)
        {
            BlinkyCurrentPosition = 0;
        }
        else
        {
            Debug.Log("test");
            BlinkyCurrentPosition++;
        }

        _agent.destination = _blinkyScatterPositions[BlinkyCurrentPosition].position;
    }

    bool GenerateRandomFrightenedPosition()
    {
        _randomNumber = EnemyStateManager.Instance.RandomNumber();
        Transform temp = EnemyStateManager.Instance.FrightenedPositions[_randomNumber];
        float distance = Vector3.Distance(temp.position, _pacmanTargetPos.position);

        if (distance > 20)
        {
            _agent.destination = temp.position;
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator FrightenedRoutineTimer(EnemyState previousState, string state)
    {
        yield return _frightenedTimer;      // Wait for cached time (6 secs)
        _currentState = previousState;      // Return to previous state
        _animator.SetTrigger("To" + state);     // Set Animator to previous state
        _frightenedRoutine = null;
    }


    #region Events
    // Event that handles resetting the enemies position during a round when the player dies
    void RestartPosition()
    {
        _agent.isStopped = true;
        _agent.Warp(_blinkyStartingPos);
        _agent.isStopped = false;
    }

    // Event that handles cycling through Chase & Scatter states
    void SetNewState()
    {
        if (_currentState == EnemyState.Chase)
        {
            _currentState = EnemyState.Scatter;
            _animator.SetTrigger("ToScatter");

            if (_animator != null)
            {
                _agent.destination = _blinkyScatterPositions[BlinkyCurrentPosition].position;          // We can have this here because the boxes are static
                _agent.isStopped = false;
            }
            else
                Debug.Log("Animator is NULL 1 in SetNewState() - BlinkyBehaviour");
        }
        else if (_currentState == EnemyState.Scatter)
        {
            _currentState = EnemyState.Chase;

            if (_animator != null)
            {
                _animator.SetTrigger("ToChase");
            }
            else
                Debug.Log("Animator is NULL 2 in SetNewState() - BlinkyBehaviour");
        }
    }

    // Event that handles incrementing agent speed when a pellet is collected
    void PelletCollected(int value)
    {
        IncrementAgentSpeed();
    }
    
    // Event that handles setting the enemy's state to Frightened
    void FrightenedState()
    {
        EnemyState tempState = _currentState;       // Store the current state so we can switch back to it once the timer has ended
        string state = tempState.ToString();
        _currentState = EnemyState.Frightened;      // Set new state to Frightened
        _animator.SetTrigger("ToFrightened");

        while (!GenerateRandomFrightenedPosition());

        if (_frightenedRoutine == null)
            _frightenedRoutine = StartCoroutine(FrightenedRoutineTimer(tempState, state));     // Start 6 second timer
    }

    // Event that handles the successful completion of a round
    void RoundCompleted()
    {
        _agent.isStopped = true;
        _agent.Warp(_blinkyStartingPos);
        _agent.isStopped = false;
        BlinkyCurrentPosition = 0;
        _agent.speed = _minSpeed;
        _currentState = EnemyState.Scatter;
        _triggerCube.SetActive(true);
        _agent.destination = _blinkyScatterPositions[BlinkyCurrentPosition].position;
    }
    #endregion
}
