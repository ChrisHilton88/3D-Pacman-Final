using UnityEngine;

public class ClydeBehaviour : EnemyBase
{
    private int _movePelletCount;
    private int _clydeCurrentChasePosition;

    private float _maxDistance = 8;

    [SerializeField] private bool _clydeCanMove;

    private readonly Vector3 _clydeStartingPosition = new Vector3(6f, 0, -0.25f);

    [SerializeField] private Transform _clydeTargetPacmanPos;       // If > 8 tiles, target becomes Blinky's target tile (Pacman). If < 8 tiles, set to Scatter mode tiles
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

    protected override void EnemyInitialisation()
    {
        _scatterPositions = _clydeScatterPositions;
        _startingPosition = _clydeStartingPosition;
        ClydeCanMove = false;
        MovePelletCount = 80;       // 1/3 of total pellet count
    }

    protected override void CheckState()
    {
        if (ClydeCanMove)
        {
            switch (_currentState)
            {
                case EnemyState.Scatter:
                    Debug.DrawLine(transform.position, _scatterPositions[CurrentPosition].position, Color.yellow);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                    break;

                case EnemyState.Chase:
                    if (Vector2.Distance(transform.position, _pacmanTargetPos.position) > _maxDistance)     // If distance between Clyde and pacman is greater than 8 tiles
                    {
                        _agent.SetDestination(_pacmanTargetPos.position);     // Same target as Blinky - Pacmans current tile
                        Debug.DrawLine(transform.position, _pacmanTargetPos.position, Color.yellow);        // Should be to Pacman
                    }
                    else
                    {
                        _agent.SetDestination(_clydeChasePositions[ClydeCurrentChasePosition].position);
                        Debug.DrawLine(transform.position, _clydeChasePositions[ClydeCurrentChasePosition].position, Color.yellow);

                        if (_agent.remainingDistance < 1.5f)
                        {
                            CalculateNextDestination();
                        }
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
                    _agent.speed = _stopSpeed;
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
        _agent.speed = _minSpeed;
    }

    protected override void RoundCompleted()
    {
        base.RoundCompleted();
        ClydeCanMove = false;
    }
}
