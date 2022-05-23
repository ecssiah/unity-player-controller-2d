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

			if (!triggerInfo.Climb)
			{
				player.SetState(PlayerStateType.Move);
			}
			else if (Mathf.Approximately(player.Velocity.y, -settings.ClimbSpeed.y) && triggerInfo.Ground)
			{
				player.SetState(PlayerStateType.Move);
			}
			else if (inputInfo.Direction.y > 0 && triggerInfo.Ledge)
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
			player.SetVelocity(inputInfo.Direction * settings.ClimbSpeed);
		}

		public override void SetVerticalInput(float inputValue)
		{
			base.SetVerticalInput(inputValue);

			if (inputInfo.Direction.y == 0)
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