using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    InputSystem_Actions playerInput;
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
    public event EventHandler OnBindingRebind;
    public enum Binding {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlt,
        Pause,
        Gamepad_Interact,
        Gamepad_InteractAlternate,
        Gamepad_Pause,
    }
    private void Awake() {
        Instance = this;
        playerInput = new InputSystem_Actions();
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS)) {
            playerInput.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        playerInput.Enable();

        playerInput.Player.Interact.performed += Interact_performed;
        playerInput.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInput.Player.Pause.performed += Pause_performed;
    }
    private void OnDisable() {
        playerInput.Player.Interact.performed -= Interact_performed;
        playerInput.Player.InteractAlternate.performed -= InteractAlternate_performed;
        playerInput.Player.Pause.performed -= Pause_performed;
        playerInput.Dispose();
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInput.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }

    public string GetBindingText(Binding binding) {
        string returnString = "";
        switch (binding) {
            case Binding.Move_Up:
                returnString = playerInput.Player.Move.bindings[1].ToDisplayString();
                break;
            case Binding.Move_Down:
                returnString = playerInput.Player.Move.bindings[2].ToDisplayString();
                break;
            case Binding.Move_Left:
                returnString = playerInput.Player.Move.bindings[3].ToDisplayString();
                break;
            case Binding.Move_Right:
                returnString = playerInput.Player.Move.bindings[4].ToDisplayString();
                break;
            case Binding.Interact:
                returnString = playerInput.Player.Interact.bindings[0].ToDisplayString();
                break;
            case Binding.InteractAlt:
                returnString = playerInput.Player.InteractAlternate.bindings[0].ToDisplayString();
                break;
            case Binding.Pause:
                returnString = playerInput.Player.Pause.bindings[0].ToDisplayString();
                break;
            case Binding.Gamepad_Interact:
                returnString = playerInput.Player.Interact.bindings[1].ToDisplayString();
                break;
            case Binding.Gamepad_InteractAlternate:
                returnString = playerInput.Player.InteractAlternate.bindings[1].ToDisplayString();
                break;
            case Binding.Gamepad_Pause:
                returnString = playerInput.Player.Pause.bindings[1].ToDisplayString();
                break;
            default:
                return "Attmpeted to get binding string for unsupported binding";
        }
        if (returnString.Length > 3) {
            returnString = returnString.Substring(0, 3);
        }
        return returnString;
    }
    public void RebindBinding(Binding binding, Action onActionRebound) {
        playerInput.Disable();
        InputAction inputAction;
        int bindingIndex;
        switch (binding) {
            default:
            case Binding.Move_Up:
                inputAction = playerInput.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = playerInput.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = playerInput.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = playerInput.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerInput.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlt:
                inputAction = playerInput.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInput.Player.Pause;
                bindingIndex = 0;
                break;
            case Binding.Gamepad_Interact:
                inputAction = playerInput.Player.Interact;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_InteractAlternate:
                inputAction = playerInput.Player.InteractAlternate;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_Pause:
                inputAction = playerInput.Player.Pause;
                bindingIndex = 1;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback => {
                callback.Dispose();
                playerInput.Enable();
                onActionRebound();
                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInput.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
                OnBindingRebind?.Invoke(this, EventArgs.Empty);
            })
            .Start();
    }
}
