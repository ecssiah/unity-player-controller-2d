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

			if (player.Hanging || player.WallSliding != 0)
			{
				return;
			} 
			
			if (player.Ducking)
			{
				float newVelocityX = Mathf.SmoothDamp(
					player.RigidBody2D.velocity.x,
					0,
					ref playerDampedVelocityX,
					gameSettings.HorizontalDamping
				);

				if (Mathf.Abs(newVelocityX) < gameSettings.MinHorizontalMovementSpeed)
				{
					newVelocityX = 0;
				}

				player.RigidBody2D.velocity = new Vector2(newVelocityX, 0);
			}
			else if (player.Climbing)
			{
				player.RigidBody2D.gravityScale = 0f;
				player.RigidBody2D.velocity = player.InputInfo.Direction * gameSettings.ClimbSpeed;
			}
			else
			{
				Vector2 newVelocity;

				float targetVelocityX = player.InputInfo.Direction.x * gameSettings.RunSpeed;

				newVelocity.x = Mathf.SmoothDamp(
					player.RigidBody2D.velocity.x,
					targetVelocityX,
					ref playerDampedVelocityX,
					gameSettings.HorizontalDamping
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

				if (player.RigidBody2D.velocity.y < 0)
				{
					player.RigidBody2D.gravityScale = gameSettings.FallingGravityScale;
				}
				else
				{
					player.RigidBody2D.gravityScale = gameSettings.DefaultGravityScale;
				}

				if (player.Position.y < -20)
				{
					player.SetPosition(0, 4);
					player.RigidBody2D.velocity = Vector2.zero;
				}
			}
		}
	}
}
