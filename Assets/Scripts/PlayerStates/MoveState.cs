using UnityEngine;

namespace C0
{
	public class MoveState : PlayerState
	{
		public MoveState(GameSettings settings, Player player) : base(settings, player) { }

		public override void Init()
		{
			Player.SetAnimation("Idle");
			Player.SetGravityScale(Settings.DefaultGravityScale);
		}

		public override void UpdateManaged()
		{
			UpdateTriggers();

			if (InputInfo.Move.y < 0 && TriggerInfo.Ground)
			{
				Player.SetState(PlayerStateType.Duck);
			}
			else if (InputInfo.Move.y > 0 && TriggerInfo.Ledge)
			{
				Player.SetState(PlayerStateType.Hang);
			}
			else if (InputInfo.Move.y != 0 && TriggerInfo.Climb)
			{
				Player.SetState(PlayerStateType.Climb);
			}
			else if (InputInfo.Move.x == Player.Facing && TriggerInfo.WallSlide)
			{
				Player.SetState(PlayerStateType.WallSlide);
			}
			else if (!Settings.LevelBounds.Contains(Player.Position))
			{
				Player.SetFacing(1);
				Player.SetPosition(Settings.StartPosition);
				Player.SetVelocity(Vector2.zero);
				Player.SetState(PlayerStateType.Move);
			}
			else
			{
				if (TriggerInfo.Ground)
				{
					Player.SetGravityScale(Settings.DefaultGravityScale);
				}
				else if (Player.Velocity.y < -Settings.MinFallSpeed)
				{
					Player.SetGravityScale(Settings.FallingGravityScale);
				}

				Player.UpdateAnimation();
				Player.UpdateFacing();
			}
		}

		public override void FixedUpdateManaged()
		{
			Vector2 newVelocity = Player.Velocity;

			newVelocity.x = Mathf.SmoothDamp(
				Player.Velocity.x,
				InputInfo.Move.x * Settings.RunSpeed,
				ref VelocityXDamped,
				TriggerInfo.Ground ? Settings.GroundSpeedSmoothTime : Settings.AirSpeedSmoothTime
			);

			Player.SetVelocity(newVelocity);
		}

		public override void SetJumpInput(float inputValue)
		{
			base.SetJumpInput(inputValue);

			if (inputValue == 1)
			{
				if (TriggerInfo.Ground)
				{
					Player.SetVelocity(Player.Velocity.x, Settings.JumpSpeed);
				}
			}
			else if (inputValue == 0)
			{
				if (Player.Velocity.y > 0)
				{
					Player.SetGravityScale(Settings.FallingGravityScale);
				}
			}
		}
	}
}