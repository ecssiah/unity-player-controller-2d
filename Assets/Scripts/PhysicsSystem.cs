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
			} 
			else if (player.Climbing)
			{
				player.RigidBody2D.gravityScale = 0f;
				player.RigidBody2D.velocity = player.InputInfo.Direction * gameSettings.ClimbSpeed;
			}
			else
			{
				player.RigidBody2D.gravityScale = 2.0f;

				float targetVelocityX = player.InputInfo.Direction.x * gameSettings.RunSpeed;

				float newVelocityX = Mathf.SmoothDamp(
					player.RigidBody2D.velocity.x,
					targetVelocityX,
					ref playerDampedVelocityX,
					0.15f
				);

				if (Mathf.Abs(newVelocityX) < 0.11f)
				{
					newVelocityX = 0;
				}

				player.RigidBody2D.velocity = new Vector2(newVelocityX, player.RigidBody2D.velocity.y);
			}
		}
	}
}
