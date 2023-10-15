using UnityEngine;
using UnityEngine.AI;

public class PortalTeleport : MonoBehaviour
{
    [SerializeField] private Transform _targetPortal;      // Reference to the target portal
    [SerializeField] private CharacterController _cc;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject != _targetPortal.gameObject)
        {
            TeleportPlayer(other.transform);       
        }
        else if (other.CompareTag("Enemy") && other.gameObject != _targetPortal.gameObject)
        {
            Debug.Log("Tagged Enemy in Teleport");
            TeleportEnemy(other.transform);
        }
    }

    void TeleportPlayer(Transform playerTransform)
    {
        _cc.enabled = false;
        playerTransform.transform.position = _targetPortal.position;
        _cc.enabled = true; 
    }

    void TeleportEnemy(Transform enemyTransform)
    {
        NavMeshAgent navMeshAgent = enemyTransform.GetComponent<NavMeshAgent>();
        Vector3 temp = navMeshAgent.destination;
        navMeshAgent.isStopped = true;
        navMeshAgent.Warp(_targetPortal.position);
        navMeshAgent.isStopped = false;
        navMeshAgent.destination = temp;    
    }
}
