using UnityEngine;

// Responsible for telling Pinky that Blinky has left the cube and Pinky can start moving
public class BlinkyExitCube : MonoBehaviour
{
    [SerializeField] private PinkyBehaviour _pinky;


    void OnEnable()
    {
        gameObject.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _pinky.SetDestination();
            gameObject.SetActive(false);  
        }
    }
}
