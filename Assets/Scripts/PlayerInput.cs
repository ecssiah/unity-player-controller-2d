using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
	private Player player;

	private PlayerInputActions playerInputActions;

	private InputAction runAction;
	private InputAction climbAction;
	private InputAction jumpAction;

	void Awake()
	{
		player = GetComponent<Player>();

		playerInputActions = new PlayerInputActions();

		runAction = playerInputActions.Player.Run;
		climbAction = playerInputActions.Player.Climb;
		jumpAction = playerInputActions.Player.Jump;

		runAction.started += OnRunStart;
		runAction.canceled += OnRunCancel;

		climbAction.started += OnClimbStart;
		climbAction.canceled += OnClimbCancel;

		jumpAction.started += OnJumpStart;
		jumpAction.canceled += OnJumpCancel;
	}

	void OnEnable()
	{
		runAction.Enable();
		climbAction.Enable();
		jumpAction.Enable();
	}

	void OnDisable()
	{
		runAction.Disable();
		climbAction.Disable();
		jumpAction.Disable();
	}
	
	private void OnRunStart(InputAction.CallbackContext context)
	{
		player.SetRunInput(context.ReadValue<float>());
	}

	private void OnRunCancel(InputAction.CallbackContext context)
	{
		player.SetRunInput(0);
	}

	private void OnClimbStart(InputAction.CallbackContext context)
	{
		player.SetClimbInput(context.ReadValue<float>());
	}

	private void OnClimbCancel(InputAction.CallbackContext context)
	{
		player.SetClimbInput(0);
	}

	private void OnJumpStart(InputAction.CallbackContext context)
	{
		player.SetJumpInput(1);
	}

	private void OnJumpCancel(InputAction.CallbackContext context)
	{
		player.SetJumpInput(0);
	}
}
