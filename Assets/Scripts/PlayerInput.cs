using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
	private Player player;

	private PlayerInputActions playerInputActions;

	private InputAction runAction;
	private InputAction jumpAction;

	void Awake()
	{
		player = GetComponent<Player>();

		playerInputActions = new PlayerInputActions();

		runAction = playerInputActions.Player.Run;
		jumpAction = playerInputActions.Player.Jump;

		runAction.started += OnRunStart;
		runAction.canceled += OnRunCancel;

		jumpAction.started += OnJumpStart;
		jumpAction.canceled += OnJumpCancel;
	}

	void OnEnable()
	{
		runAction.Enable();
		jumpAction.Enable();
	}

	void OnDisable()
	{
		runAction.Disable();
		jumpAction.Disable();
	}
	private void OnJumpStart(InputAction.CallbackContext context)
	{
		player.SetJumpInput(1);
	}

	private void OnJumpCancel(InputAction.CallbackContext context)
	{
	}

	private void OnRunStart(InputAction.CallbackContext context)
	{
		player.SetRunInput(context.ReadValue<float>());
	}

	private void OnRunCancel(InputAction.CallbackContext context)
	{
		player.SetRunInput(0);
	}
}
