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

			if (player.Hanging)
			{
				player.ClimbLedgeCheck();
			}
			else
			{
				player.UpdateState();
			}
		}

		public void FixedUpdateSystem()
		{
			if (player.ClimbingLedge)
			{
				return;
			}

			if (player.Hanging)
			{
			}
			else
			{
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
