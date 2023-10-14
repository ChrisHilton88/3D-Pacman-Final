using System.Collections;
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

    private int _randomNumber;
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
    Coroutine _frightenedRoutine;
    WaitForSeconds _frightenedTimer = new WaitForSeconds(6f);

    [SerializeField] private Transform _playerTargetPos;
    [SerializeField] private Transform _pacmanPos;
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
        ItemCollection.OnFrightened += FrightenedState;
        RoundManager.OnRoundEnd += RoundCompleted;
    }

    void OnDisable()
    {
        EnemyCollision.OnEnemyCollision -= RestartPosition;
        EnemyStateManager.OnNewState -= SetNewState;
        ItemCollection.OnFrightened -= FrightenedState;
        RoundManager.OnRoundEnd -= RoundCompleted;
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
        if (ClydeCanMove & _agent.hasPath)
        {
            switch (_currentState)
            {
                case EnemyState.Scatter:
                    _agent.isStopped = false;
                    Debug.DrawLine(transform.position, _clydeScatterPositions[ClydeCurrentScatterPosition].position, Color.yellow);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextScatterDestination();
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

                        if (_agent.remainingDistance < 1.5f)
                        {
                            CalculateNextChaseDestination();
                        }

                        Debug.DrawLine(transform.position, _clydeChasePositions[ClydeCurrentChasePosition].position, Color.yellow);
                    }
                    break;

                case EnemyState.Frightened:
                    if(_agent.remainingDistance < 1.5f)
                    {
                        GenerateRandomFrightenedPosition();
                    }

                    Debug.DrawLine(transform.position, _agent.destination, Color.yellow);
                    break;

                default:
                    _agent.isStopped = true;
                    break;
            }
        }
        else
            return;
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
        _animator.SetTrigger("To" + state);
        _frightenedRoutine = null;
    }

    // Called when the total pellets collected equals 80
    public void StartMovement()
    {
        Debug.Log("Clyde Pellet Count: " + PelletManager.Instance.PelletTally);
        _agent.destination = _clydeScatterPositions[ClydeCurrentScatterPosition].position;
        ClydeCanMove = true;
    }


    #region Events
    void FrightenedState()
    {
        EnemyState tempState = _currentState;       // Store the current state so we can switch back to it once the timer has ended
        string state = tempState.ToString();
        _currentState = EnemyState.Frightened;      // Set new state to Frightened
        _animator.SetTrigger("ToFrightened");

        if (_frightenedRoutine == null)
            _frightenedRoutine = StartCoroutine(FrightenedRoutineTimer(tempState, state));     // Start 6 second timer

        if (ClydeCanMove)
        {
            while (!GenerateRandomFrightenedPosition()) ;
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

                if (_animator != null && ClydeCanMove)
                {
                    _agent.destination = _clydeScatterPositions[ClydeCurrentScatterPosition].position;          // We can have this here because the boxes are static
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
        _agent.Warp(_clydeStartingPos);
        _agent.isStopped = false;
        ClydeCurrentScatterPosition = 0;
        _agent.speed = _minSpeed;
        _currentState = EnemyState.Scatter;
    }

    // Event that handles resetting the enemies position during a round when the player dies
    void RestartPosition()
    {
        _agent.isStopped = true;
        _agent.Warp(_clydeStartingPos);
        _agent.isStopped = false;
    }
    #endregion
}
