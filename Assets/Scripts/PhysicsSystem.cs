using UnityEngine;

namespace C0
{
	public class PhysicsSystem : MonoBehaviour
	{
		private GameSettings gameSettings;

		private Player player;
		private float playerDampedVelocityX;

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
			float newVelocityX = Mathf.SmoothDamp(
				player.RigidBody2D.velocity.x,
				0,
				ref playerDampedVelocityX,
				gameSettings.GroundSpeedSmoothTime
			);

			if (Mathf.Abs(newVelocityX) < gameSettings.MinHorizontalMovementSpeed)
			{
				newVelocityX = 0;
			}

			player.RigidBody2D.velocity = new Vector2(newVelocityX, 0);
		}

		private void UpdateClimbMovement() 
		{
			player.RigidBody2D.velocity = player.InputInfo.Direction * gameSettings.ClimbSpeed;
		}

		private void UpdateRunMovement()
		{
			Vector2 newVelocity;

			newVelocity.x = Mathf.SmoothDamp(
				player.RigidBody2D.velocity.x,
				player.InputInfo.Direction.x * gameSettings.RunSpeed,
				ref playerDampedVelocityX,
				player.TriggerInfo.Ground ? gameSettings.GroundSpeedSmoothTime : gameSettings.AirSpeedSmoothTime
			);

			if (Mathf.Abs(newVelocity.x) < gameSettings.MinHorizontalMovementSpeed)
			{
				newVelocity.x = 0;
			}

			newVelocity.y = player.RigidBody2D.velocity.y;

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
