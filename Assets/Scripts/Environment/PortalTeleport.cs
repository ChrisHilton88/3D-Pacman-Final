using UnityEngine;

public class PortalTeleport : MonoBehaviour
{
    [SerializeField] private Transform targetPortal;      // Reference to the target portal



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject != targetPortal.gameObject) 
        {
            TeleportPlayer(other.transform);        // Teleport the player
        }
    }

    void TeleportPlayer(Transform playerTransform)
    {
        CharacterController characterController = playerTransform.GetComponent<CharacterController>();
        characterController.enabled = false;
        playerTransform.transform.position = targetPortal.position;
        characterController.enabled = true; 
    }
}
