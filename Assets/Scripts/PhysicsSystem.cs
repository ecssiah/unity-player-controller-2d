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
				player.UpdateState();
			}
		}
	}
}
