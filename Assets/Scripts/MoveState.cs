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
			player.CurrentState = this;

			player.RigidBody2D.gravityScale = settings.DefaultGravityScale;
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
			else if (!player.TriggerInfo.Ground && player.TriggerInfo.Wall && player.InputInfo.Direction.x == player.Facing)
			{
				player.SetState(PlayerStateType.WallSlide);
			}
			else
			{
				player.UpdateAnimation();
				player.UpdateOrientation();
			}
		}

		public override void FixedUpdate()
		{
			Vector2 newVelocity = player.RigidBody2D.velocity;

			newVelocity.x = Mathf.SmoothDamp(
				newVelocity.x,
				player.InputInfo.Direction.x * settings.RunSpeed,
				ref player.CurrentDampedVelocity,
				player.TriggerInfo.Ground ? settings.GroundSpeedSmoothTime : settings.AirSpeedSmoothTime
			);

			if (Mathf.Abs(newVelocity.x) < settings.MinMoveSpeed)
			{
				newVelocity.x = 0;
				player.CurrentDampedVelocity = 0;
			}

			if (newVelocity.x < settings.TerminalVelocity)
			{
				newVelocity.x = settings.TerminalVelocity;
			}

			player.RigidBody2D.velocity = newVelocity;

			if (player.TriggerInfo.Ground)
			{
				player.RigidBody2D.gravityScale = settings.DefaultGravityScale;
			}
			else if (player.RigidBody2D.velocity.y < -settings.MinFallSpeed)
			{
				player.RigidBody2D.gravityScale = settings.FallingGravityScale;
			}

			if (player.Position.y < -20)
			{
				player.SetPosition(settings.StartPosition);
				player.RigidBody2D.velocity = Vector2.zero;
			}
		}

		public override void SetJumpInput(float inputValue)
		{
			base.SetJumpInput(inputValue);

			if (inputValue == 1)
			{
				if (player.TriggerInfo.Ground)
				{
					player.RigidBody2D.velocity = new Vector2(player.RigidBody2D.velocity.x, settings.JumpVelocity);
				}
			}
			else if (inputValue == 0)
			{
				if (player.RigidBody2D.velocity.y > 0)
				{
					player.RigidBody2D.gravityScale = settings.FallingGravityScale;
				}
			}
		}
	}
}