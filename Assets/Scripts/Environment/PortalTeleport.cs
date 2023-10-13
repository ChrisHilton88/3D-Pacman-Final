using UnityEngine;
using UnityEngine.AI;

public class PortalTeleport : MonoBehaviour
{
    [SerializeField] private Transform targetPortal;      // Reference to the target portal



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject != targetPortal.gameObject)
        {
            TeleportPlayer(other.transform);        // Teleport the player
        }
        else if (other.CompareTag("Enemy") && other.gameObject != targetPortal.gameObject)
        {
            TeleportEnemy(other.transform);
        }
    }

    void TeleportPlayer(Transform playerTransform)
    {
        CharacterController characterController = playerTransform.GetComponent<CharacterController>();
        characterController.enabled = false;
        playerTransform.transform.position = targetPortal.position;
        characterController.enabled = true; 
    }

    void TeleportEnemy(Transform enemyTransform)
    {
        NavMeshAgent navMeshAgent = enemyTransform.GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = true;
        navMeshAgent.Warp(targetPortal.position);
        navMeshAgent.isStopped = false;
    }
}
