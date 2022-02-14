using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
	private Player player;

	private PlayerInputActions playerInputActions;

	private InputAction moveAction;
	private InputAction jumpAction;

	private Vector2 previousMoveInput;
	private Vector2 currentMoveInput;

	void Awake()
	{
		player = GetComponent<Player>();

		playerInputActions = new PlayerInputActions();

		moveAction = playerInputActions.Player.Move;
		jumpAction = playerInputActions.Player.Jump;

		jumpAction.started += OnJumpStart;
		jumpAction.canceled += OnJumpCancel;

		previousMoveInput = Vector2.zero;
		currentMoveInput = Vector2.zero;
	}

	void Update()
	{
		currentMoveInput = moveAction.ReadValue<Vector2>();

		if (currentMoveInput.x > 0 && previousMoveInput.x <= 0)
		{
			player.SetHorizontalInput(1);
		}
		else if (currentMoveInput.x < 0 && previousMoveInput.x >= 0)
		{
			player.SetHorizontalInput(-1);
		}
		else if (currentMoveInput.x == 0 && previousMoveInput.x != 0)
		{
			player.SetHorizontalInput(0);
		}

		if (currentMoveInput.y > 0 && previousMoveInput.y <= 0)
		{
			player.SetVerticalInput(1);
		}
		else if (currentMoveInput.y < 0 && previousMoveInput.y >= 0)
		{
			player.SetVerticalInput(-1);
		}
		else if (currentMoveInput.y == 0 && previousMoveInput.y != 0)
		{
			player.SetVerticalInput(0);
		}

		previousMoveInput = currentMoveInput;
	}

	void OnEnable()
	{
		moveAction.Enable();
		jumpAction.Enable();
	}

	void OnDisable()
	{
		moveAction.Disable();
		jumpAction.Disable();
	}
	
	private void OnJumpStart(InputAction.CallbackContext context)
	{
		player.SetJumpInput(context.ReadValue<float>());
	}

	private void OnJumpCancel(InputAction.CallbackContext context)
	{
		player.SetJumpInput(context.ReadValue<float>());
	}
}
