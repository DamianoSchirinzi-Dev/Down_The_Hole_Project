using EazyCamera;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // assign the actions asset to this field in the inspector:
    public InputActionAsset actions;
    public PlayerController playerController;
    public EazyController eazyController;
    public TimeManager timeManager;

    // private field to store move action reference
    private InputAction moveAction;
    private InputAction lookAction;

    public bool isControllerConnected;

    Vector2 _moveVector;
    Vector2 _lookVector;

    void Awake()
    {
        // find the "move" action, and keep the reference to it, for use in Update
        moveAction = actions.FindActionMap("gameplay").FindAction("move");
        lookAction = actions.FindActionMap("gameplay").FindAction("look");

        // for the "jump" action, we add a callback method for when it is performed
        actions.FindActionMap("gameplay").FindAction("jump").performed += OnJump;
        actions.FindActionMap("gameplay").FindAction("interact").performed += OnInteract;
        actions.FindActionMap("gameplay").FindAction("focus").performed += OnSlowMo;
        actions.FindActionMap("gameplay").FindAction("stopfocus").performed += StopSlowMo;
        actions.FindActionMap("gameplay").FindAction("sprint").performed += OnSprint;
        actions.FindActionMap("gameplay").FindAction("stopsprint").performed += StopSprint;
    }

    void Update()
    {
        // our update loop polls the "move" action value each frame
        if (!playerController.isPaused)
        {
            _moveVector = moveAction.ReadValue<Vector2>();
        }

        _lookVector = lookAction.ReadValue<Vector2>();

        CheckIfControllerConnected();
    }

    private void FixedUpdate()
    {
        if (!playerController.isPaused || !playerController.isDead)
        {
            playerController.Move(_moveVector);
            eazyController.ControlCameraRotation(_lookVector, Time.deltaTime, isControllerConnected);
        }
    }

    private void OnSlowMo(InputAction.CallbackContext context)
    {
        timeManager.DoSlowMoTime();
    }
    private void StopSlowMo(InputAction.CallbackContext context)
    {
        timeManager.StopSlowMoTime();
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        playerController.isSprinting = true;
    }
    private void StopSprint(InputAction.CallbackContext context)
    {
        playerController.isSprinting = false;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (playerController.isGrounded)
        {
            playerController.Jump();
        }
    }
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (playerController.canInteract)
        {
            playerController.Interact();
        }
    }

    void OnEnable()
    {
        actions.FindActionMap("gameplay").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("gameplay").Disable();
    }

    private void CheckIfControllerConnected()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            isControllerConnected = false;
            return;
        }

        if (gamepad is DualShockGamepad || gamepad is XInputController)
        {
            isControllerConnected = true;
        }
    }
}
