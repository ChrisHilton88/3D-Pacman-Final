using System.Collections;
using UnityEngine;

public class BlinkyBehaviour : EnemyBase
{
    private readonly Vector3 _blinkyStartingPosition = new Vector3(0.5f, 0, 8.5f);

    private BlinkyExitCube _blinkyExitCube;
    private Coroutine _newRoundRoutine;

    [SerializeField] private GameObject _exitCube;
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
        _blinkyExitCube = GetComponent<BlinkyExitCube>();   
        _agent.speed = _minSpeed;
    }

    protected override void EnemyInitialisation()
    {
        _startingPosition = _blinkyStartingPosition;
        _scatterPositions = _blinkyScatterPositions;
        _pacmanTargetPos = _blinkyTargetPacmanPos;
        _newRoundRoutine = null;
    }

    protected sealed override void CheckState()        
    {
        switch (_currentState)
        {
            case EnemyState.Scatter:

                Debug.DrawLine(transform.position, _scatterPositions[CurrentPosition].position, Color.red);

                if (_agent.remainingDistance < 1.5f)
                {
                    CalculateNextDestination();
                }
                break;

            case EnemyState.Chase:
                _agent.SetDestination(_blinkyTargetPacmanPos.position);
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
                _agent.speed = _stopSpeed;
                Debug.Log(gameObject.name + " isStopped - Default case - CheckState()");
                break;
        }
    }

    protected override void RoundCompleted()
    {
        base.RoundCompleted();
        _exitCube.SetActive(true);
        _blinkyExitCube.enabled = true;
        if(_newRoundRoutine != null)
            StartCoroutine(NewRoundRoutine());
    }

    IEnumerator NewRoundRoutine()
    {
        yield return null;
        _agent.speed = _minSpeed;
    }
}
