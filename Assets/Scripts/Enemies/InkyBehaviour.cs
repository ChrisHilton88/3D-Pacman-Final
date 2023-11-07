using UnityEngine;

public class InkyBehaviour : EnemyBase
{
    private int _startRandomValue;       // Choose a starting value between 30 - 40% of total pellet count. This random value will be used to start moving Inky
    private int _minStartValue = 30, _maxStartValue = 40;     // 30% & 40% of total pellet count (240)

    [SerializeField] private bool _inkyCanMove;

    private Vector3 _inkyStartingPosition = new Vector3(-5f, 0f, 0f);

    [SerializeField] private Transform[] _inkyScatterPositions;
    [SerializeField] private Transform _blinkyPos;
    [SerializeField] private Transform _inkyTargetPacmanPos;        // Double the length of the vector from 2 tiles in front of Pacmans position and Blinkys pos


    #region Properties
    public bool InkyCanMove { get { return _inkyCanMove; } private set { _inkyCanMove = value; } }
    public int StartRandomValue { get { return _startRandomValue; } private set { _startRandomValue = value; } }
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

    protected override void Start()
    {
        base.Start();
        _agent.isStopped = true;
    }

    protected override void EnemyInitialisation()
    {
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

    public void StartMovement()
    {
        _agent.destination = _inkyScatterPositions[CurrentPosition].position;
        InkyCanMove = true;
        _agent.isStopped = false;
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
        Debug.Log("Inky Start Random Value 2: " + StartRandomValue);
    }
}
