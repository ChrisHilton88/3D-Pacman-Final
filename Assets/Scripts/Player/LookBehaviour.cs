using UnityEngine;

// Responsible for calcuting the players look rotation
public class LookBehaviour : MonoBehaviour
{
    public float _sensitivityModifierX;
    public float _sensitivityModifierY;

    private float _maxPitch = 10;

    private float xRotation;
    private float yRotation;

    Vector2 _lookDirection;

    [SerializeField] private Transform _playerOrientation;



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   
        Cursor.visible = false;
    }

    void Update()
    {
        CalculateLookRotation();
    }

    void CalculateLookRotation()
    {
        float mouseX = _lookDirection.x * _sensitivityModifierX * Time.deltaTime;
        float mouseY = _lookDirection.y * _sensitivityModifierY * Time.deltaTime;
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -_maxPitch, _maxPitch);

        // Rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        _playerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void Look(Vector2 look)
    {
        _lookDirection = look;
    }
}
