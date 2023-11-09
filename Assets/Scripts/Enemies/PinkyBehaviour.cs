using UnityEngine;
using UnityEngine.AI;

public class PinkyBehaviour : EnemyBase
{
    private bool _pinkyCanMove;

    private readonly Vector3 _pinkyStartingPosition = new Vector3(0.5f, 0, 0);

    NavMeshPath _navMeshPath;

    [SerializeField] private Transform[] _pinkyScatterPositions;
    [SerializeField] private Transform _pinkyTargetPacmanPos;       // 4 tiles ahead of Pacman

    #region Properties
    public bool PinkyCanMove { get { return _pinkyCanMove; } private set { _pinkyCanMove = value; } }
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
        _scatterPositions = _pinkyScatterPositions;
        _startingPosition = _pinkyStartingPosition;
        PinkyCanMove = false;
    }

    protected sealed override void CheckState()
    {
        if(PinkyCanMove)
        {
            switch (_currentState)
            {
                case EnemyState.Scatter:
                    Debug.DrawLine(transform.position, _scatterPositions[CurrentPosition].position, Color.magenta);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                    break;

                case EnemyState.Chase:
                    if (IsPositionReachable(_pinkyTargetPacmanPos.position))        // If position is reachable, set as Pinky's original game design destination
                    {
                        _agent.SetDestination(_pinkyTargetPacmanPos.position);
                        Debug.DrawLine(transform.position, _pinkyTargetPacmanPos.position, Color.magenta);
                    }
                    else                                                            // Else, set the position as Pacman's position (as this is always in bounds, on a complete path)
                    {
                        _agent.SetDestination(_pacmanTargetPos.position);
                        Debug.DrawLine(transform.position, _pacmanTargetPos.position, Color.magenta);
                    }
                    break;
                case EnemyState.Frightened:
                    if (_agent.remainingDistance < 1.5f)
                    {
                        GenerateRandomFrightenedPosition();
                    }

                    Debug.DrawLine(transform.position, _agent.destination, Color.magenta);
                    break;

                default:
                    _agent.speed = _stopSpeed;
                    break;
            }
        }
    }

    bool IsPositionReachable(Vector3 position)
    {
        _agent.CalculatePath(position, _navMeshPath);

        if (_navMeshPath.status == NavMeshPathStatus.PathComplete)
            return true;
        else
            return false;
    }

    public void StartMoving()
    {
        _agent.SetDestination(_pinkyScatterPositions[CurrentPosition].position);
        PinkyCanMove = true;
        _agent.speed = _minSpeed;
    }

    protected override void RoundCompleted()
    {
        base.RoundCompleted();
        PinkyCanMove = false;
    }
}
