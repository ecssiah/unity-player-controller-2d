using UnityEngine;

namespace C0
{
	public class PhysicsSystem : MonoBehaviour
	{
		private Player player;

		public void AwakeSystem()
		{
			player = GameObject.Find("Player").GetComponent<Player>();
		}

		public void UpdateSystem()
		{
			player.CurrentState.Update();
		}

		public void FixedUpdateSystem()
		{
			player.CurrentState.FixedUpdate();
		}
	}
}
