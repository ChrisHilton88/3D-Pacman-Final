using System.Collections;
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

    private int _pinkyCurrentPosition;       // Scatter mode waypoint incrementer
    private int _randomNumber;

    private float _minSpeed = 5f;
    private float _minTunnelSpeed = 2.5f;
    private float _maxSpeed = 10f;

    private const float _speedIncrement = 0.02f;

    private bool _pinkyCanMove;

    private readonly Vector3 _pinkyStartingPos = new Vector3(0.25f, 0, 0);

    NavMeshAgent _agent;
    Animator _animator;
    Coroutine _frightenedRoutine;
    WaitForSeconds _frightenedTimer = new WaitForSeconds(6f);

    [SerializeField] private Transform _pacmanTargetPos;
    [SerializeField] private Transform _pacmanPos;
    [SerializeField] private Transform[] _pinkyScatterPositions = new Transform[4];

    #region Properties
    public bool PinkyCanMove { get { return _pinkyCanMove; } private set { _pinkyCanMove = value; } }
    public int PinkyCurrentPosition { get { return _pinkyCurrentPosition; } private set { _pinkyCurrentPosition = value; } }
    #endregion

    // Blinky starts directly above the exit
    // As soon as Blinky moves out of the doorway, Pinky can leave
    // Pinky speed is the same as Blinky & Inky, which means that this AI can also subscribe to the pellet collecting event


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
        ItemCollection.OnFrightened += FrightenedState;
        RoundManager.OnRoundEnd -= RoundCompleted;
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
                    Debug.DrawLine(transform.position, _pinkyScatterPositions[PinkyCurrentPosition].position, Color.magenta);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                }
                break;

            case EnemyState.Chase:
                _agent.destination = _pacmanTargetPos.position;
                Debug.DrawLine(transform.position, _pacmanTargetPos.position, Color.magenta);  
                break;

            case EnemyState.Frightened:
                if (_agent.remainingDistance < 1.5f)
                {
                    GenerateRandomFrightenedPosition();
                }

                Debug.DrawLine(transform.position, _agent.destination, Color.magenta);
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

    IEnumerator FrightenedRoutineTimer(EnemyState previousState, string state)
    {
        yield return _frightenedTimer;      // Wait for cached time (6 secs)
        _currentState = previousState;      // Return to previous state
        _animator.SetTrigger("To" + state);     // Set Animator to previous state
        _frightenedRoutine = null;
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

    void FrightenedState()
    {
        if (PinkyCanMove)
        {
            EnemyState tempState = _currentState;       // Store the current state so we can switch back to it once the timer has ended
            string state = tempState.ToString();
            _currentState = EnemyState.Frightened;      // Set new state to Frightened
            _animator.SetTrigger("ToFrightened");

            while (!GenerateRandomFrightenedPosition()) ;

            if (_frightenedRoutine == null)
                _frightenedRoutine = StartCoroutine(FrightenedRoutineTimer(tempState, state));     // Start 6 second timer
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

            if (_animator != null)
            {
                _animator.SetTrigger("ToChase");
            }
            else
                Debug.Log("Animator is NULL 2 in SetNewState() - PinkyBehaviour");
        }
    }

    // Event that handles the successful completion of a round
    void RoundCompleted()
    {
        _agent.isStopped = true;
        _agent.Warp(_pinkyStartingPos);
        _agent.isStopped = false;
        PinkyCurrentPosition = 0;
        _agent.speed = _minSpeed;
        _currentState = EnemyState.Scatter;
    }

    // Event that handles resetting the enemies position during a round when the player dies
    void RestartPosition()
    {
        _agent.isStopped = true;
        _agent.Warp(_pinkyStartingPos);
        _agent.isStopped = false;
    }
    #endregion
}
