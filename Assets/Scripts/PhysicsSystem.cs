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
				UpdateDuckMovement();
			}
			else if (player.Climbing)
			{
				UpdateClimbMovement();
			}
			else
			{
				UpdateRunMovement();
			}
		}

		private void UpdateDuckMovement()
		{
			Vector2 newVelocity = Vector2.zero;

			newVelocity.x = Mathf.SmoothDamp(
				player.RigidBody2D.velocity.x,
				0,
				ref player.DampingVelocity,
				gameSettings.GroundSpeedSmoothTime
			);

			if (Mathf.Abs(newVelocity.x) < gameSettings.MinMoveSpeed)
			{
				newVelocity.x = 0;
				player.DampingVelocity = 0;
			}

			player.RigidBody2D.velocity = newVelocity;
		}

		private void UpdateClimbMovement()   
		{
			player.RigidBody2D.velocity = player.InputInfo.Direction * gameSettings.ClimbSpeed;
		}

		private void UpdateRunMovement()
		{
			Vector2 newVelocity = player.RigidBody2D.velocity;

			newVelocity.x = Mathf.SmoothDamp(
				newVelocity.x,
				player.InputInfo.Direction.x * gameSettings.RunSpeed,
				ref player.DampingVelocity,
				player.TriggerInfo.Ground ? gameSettings.GroundSpeedSmoothTime : gameSettings.AirSpeedSmoothTime
			);

			if (Mathf.Abs(newVelocity.x) < gameSettings.MinMoveSpeed)
			{
				newVelocity.x = 0;
			}

			if (newVelocity.y < gameSettings.TerminalVelocity)
			{
				newVelocity.y = gameSettings.TerminalVelocity;
			}

			player.RigidBody2D.velocity = newVelocity;

			if (player.TriggerInfo.Ground)
			{
				player.RigidBody2D.gravityScale = gameSettings.DefaultGravityScale;
			}
			else if (player.RigidBody2D.velocity.y < -gameSettings.MinFallSpeed)
			{
				player.RigidBody2D.gravityScale = gameSettings.FallingGravityScale;
			}

			if (player.Position.y < -20)
			{
				player.SetPosition(gameSettings.StartPosition);
				player.RigidBody2D.velocity = Vector2.zero;
			}
		}
	}
}
