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

    private const float _speedIncrement = 0.02f;

    Vector3 newDestination;

    // Blinky starts directly above the exit
    // As soon as Blinky moves out of the doorway, Pinky can leave
    // Pinky speed is the same as Blinky & Inky, which means that this AI can also subscrie to the pellet collecting event

    void OnEnable()
    {
        ItemCollection.onItemCollected += PelletCollected;
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
            newDestination = _forwardPos.position;
            Debug.DrawLine(transform.position, newDestination, Color.red);
        }
        else
        {
            return;
            // Stay in the box
            //newDestination = 
            Debug.DrawLine(transform.position, newDestination, Color.red);
        }

        _agent.destination = newDestination;       
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

    // Event
    void PelletCollected(int value)
    {
        IncrementAgentSpeed();
    }

    void OnDisable()
    {
        ItemCollection.onItemCollected -= PelletCollected;
    }
}
