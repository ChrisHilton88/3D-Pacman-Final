using UnityEngine;
using UnityEngine.AI;

public class ClydeBehaviour : MonoBehaviour
{
    NavMeshAgent _agent;

    private readonly Vector3 _startingPos = new Vector3(6f, 0, -0.25f);


    void OnEnable()
    {
        EnemyCollision.OnEnemyCollision += RestartPosition;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

    }

    void Update()
    {
        
    }

    #region
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
