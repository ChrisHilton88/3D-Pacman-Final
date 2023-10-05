using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float _speed = 10f;

    Vector3 _moveDirection;

    CharacterController _cc;


    void Start()
    {
        _cc = GetComponent<CharacterController>();  
    }

    void Update()
    {
        CalculateMovement();
    }

    public Vector3 CalculateMovement()
    {
        Vector3 forwardDirection = transform.forward * _moveDirection.z + transform.right * _moveDirection.x;
        Vector3 movementVector = forwardDirection * _speed * Time.deltaTime;
        _cc.Move(movementVector);
        return movementVector;
    }

    public void Movement(Vector2 movement)     
    {
        _moveDirection.x = movement.x;
        _moveDirection.y = 0;
        _moveDirection.z = movement.y;
    }
}
