using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider), typeof(Animator))]
public class PinkyBehaviour : MonoBehaviour
{

    private int _maxSpeed = 10;

    NavMeshAgent _agent;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _homePos;
    [SerializeField] private Transform _forwardPos;

    private const float _speedIncrement = 0.02f;

    // Blinky starts directly above the exit
    // As soon as Blinky moves out of the doorway, Pinky can leave
    // Pinky speed is the same as Blinky & Inky, which means that this AI can also subscrie to the pellet collecting event

    void OnEnable()
    {
        ItemCollection.onItemCollected += PelletCollected;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        Vector3 destination = _forwardPos.position;      // Destination is equal to the players forward position GameObject
        _agent.destination = destination;       
        Debug.DrawLine(transform.position, destination, Color.red);
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
