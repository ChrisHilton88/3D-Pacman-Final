using UnityEngine;
using UnityEngine.AI;

public class TunnelSlowEnemy : MonoBehaviour
{
    private float _originalSpeed;

    BlinkyBehaviour _blinkybehaviour;
    NavMeshAgent _agent;    
    

    void Start()
    {
        _blinkybehaviour = GetComponent<BlinkyBehaviour>(); 
        _agent = GetComponent<NavMeshAgent>();
    }

    void OnTriggerEnter(Collider other)
    {
        _originalSpeed = _agent.speed;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Tunnel"))
        {
            _blinkybehaviour.DecrementAgentSpeed();
        }
    }

    void OnTriggerExit(Collider other)
    {
        _agent.speed = _originalSpeed;  
    }
}
