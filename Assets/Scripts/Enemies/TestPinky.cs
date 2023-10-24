using UnityEngine;

public class TestPinky : EnemyBase
{
    private bool _pinkyCanMove;

    private Vector3 _pinkyStartingPosition = new Vector3(0.25f, 0, 0);

    [SerializeField] private Transform[] _pinkyScatterPositions;
    [SerializeField] private Transform _pinkyTargetPacmanPos;


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

    protected override void Start()
    {
        base.Start();
        _agent.isStopped = true;
    }

    protected override void EnemyInitialisation()
    {
        _scatterPositions = _pinkyScatterPositions;
        _startingPosition = _pinkyStartingPosition;
        _pacmanTargetPos = _pinkyTargetPacmanPos;
        PinkyCanMove = false;
    }

    protected sealed override void CheckState()
    {
        switch (_currentState)
        {
            case EnemyState.Scatter:
                Debug.Log("Test 1");
                if (PinkyCanMove && _agent.hasPath)
                {
                    Debug.Log("Test 2");
                    _agent.isStopped = false;
                    Debug.DrawLine(transform.position, _scatterPositions[CurrentPosition].position, Color.magenta);

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

    public void StartMoving()
    {
        _agent.destination = _pinkyScatterPositions[CurrentPosition].position;
        PinkyCanMove = true;
    }
}
