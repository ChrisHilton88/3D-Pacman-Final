using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float _speed = 12f;

    private Vector3 _moveDirection;
    private readonly Vector3 _startingPos = new Vector3(0.25f, 1f, -32f);

    CharacterController _cc;


    void OnEnable()
    {
        EnemyCollision.OnEnemyCollision += RestartPosition;
        RoundManager.OnRoundEnd += RestartPosition;
    }

    void Start()
    {
        _cc = GetComponent<CharacterController>();  
        transform.position = _startingPos;  
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

    void RestartPosition()
    {
        _cc.enabled = false;
        transform.position = _startingPos;
        _cc.enabled = true;
    }
}
