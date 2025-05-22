using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    InputSystem_Actions playerInput;
    public event EventHandler OnInteractAction;
    private void Awake() {
        playerInput = new InputSystem_Actions();
        playerInput.Enable();

        playerInput.Player.Interact.performed += Interact_performed;
    }


    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInput.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }
}
