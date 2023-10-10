using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider), typeof(Animator))]
public class PinkyBehaviour : MonoBehaviour
{
    private int _maxSpeed = 10;

    private bool _canStart;
    public bool CanStart { get { return _canStart; } private set { _canStart = value; } }   

    NavMeshAgent _agent;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _homePos;
    [SerializeField] private Transform _forwardPos;

    private readonly Vector3 _startingPos = new Vector3(0.25f, 0, 0);
    private const float _speedIncrement = 0.02f;

    // Blinky starts directly above the exit
    // As soon as Blinky moves out of the doorway, Pinky can leave
    // Pinky speed is the same as Blinky & Inky, which means that this AI can also subscribe to the pellet collecting event

    void OnEnable()
    {
        ItemCollection.OnItemCollected += PelletCollected;
        EnemyCollision.OnEnemyCollision += RestartPosition;
    }

    void Start()
    {
        _canStart = false;
        _agent = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        if(_canStart == true)
        {
            _agent.destination = _forwardPos.position;
            Debug.DrawLine(transform.position, _forwardPos.position, Color.red);
        }
        else
        {
            return;
        }
    }

    // TODO - When Reset level takes place, reset enemy speed
    void IncrementAgentSpeed()
    {
        if (_agent.speed < _maxSpeed)
            _agent.speed += _speedIncrement;
        else
        {
            _agent.speed = _maxSpeed;
            return;
        }
    }

    // Once Blinky has moved outside of his start box - Set destination for Pinky to start moving
    public void SetDestination()
    {
        CanStart = true;
    }

    #region Events
    void PelletCollected(int value)
    {
        IncrementAgentSpeed();
    }

    void RestartPosition()
    {
        _agent.Warp(_startingPos);
    }

    #endregion
    void OnDisable()
    {
        ItemCollection.OnItemCollected -= PelletCollected;
        EnemyCollision.OnEnemyCollision -= RestartPosition; 
    }
}
