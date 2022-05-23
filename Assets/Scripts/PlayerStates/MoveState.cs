using UnityEngine;

namespace C0
{
	public class MoveState : PlayerState
	{
		public override void Init()
		{
			player.SetAnimation("Idle");
			player.SetGravityScale(settings.DefaultGravityScale);
		}

		public override void UpdateManaged()
		{
			UpdateTriggers();

			if (inputInfo.Direction.y < 0 && triggerInfo.Ground)
			{
				player.SetState(PlayerStateType.Duck);
			}
			else if (inputInfo.Direction.y > 0 && triggerInfo.Ledge)
			{
				player.SetState(PlayerStateType.Hang);
			}
			else if (inputInfo.Direction.y != 0 && triggerInfo.Climb)
			{
				player.SetState(PlayerStateType.Climb);
			}
			else if (inputInfo.Direction.x == player.Facing && triggerInfo.WallSlide)
			{
				player.SetState(PlayerStateType.WallSlide);
			}
			else if (!settings.LevelBounds.Contains(player.Position))
			{
				player.SetFacing(1);
				player.SetPosition(settings.StartPosition);
				player.SetVelocity(Vector2.zero);
			}
			else
			{
				if (triggerInfo.Ground)
				{
					player.SetGravityScale(settings.DefaultGravityScale);
				}
				else if (player.Velocity.y < -settings.MinFallSpeed)
				{
					player.SetGravityScale(settings.FallingGravityScale);
				}

				player.UpdateAnimation();
				player.UpdateFacing();
			}
		}

		public override void FixedUpdateManaged()
		{
			Vector2 newVelocity = player.Velocity;

			newVelocity.x = Mathf.SmoothDamp(
				player.Velocity.x,
				inputInfo.Direction.x * settings.RunSpeed,
				ref VelocityXDamped,
				triggerInfo.Ground ? settings.GroundSpeedSmoothTime : settings.AirSpeedSmoothTime
			);

			player.SetVelocity(newVelocity);
		}

		public override void SetJumpInput(float inputValue)
		{
			base.SetJumpInput(inputValue);

			if (inputValue == 1)
			{
				if (triggerInfo.Ground)
				{
					player.SetVelocity(player.Velocity.x, settings.JumpVelocity);
				}
			}
			else if (inputValue == 0)
			{
				if (player.Velocity.y > 0)
				{
					player.SetGravityScale(settings.FallingGravityScale);
				}
			}
		}
	}
}