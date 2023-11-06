using UnityEngine;

public class BlinkyBehaviour : EnemyBase
{
    private Vector3 _blinkyStartingPosition = new Vector3(0.5f, 0, 8.5f);

    [SerializeField] private Transform[] _blinkyScatterPositions;
    [SerializeField] private Transform _blinkyTargetPacmanPos;      // Pacmans position


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
        _agent.isStopped = false;
    }

    protected override void EnemyInitialisation()
    {
        _startingPosition = _blinkyStartingPosition;
        _scatterPositions = _blinkyScatterPositions;
        _pacmanTargetPos = _blinkyTargetPacmanPos;
    }

    protected sealed override void CheckState()        
    {
        switch (_currentState)
        {
            case EnemyState.Scatter:
                if (_agent.hasPath)
                {
                    Debug.DrawLine(transform.position, _scatterPositions[CurrentPosition].position, Color.red);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                    }
                }
                break;

            case EnemyState.Chase:
                _agent.destination = _pacmanTargetPos.position;       
                Debug.DrawLine(transform.position, _pacmanTargetPos.position, Color.red);       
                break;

            case EnemyState.Frightened:
                if (_agent.remainingDistance < 1.5f)        // If agent reaches this, find a new destination
                {
                    GenerateRandomFrightenedPosition();
                }

                Debug.DrawLine(transform.position, _agent.destination, Color.red);
                break;

            default:
                _agent.isStopped = true;
                break;
        }
    }
}
