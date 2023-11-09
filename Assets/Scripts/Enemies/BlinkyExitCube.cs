using UnityEngine;

// Responsible for telling Pinky that Blinky has left the cube and Pinky can start moving
public class BlinkyExitCube : MonoBehaviour
{
    [SerializeField] private PinkyBehaviour _pinkyBehaviour;


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Exit Box"))
        {
            _pinkyBehaviour.StartMoving();
            other.gameObject.SetActive(false);          // Disable the exit box

            if (enabled)
                enabled = false;
        }
        else
            return;
    }
}
