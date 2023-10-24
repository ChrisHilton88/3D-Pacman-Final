using UnityEngine;

// Responsible for telling Pinky that Blinky has left the cube and Pinky can start moving
public class BlinkyExitCube : MonoBehaviour
{
    [SerializeField] private TestPinky _testPinky;


    void OnEnable()
    {
        gameObject.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("StartMoving has been called");
            _testPinky.StartMoving();
            gameObject.SetActive(false);  
        }
    }
}
