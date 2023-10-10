using UnityEngine;
using UnityEngine.AI;

public class InkyBehaviour : MonoBehaviour
{
    private int _maxSpeed = 10;

    NavMeshAgent _agent;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _homePos;
    private readonly Vector3 _startingPos = new Vector3(-5f, 0, 0f);
    private const float _speedIncrement = 0.02f;       // (10% - 5% / 240) = 5/240. Or, (maximum allowed speed - starting speed / total pellets)



    void OnEnable()
    {
        ItemCollection.OnItemCollected += PelletCollected;
        EnemyCollision.OnEnemyCollision += RestartPosition;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

    }

    void Update()
    {
        
    }

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
