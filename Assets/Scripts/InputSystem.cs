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
		private float previousJumpInput;

		public void AwakeSystem()
		{
			player = GameObject.Find("Player").GetComponent<Player>();

			playerInputActions = new PlayerInputActions();

			moveAction = playerInputActions.Player.Move;
			jumpAction = playerInputActions.Player.Jump;

			previousMoveInput = Vector2.zero;
			previousJumpInput = 0;
		}

		public void UpdateSystem()
		{
			if (player.ClimbingLedge)
			{
				return;
			}

			PollMoveInput();
			PollJumpInput();
		}

		private void PollMoveInput()
		{
			Vector2 currentMoveInput = moveAction.ReadValue<Vector2>();

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

		private void PollJumpInput()
		{
			float currentJumpInput = jumpAction.ReadValue<float>();

			if (currentJumpInput != previousJumpInput)
			{
				player.SetJumpInput(currentJumpInput);
			}

			previousJumpInput = currentJumpInput;
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
	}
}