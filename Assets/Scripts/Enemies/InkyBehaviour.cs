using System.Collections;
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

    private int _inkyCurrentPosition;
    private int _randomNumber;
    private int _startRandomValue;       // Choose a starting value between 30 - 40% of total pellet count. This random value will be used to start moving Inky
    private int _minStartValue = 30, _maxStartValue = 40;     // 30% & 40% of total pellet count (240)

    private float _minSpeed = 5;
    private float _minTunnelSpeed = 2.5f;
    private float _maxSpeed = 10;
    private const float _speedIncrement = 0.02f;       

    private bool _inkyCanMove;

    private readonly Vector3 _inkyStartingPos = new Vector3(-5f, 0, 0f);

    NavMeshAgent _agent;
    Animator _animator;
    Coroutine _frightenedRoutine;
    WaitForSeconds _frightenedTimer = new WaitForSeconds(6f);

    [SerializeField] private Transform _pacmanTargetPos;
    [SerializeField] private Transform _pacmanPos;
    [SerializeField] private Transform _blinkyPos;
    [SerializeField] private Transform[] _inkyScatterPositions = new Transform[4];

    #region Properties
    public bool InkyCanMove { get { return _inkyCanMove; } private set { _inkyCanMove = value; } }
    public int StartRandomValue { get { return _startRandomValue; } private set { _startRandomValue = value; } }
    public int InkyCurrentPosition { get { return _inkyCurrentPosition; } private set { _inkyCurrentPosition = value; } }
    #endregion



    void OnEnable()
    {
        EnemyCollision.OnEnemyCollision += RestartPosition;
        EnemyStateManager.OnNewState += SetNewState;
        ItemCollection.OnFrightened += FrightenedState;
        ItemCollection.OnItemCollected += PelletCollected;
        RoundManager.OnRoundEnd += RoundCompleted;
    }
    void OnDisable()
    {
        EnemyCollision.OnEnemyCollision -= RestartPosition;
        EnemyStateManager.OnNewState -= SetNewState;
        ItemCollection.OnFrightened -= FrightenedState;
        ItemCollection.OnItemCollected -= PelletCollected;
        RoundManager.OnRoundEnd -= RoundCompleted;
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
        if (InkyCanMove && _agent.hasPath)
        {
            switch (_currentState)
            {
                case EnemyState.Scatter:
                    _agent.isStopped = false;
                    Debug.DrawLine(transform.position, _inkyScatterPositions[InkyCurrentPosition].position, Color.cyan);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                    break;

                case EnemyState.Chase:
                    Vector3 vectorToPlayer = _pacmanTargetPos.position - _blinkyPos.position;       // Calculate vector from Blinky to Players position
                    Vector3 doubledVector = vectorToPlayer * 2.0f;      // Double the length of the vector
                    Vector3 inkyTargetPosition = _blinkyPos.position + doubledVector;       // Inky's target = Adding doubled vector to Blinkys position
                    _agent.destination = inkyTargetPosition;        // Set destination = inkyTargetPosition
                    Debug.DrawLine(transform.position, inkyTargetPosition, Color.cyan);
                    break;

                case EnemyState.Frightened:
                    if (_agent.remainingDistance < 1.5f)
                    {
                        GenerateRandomFrightenedPosition();
                    }

                    Debug.DrawLine(transform.position, _agent.destination, Color.cyan);
                    break;

                default:
                    _agent.isStopped = true;
                    break;
            }
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

    // Generates a random number and uses this to grab the corresponding element in the Transform array to set the agent's destination
    bool GenerateRandomFrightenedPosition()
    {
        _randomNumber = EnemyStateManager.Instance.RandomNumber();

        Transform temp = EnemyStateManager.Instance.FrightenedPositions[_randomNumber];
        float distance = Vector3.Distance(temp.position, _pacmanPos.position);

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

    // Frightened State timer
    IEnumerator FrightenedRoutineTimer(EnemyState previousState, string state)
    {
        yield return _frightenedTimer;      // Wait for cached time (6 secs)
        _currentState = previousState;      // Return to previous state
        _animator.SetTrigger("To" + state);     // Set Animator to previous state
        _frightenedRoutine = null;
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

    // Event that handles changing to frightened state
    void FrightenedState()
    {
        EnemyState tempState = _currentState;       // Store the current state so we can switch back to it once the timer has ended
        string state = tempState.ToString();
        _currentState = EnemyState.Frightened;      // Set new state to Frightened
        _animator.SetTrigger("ToFrightened");

        if (_frightenedRoutine == null)
            _frightenedRoutine = StartCoroutine(FrightenedRoutineTimer(tempState, state));     // Start 6 second timer
        
        if (InkyCanMove)
        {
            while (!GenerateRandomFrightenedPosition());
        }
        else
            return;
    }

    // Handles cycling through Chase & Scatter states
    void SetNewState()
    {
        if (_currentState == EnemyState.Chase)
        {
            _currentState = EnemyState.Scatter;
            _animator.SetTrigger("ToScatter");

            if (_animator != null && InkyCanMove)
            {
                _agent.destination = _inkyScatterPositions[InkyCurrentPosition].position;          // We can have this here because the boxes are static
                _agent.isStopped = false;
            }
        }
        else if (_currentState == EnemyState.Scatter)
        {
            _currentState = EnemyState.Chase;
            _animator.SetTrigger("ToChase");
        }
    }

    // Event that handles the successful completion of a round
    void RoundCompleted()
    {
        _agent.isStopped = true;
        _agent.Warp(_inkyStartingPos);
        _agent.isStopped = false;
        InkyCurrentPosition = 0;
        _agent.speed = _minSpeed;
        _currentState = EnemyState.Scatter;
    }

    // Event that handles resetting the enemies position during a round when the player dies
    void RestartPosition()
    {
        _agent.isStopped = true;
        _agent.Warp(_inkyStartingPos);
        _agent.isStopped = false;
    }
    #endregion
}
