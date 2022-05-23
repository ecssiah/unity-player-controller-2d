using UnityEngine;

namespace C0
{
	public class ClimbState : PlayerState
	{
		public ClimbState(GameSettings settings, Player player) : base(settings, player) { }

		public override void Init()
		{
			Player.SetAnimation("Climb");
			Player.SetVelocity(Vector2.zero);
			Player.SetGravityScale(0);
		}

		public override void UpdateManaged()
		{
			UpdateTriggers();

			if (!TriggerInfo.Climb)
			{
				Player.SetState(PlayerStateType.Move);
			}
			else if (Mathf.Approximately(Player.Velocity.y, -Settings.ClimbSpeed.y) && TriggerInfo.Ground)
			{
				Player.SetState(PlayerStateType.Move);
			}
			else if (InputInfo.Direction.y > 0 && TriggerInfo.Ledge)
			{
				Player.SetState(PlayerStateType.Hang);
			}
			else
			{
				Player.UpdateFacing();
			}
		}

		public override void FixedUpdateManaged()
		{
			Player.SetVelocity(InputInfo.Direction * Settings.ClimbSpeed);
		}

		public override void SetVerticalInput(float inputValue)
		{
			base.SetVerticalInput(inputValue);

			if (InputInfo.Direction.y == 0)
			{
				Player.SetAnimationSpeed(0);
			}
			else
			{
				Player.SetAnimationSpeed(1);
			}
		}

		public override void SetJumpInput(float inputValue)
		{
			base.SetJumpInput(inputValue);

			if (inputValue == 1)
			{
				Player.SetState(PlayerStateType.Move);
				Player.SetVelocity(Player.Velocity.x, Settings.JumpVelocity);
			}
		}
	}
}