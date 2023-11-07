using UnityEngine;
using UnityEngine.AI;

// Responsible for slowing the enemy AI in the tunnel
public class TunnelSlowEnemy : MonoBehaviour 
{
    private float _minTunnelSpeed = 2.5f;
    private float _originalSpeed;

    private NavMeshAgent _agent;    


    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();  
    }


    // Store agents current speed
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tunnel"))
        {
            _originalSpeed = _agent.speed;  
        }
        else
            return;
    }

    // Decrement agents speed
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Tunnel"))
        {
            _agent.speed = _minTunnelSpeed;
        }
        else
            return;
    }

    // Restore agents previous speed
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tunnel"))
        {
            _agent.speed = _originalSpeed;      
        }
        else
            return;
    }
}
