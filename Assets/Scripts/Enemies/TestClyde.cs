using UnityEngine;

public class TestClyde : EnemyBase
{
    private int _movePelletCount;
    private int _clydeCurrentChasePosition;

    private float _maxDistance = 8;

    private bool _clydeCanMove;

    private Vector3 _clydeStartingPosition = new Vector3(6f, 0, -0.25f);

    [SerializeField] private Transform _pacmanPos;
    [SerializeField] private Transform _clydeTargetPacmanPos;
    [SerializeField] private Transform[] _clydeScatterPositions;
    [SerializeField] private Transform[] _clydeChasePositions;

    #region Properties
    public int MovePelletCount { get { return _movePelletCount; } private set { _movePelletCount = value; } }
    public int ClydeCurrentChasePosition { get { return _clydeCurrentChasePosition; } private set { _clydeCurrentChasePosition = value; } }
    public bool ClydeCanMove { get { return _clydeCanMove; } private set { _clydeCanMove = value; } }
    #endregion


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
        _scatterPositions = _clydeScatterPositions;
        _startingPosition = _clydeStartingPosition;
        _pacmanTargetPos = _clydeTargetPacmanPos;
        ClydeCanMove = false;
        MovePelletCount = 80;       // 1/3 of total pellet count
    }

    protected override void CheckState()
    {
        if (ClydeCanMove & _agent.hasPath)
        {
            switch (_currentState)
            {
                case EnemyState.Scatter:
                    _agent.isStopped = false;
                    Debug.DrawLine(transform.position, _scatterPositions[CurrentPosition].position, Color.yellow);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                    break;

                case EnemyState.Chase:
                    if (Vector2.Distance(transform.position, _pacmanPos.position) > _maxDistance)     // If distance between Clyde and pacman is greater than 8 tiles
                    {
                        _agent.destination = _pacmanPos.position;     // Same target as Blinky - Pacmans current tile
                        Debug.DrawLine(transform.position, _pacmanPos.position, Color.yellow);        // Should be to Pacman
                    }
                    else
                    {
                        _agent.destination = _clydeChasePositions[ClydeCurrentChasePosition].position;

                        if (_agent.remainingDistance < 1.5f)
                        {
                            CalculateNextDestination();
                        }

                        Debug.DrawLine(transform.position, _clydeChasePositions[ClydeCurrentChasePosition].position, Color.yellow);
                    }
                    break;

                case EnemyState.Frightened:
                    if (_agent.remainingDistance < 1.5f)
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

    public void StartMovement()
    {
        _agent.destination = _clydeScatterPositions[CurrentPosition].position;
        ClydeCanMove = true;
    }
}
