using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider), typeof(Animator))] 
public abstract class EnemyBase : MonoBehaviour
{
    protected enum EnemyState
    {
        Scatter,
        Chase,
        Frightened
    }

    protected EnemyState _currentState;

    protected int _currentPosition;
    protected int _randomFrightenedLocation;

    protected float _minSpeed = 5f;
    protected float _minTunnelSpeed = 2.5f;
    protected float _maxSpeed = 10f;

    protected const float _speedIncrement = 0.02f;     // (10% - 5% / 240) = 5/240. Or, (maximum allowed speed - starting speed / total pellets)

    protected Vector3 _startingPosition;     // Set own starting position

    protected NavMeshAgent _agent;
    protected Animator _animator;
    protected Coroutine _frightenedRoutine;
    protected WaitForSeconds _frightenedTimer = new WaitForSeconds(6f);
    protected Transform _pacmanTargetPos;

    protected Transform[] _scatterPositions;


    #region Properties
    public int CurrentPosition { get { return _currentPosition; } protected set { _currentPosition = value; } }   
    #endregion



    protected virtual void OnEnable()
    {
        EnemyCollision.OnEnemyCollision += RestartPosition;
        EnemyStateManager.OnNewState += SetNewState;
        ItemCollection.OnFrightened += FrightenedState;
        RoundManager.OnRoundEnd += RoundCompleted;
    }

    protected virtual void OnDisable()
    {
        EnemyCollision.OnEnemyCollision -= RestartPosition;
        EnemyStateManager.OnNewState -= SetNewState;
        ItemCollection.OnFrightened -= FrightenedState;
        RoundManager.OnRoundEnd -= RoundCompleted;
    }

    // The base class's Start method will be automatically called when the game starts for any object that has the derived class script attached.
    // In other words, you don't need to re-implement this method in each derived class unless you have a specific need to do so.
    protected virtual void Start()      
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _frightenedRoutine = null;
        _agent.Warp(_startingPosition);
        CurrentPosition = 0;
        _agent.destination = _scatterPositions[CurrentPosition].position;
    }

    protected virtual void FixedUpdate()
    {
        CheckState();
    }

    protected abstract void EnemyInitialisation();

    protected abstract void CheckState();           // Enemies implement their own State Behaviour

    #region Void Methods
    // Scatter mode waypoint system
    protected virtual void CalculateNextDestination()       // Only derived classes can inherit this method and unless they use the Override keyword, it cannot be changed
    {
        if (CurrentPosition >= _scatterPositions.Length - 1)
        {
            CurrentPosition = 0;
        }
        else
        {
            CurrentPosition++;
        }

        _agent.destination = _scatterPositions[CurrentPosition].position;
    }

    // Increments agents speed everytime a pellet is collected (Blinky, Pinky & Inky)
    private void IncrementAgentSpeed()
    {
        if (_agent.speed < _maxSpeed)
            _agent.speed += _speedIncrement;
        else
        {
            _agent.speed = _maxSpeed;
            return;
        }
    }
    #endregion

    #region Return Methods
    // Generates and returns a random frightened location
    protected virtual bool GenerateRandomFrightenedPosition()
    {
        _randomFrightenedLocation = EnemyStateManager.Instance.RandomNumber();
        Transform temp = EnemyStateManager.Instance.FrightenedPositions[_randomFrightenedLocation];
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
    #endregion

    #region Public Methods
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
    #endregion

    #region Coroutines
    IEnumerator FrightenedRoutineTimer(EnemyState previousState, string state, Vector3 previousDestination, Action onComplete)
    {
        yield return _frightenedTimer;      // Wait for cached time (6 secs)
        _currentState = previousState;      // Return to previous state
        _agent.destination = previousDestination;
        _animator.SetTrigger("To" + state);     // Set Animator to previous state
        _frightenedRoutine = null;
        onComplete?.Invoke();   
    }
    #endregion

    #region Events
    // Event that handles resetting the enemies position during a round when the player dies
    void RestartPosition()
    {
        _agent.isStopped = true;
        _agent.Warp(_startingPosition);
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
                _agent.destination = _scatterPositions[CurrentPosition].position;          // We can have this here because the boxes are static
            }
            else
                Debug.Log("Animator is NULL Chase - EnemyBase");
        }
        else if (_currentState == EnemyState.Scatter)
        {
            _currentState = EnemyState.Chase;

            if (_animator != null)
            {
                _animator.SetTrigger("ToChase");
            }
            else
                Debug.Log("Animator is NULL Scatter - EnemyBase");
        }
    }

    // Event that handles incrementing agent speed when a pellet is collected
    protected virtual void PelletCollected(int value)
    {
        IncrementAgentSpeed();
    }

    // Event that handles setting the enemy's state to Frightened
    void FrightenedState()
    {
        EnemyState tempState = _currentState;       // Store the current state so we can switch back to it once the timer has ended
        string state = tempState.ToString();
        Vector3 previousDestination = _agent.destination;
        _currentState = EnemyState.Frightened;      // Set new state to Frightened
        _animator.SetTrigger("ToFrightened");

        while (!GenerateRandomFrightenedPosition());

        if (_frightenedRoutine == null)
            _frightenedRoutine = StartCoroutine(FrightenedRoutineTimer(tempState, state, previousDestination, () =>
            {

            }));     
    }

    // Event that handles the successful completion of a round
    protected virtual void RoundCompleted()
    {
        _agent.isStopped = true;
        _agent.Warp(_startingPosition);
        CurrentPosition = 0;
        _agent.speed = _minSpeed;
        _currentState = EnemyState.Scatter;
    }
    #endregion
}
