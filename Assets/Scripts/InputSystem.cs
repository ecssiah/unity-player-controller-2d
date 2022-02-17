using UnityEngine;
using UnityEngine.InputSystem;

namespace C0
{
	public class InputSystem : MonoBehaviour
	{
		private Player player;

		private PlayerInputActions playerInputActions;

		private InputAction moveAction;
		private InputAction jumpAction;

		private Vector2 previousMoveInput;
		private Vector2 currentMoveInput;

		public void AwakeSystem()
		{
			player = GameObject.Find("Player").GetComponent<Player>();
			
			playerInputActions = new PlayerInputActions();

			moveAction = playerInputActions.Player.Move;

			previousMoveInput = Vector2.zero;
			currentMoveInput = Vector2.zero;
			
			jumpAction = playerInputActions.Player.Jump;

			jumpAction.started += OnJumpStart;
			jumpAction.canceled += OnJumpCancel;
		}

		public void StartSystem()
		{
		}

		public void UpdateSystem()
		{
			PollMoveInput();
		}

		private void PollMoveInput()
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
}