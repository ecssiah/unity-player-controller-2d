using UnityEngine;

namespace C0
{
	public class ClimbState : PlayerState
	{
		public override void Init()
		{
			player.SetAnimation("Climb");
			player.SetVelocity(Vector2.zero);
			player.SetGravityScale(0);
		}

		public override void UpdateManaged()
		{
			UpdateTriggers();

			if (!TriggerInfo.Climb)
			{
				player.SetState(PlayerStateType.Move);
			}
			else if (Mathf.Approximately(player.Velocity.y, -settings.ClimbSpeed.y) && TriggerInfo.Ground)
			{
				player.SetState(PlayerStateType.Move);
			}
			else if (InputInfo.Direction.y > 0 && TriggerInfo.Ledge)
			{
				player.SetState(PlayerStateType.Hang);
			}
			else
			{
				player.UpdateFacing();
			}
		}

		public override void FixedUpdateManaged()
		{
			player.SetVelocity(InputInfo.Direction * settings.ClimbSpeed);
		}

		public override void SetVerticalInput(float inputValue)
		{
			base.SetVerticalInput(inputValue);

			if (InputInfo.Direction.y == 0)
			{
				player.SetAnimationSpeed(0);
			}
			else
			{
				player.SetAnimationSpeed(1);
			}
		}

		public override void SetJumpInput(float inputValue)
		{
			base.SetJumpInput(inputValue);

			if (inputValue == 1)
			{
				player.SetState(PlayerStateType.Move);
				player.SetVelocity(player.Velocity.x, settings.JumpVelocity);
			}
		}
	}
}