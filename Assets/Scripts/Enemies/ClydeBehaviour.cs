using UnityEngine;
using UnityEngine.AI;

public class ClydeBehaviour : MonoBehaviour
{
    private int _maxDistance = 8;
    private int _movePelletCount;      
    private bool _canMove;

    #region Properties
    public int MovePelletCount { get {  return _movePelletCount; } private set { _movePelletCount = value; } }  
    public bool CanMove { get { return _canMove; } private set { _canMove = value; } }
    #endregion

    private readonly Vector3 _startingPos = new Vector3(6f, 0, -0.25f);

    NavMeshAgent _agent;

    [SerializeField] private Transform _playerTargetPos;        



    void OnEnable()
    {
        EnemyCollision.OnEnemyCollision += RestartPosition;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.transform.position = _startingPos;
        MovePelletCount = 80;       // 1/3 of total pellet count
        CanMove = false;    
    }

    void Update()
    {
        if (CanMove)
        {
            if(Vector2.Distance(_playerTargetPos.position, transform.position) > _maxDistance)
            {
                Debug.Log("Distance: " + Vector2.Distance(_playerTargetPos.position, transform.position));
                _agent.destination = _playerTargetPos.position;
                Debug.DrawLine(transform.position, _playerTargetPos.position, Color.magenta);
            }
            else
            {
                // Equal to the waypoint loop down the bottom of the map
            }
        }
        else
            return;
    }

    public void StartMovement()
    {
        CanMove = true;
    }

    #region Events
    void RestartPosition()
    {
        _agent.Warp(_startingPos);
    }
    #endregion

    void OnDisable()
    {
        EnemyCollision.OnEnemyCollision -= RestartPosition;
    }
}
