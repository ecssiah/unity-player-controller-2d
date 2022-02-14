using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
	private Player player;

	private PlayerInputActions playerInputActions;

	private InputAction moveAction;
	private InputAction jumpAction;

	private Vector2 previousMoveInput;
	public Vector2 MoveInput;

	void Awake()
	{
		player = GetComponent<Player>();

		playerInputActions = new PlayerInputActions();

		moveAction = playerInputActions.Player.Move;
		jumpAction = playerInputActions.Player.Jump;

		jumpAction.started += OnJumpStart;
		jumpAction.performed += OnJumpPerformed;

		previousMoveInput = Vector2.zero;
		MoveInput = Vector2.zero;
	}

	void Update()
	{
		MoveInput = moveAction.ReadValue<Vector2>();

		if (MoveInput.x > 0 && previousMoveInput.x == 0)
		{
			player.SetHorizontalInput(1);
		}
		else if (MoveInput.x < 0 && previousMoveInput.x == 0)
		{
			player.SetHorizontalInput(-1);
		}
		else if (MoveInput.x == 0 && previousMoveInput.x > 0)
		{
			player.SetHorizontalInput(0);
		}
		else if (MoveInput.x == 0 && previousMoveInput.x < 0)
		{
			player.SetHorizontalInput(0);
		}
		else if (MoveInput.x > 0 && previousMoveInput.x < 0)
		{
			player.SetHorizontalInput(1);
		}
		else if (MoveInput.x < 0 && previousMoveInput.x > 0)
		{
			player.SetHorizontalInput(-1);
		}

		if (MoveInput.y > 0 && previousMoveInput.y == 0)
		{
			player.SetVerticalInput(1);
		}
		else if (MoveInput.y < 0 && previousMoveInput.y == 0)
		{
			player.SetVerticalInput(-1);
		}
		else if (MoveInput.y == 0 && previousMoveInput.y > 0)
		{
			player.SetVerticalInput(0);
		}
		else if (MoveInput.y == 0 && previousMoveInput.y < 0)
		{
			player.SetVerticalInput(0);
		}
		else if (MoveInput.y > 0 && previousMoveInput.y < 0)
		{
			player.SetVerticalInput(1);
		}
		else if (MoveInput.y < 0 && previousMoveInput.y > 0)
		{
			player.SetVerticalInput(-1);
		}

		previousMoveInput = MoveInput;
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

	private void OnJumpPerformed(InputAction.CallbackContext context)
	{
		player.SetJumpInput(context.ReadValue<float>());
	}
}
