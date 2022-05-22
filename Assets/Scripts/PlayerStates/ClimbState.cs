using UnityEngine;

namespace C0
{
	public class ClimbState : PlayerState
	{
		public ClimbState(Player player, GameSettings settings) : base(player, settings) 
		{ 
			Type = PlayerStateType.Climb;
		}

		public override void Init()
		{
			player.SetAnimation("Climb");
			player.SetVelocity(Vector2.zero);
			player.SetGravityScale(0);
		}

		public override void Update()
		{
			player.UpdateTriggers();

			if (!player.TriggerInfo.Climb)
			{
				player.SetState(PlayerStateType.Move);
			}
			else if (Mathf.Approximately(player.Velocity.y, -settings.ClimbSpeed.y) && player.TriggerInfo.Ground)
			{
				player.SetState(PlayerStateType.Move);
			}
			else if (player.InputInfo.Direction.y > 0 && player.TriggerInfo.Ledge)
			{
				player.SetState(PlayerStateType.Hang);
			}
			else
			{
				player.SetVelocity(player.InputInfo.Direction * settings.ClimbSpeed);
				
				player.UpdateOrientation();
			}
		}

		public override void SetVerticalInput(float inputValue)
		{
			base.SetVerticalInput(inputValue);

			if (player.InputInfo.Direction.y == 0)
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