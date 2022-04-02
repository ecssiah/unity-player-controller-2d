using System.Collections.Generic;
using System.Linq;
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

			if (player.Hanging)
			{
				player.ClimbLedgeCheck();
			}
			else
			{
				Vector2 targetVelocity = new Vector2(
					player.InputInfo.Direction.x * gameSettings.RunSpeed, 
					player.RigidBody2D.velocity.y
				);

				player.RigidBody2D.velocity = Vector2.SmoothDamp(player.RigidBody2D.velocity, targetVelocity, ref player.Velocity, 0.05f);

				player.UpdateState();
			}
		}

		public void FixedUpdateSystem()
		{
		}
	}
}
