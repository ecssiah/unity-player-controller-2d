using UnityEngine;

namespace C0
{
	public class PhysicsSystem : MonoBehaviour
	{
		private GameSettings gameSettings;

		private Player player;

		public void AwakeSystem()
		{
			gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

			player = GameObject.Find("Player").GetComponent<Player>();
		}

		public void UpdateSystem()
		{
			if (player.ClimbingLedge)
			{
				return;
			}

			player.UpdateState();
		}

		public void FixedUpdateSystem()
		{
			if (player.ClimbingLedge)
			{
				return;
			}

			if (player.Hanging || player.WallSliding)
			{
				return;
			}

			if (player.Ducking)
			{
				UpdateDuckingMovement();
			}
			else if (player.Climbing)
			{
				UpdateClimbingMovement();
			}
			else
			{
				UpdateRunningMovement();
			}
		}

		private void UpdateDuckingMovement()
		{
			Vector2 newVelocity = Vector2.zero;

			newVelocity.x = Mathf.SmoothDamp(
				player.RigidBody2D.velocity.x,
				0,
				ref player.CurrentDampedVelocity,
				gameSettings.GroundSpeedSmoothTime
			);

			EnforceMinMoveSpeed(ref newVelocity.x);

			player.RigidBody2D.velocity = newVelocity;
		}

		private void UpdateClimbingMovement()
		{
			player.RigidBody2D.velocity = player.InputInfo.Direction * gameSettings.ClimbSpeed;
		}

		private void UpdateRunningMovement()
		{
			Vector2 newVelocity = player.RigidBody2D.velocity;

			newVelocity.x = Mathf.SmoothDamp(
				newVelocity.x,
				player.InputInfo.Direction.x * gameSettings.RunSpeed,
				ref player.CurrentDampedVelocity,
				player.TriggerInfo.Ground ? gameSettings.GroundSpeedSmoothTime : gameSettings.AirSpeedSmoothTime
			);

			EnforceMinMoveSpeed(ref newVelocity.x);
			EnforceTerminalVelocity(ref newVelocity.y);
			
			player.RigidBody2D.velocity = newVelocity;

			UpdateGravityScale();
			CheckIfPlayerOutOfBounds();
		}

		private void EnforceMinMoveSpeed(ref float speed)
		{
			if (Mathf.Abs(speed) < gameSettings.MinMoveSpeed)
			{
				speed = 0;
				player.CurrentDampedVelocity = 0;
			}
		}

		private void EnforceTerminalVelocity(ref float speed)
		{
			if (speed < gameSettings.TerminalVelocity)
			{
				speed = gameSettings.TerminalVelocity;
			}
		}

		private void UpdateGravityScale()
		{
			if (player.TriggerInfo.Ground)
			{
				player.RigidBody2D.gravityScale = gameSettings.DefaultGravityScale;
			}
			else if (player.RigidBody2D.velocity.y < -gameSettings.MinFallSpeed)
			{
				player.RigidBody2D.gravityScale = gameSettings.FallingGravityScale;
			}
		}

		private void CheckIfPlayerOutOfBounds()
		{
			if (player.Position.y < -20)
			{
				player.SetPosition(gameSettings.StartPosition);
				player.RigidBody2D.velocity = Vector2.zero;
			}
		}
	}
}
