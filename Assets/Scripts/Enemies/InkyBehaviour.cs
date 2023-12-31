using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class InkyBehaviour : EnemyBase
{
    private int _startRandomValue;       // Choose a starting value between 30 - 40% of total pellet count. This random value will be used to start moving Inky
    private int _minStartValue = 30, _maxStartValue = 40;     // 30% & 40% of total pellet count (240)

    private bool _inkyCanMove;

    private readonly Vector3 _inkyStartingPosition = new Vector3(-5f, 0f, 0f);

    NavMeshPath _navMeshPath;   

    [SerializeField] private Transform[] _inkyScatterPositions;
    [SerializeField] private Transform _blinkyPos;
    [SerializeField] private Transform _inkyTargetPacmanPos;        // Double the length of the vector from 2 tiles in front of Pacmans position and Blinkys pos

    #region Properties
    public int StartRandomValue { get { return _startRandomValue; } private set { _startRandomValue = value; } }
    public bool InkyCanMove { get { return _inkyCanMove; } private set { _inkyCanMove = value; } }
    #endregion



    protected override void OnEnable()
    {
        base.OnEnable();
        ItemCollection.OnItemCollected += PelletCollected;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ItemCollection.OnItemCollected -= PelletCollected;
    }

    protected void Awake()
    {
        EnemyInitialisation();
    }

    protected override void EnemyInitialisation()
    {
        _navMeshPath = new NavMeshPath();
        _scatterPositions = _inkyScatterPositions;
        _startingPosition = _inkyStartingPosition;
        _pacmanTargetPos = _inkyTargetPacmanPos;
        InkyCanMove = false;
        _minStartValue = (240 * _minStartValue) / 100;
        _maxStartValue = (240 * _maxStartValue) / 100;
        StartRandomValue = RandomNumber(_minStartValue, _maxStartValue);
    }

    protected sealed override void CheckState()
    {
        if (InkyCanMove)
        {
            switch (_currentState)
            {
                case EnemyState.Scatter:
                    Debug.DrawLine(transform.position, _inkyScatterPositions[CurrentPosition].position, Color.cyan);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                    break;

                case EnemyState.Chase:
                    Vector3 vectorToPlayer = _pacmanTargetPos.position - _blinkyPos.position;       // Calculate vector from Blinky to Players position
                    Vector3 doubledVector = vectorToPlayer * 2.0f;                          // Double the length of the vector
                    Vector3 inkyTargetPosition = _blinkyPos.position + doubledVector;       // Inky's target = Adding doubled vector to Blinkys position

                    if (IsPositionReachable())
                    {
                        _agent.SetDestination(inkyTargetPosition);
                        Debug.DrawLine(transform.position, inkyTargetPosition, Color.cyan);
                    }
                    else
                    {
                        _agent.SetDestination(_pacmanTargetPos.position);
                        StartCoroutine(Test());
                        Debug.DrawLine(transform.position, _pacmanTargetPos.position, Color.cyan);
                    }
                    break;

                case EnemyState.Frightened:
                    if (_agent.remainingDistance < 1.5f)
                    {
                        GenerateRandomFrightenedPosition();
                    }

                    Debug.DrawLine(transform.position, _agent.destination, Color.cyan);
                    break;

                default:
                    _agent.speed = _stopSpeed;
                    break;
            }
        }
    }
    IEnumerator Test()
    {
        InkyCanMove = false;
        yield return new WaitForSeconds(2f);
        InkyCanMove = true;
    }

    bool IsPositionReachable()
    {
        if (_agent.hasPath)
        {
            Debug.Log("Good Path");
            return true;
        }
        else
        {
            Debug.Log("Bad Path");
            return false;
        }
    }

    public void StartMovement()
    {
        _agent.SetDestination(_inkyScatterPositions[CurrentPosition].position);
        InkyCanMove = true;
        float temp = _agent.speed;      // Agent speed at the time Inky is released (accumulated through Pellet collection)
        temp += _minSpeed;
        _agent.speed = temp;
    }

    private int RandomNumber(int min, int max)
    {
        int newValue = Random.Range(min, max + 1);
        return newValue;
    }

    protected override void RoundCompleted()
    {
        base.RoundCompleted();
        InkyCanMove = false;
        StartRandomValue = RandomNumber(_minStartValue, _maxStartValue);
    }
}
