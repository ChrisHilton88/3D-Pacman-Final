using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoSingleton<InputManager>
{
    PlayerInputActions _playerinputActions;

    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private LookBehaviour _lookBehaviour;


    void OnEnable()
    {
        _playerinputActions = new PlayerInputActions();
        _playerinputActions.Player.Enable();
        _playerinputActions.Player.Move.performed += MovePerformed;
        _playerinputActions.Player.Move.canceled += MoveCanceled;
        _playerinputActions.Player.Look.performed += LookPerformed;
        _playerinputActions.Player.Look.canceled += Lookcanceled;

        _playerinputActions.UI.Enable();
        _playerinputActions.UI.Menu.started += MenuPerformed;
    }

    void OnDisable()
    {
        _playerinputActions.Player.Disable();
        _playerinputActions.Player.Move.performed -= MovePerformed;
        _playerinputActions.Player.Move.canceled -= MoveCanceled;
        _playerinputActions.Player.Look.performed -= LookPerformed;
        _playerinputActions.Player.Look.canceled -= Lookcanceled;

        _playerinputActions.UI.Disable();
        _playerinputActions.UI.Menu.started += MenuPerformed;

    }


    #region Player Actions
    void MovePerformed(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        _playerMovement.Movement(move);
    }

    void MoveCanceled(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        _playerMovement.Movement(move);
    }

    public void LookPerformed(InputAction.CallbackContext context)
    {
        Vector2 look = context.ReadValue<Vector2>();    
        _lookBehaviour.Look(look);
    }

    void Lookcanceled(InputAction.CallbackContext context)
    {
        Vector2 look = context.ReadValue<Vector2>();
        _lookBehaviour.Look(look);
    }


    #endregion

    #region UI Actions
    void MenuPerformed(InputAction.CallbackContext context)
    {
        if(context.ReadValue<float>() == 1)
        {
            GameManager.Instance.OpenMenu();
        }
    }
    #endregion

}
