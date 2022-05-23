using UnityEngine;

namespace C0
{
	public class MoveState : PlayerState
	{
		public MoveState(Player player, GameSettings settings) : base(player, settings)
		{
			Type = PlayerStateType.Move;
		}

		public override void Init()
		{
			player.SetAnimation("Idle");
			player.SetGravityScale(settings.DefaultGravityScale);
		}

		public override void Update()
		{
			player.UpdateTriggers();

			if (player.InputInfo.Direction.y < 0 && player.TriggerInfo.Ground)
			{
				player.SetState(PlayerStateType.Duck);
			}
			else if (player.InputInfo.Direction.y > 0 && player.TriggerInfo.Ledge)
			{
				player.SetState(PlayerStateType.Hang);
			}
			else if (player.InputInfo.Direction.y != 0 && player.TriggerInfo.Climb)
			{
				player.SetState(PlayerStateType.Climb);
			}
			else if (player.InputInfo.Direction.x == player.Facing && player.TriggerInfo.WallSlide)
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
				if (player.TriggerInfo.Ground)
				{
					player.SetGravityScale(settings.DefaultGravityScale);
				}
				else if (player.Velocity.y < -settings.MinFallSpeed)
				{
					player.SetGravityScale(settings.FallingGravityScale);
				}

				player.UpdateAnimation();
				player.UpdateOrientation();
			}
		}

		public override void FixedUpdate()
		{
			Vector2 newVelocity = player.Velocity;

			newVelocity.x = Mathf.SmoothDamp(
				player.Velocity.x,
				player.InputInfo.Direction.x * settings.RunSpeed,
				ref player.VelocityXDamped,
				player.TriggerInfo.Ground ? settings.GroundSpeedSmoothTime : settings.AirSpeedSmoothTime
			);

			player.SetVelocity(newVelocity);
		}

		public override void SetJumpInput(float inputValue)
		{
			base.SetJumpInput(inputValue);

			if (inputValue == 1)
			{
				if (player.TriggerInfo.Ground)
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