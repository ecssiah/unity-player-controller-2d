using UnityEngine;

namespace C0
{
	public class GameManager : MonoBehaviour
	{
		private InputSystem inputSystem;
		private PhysicsSystem physicsSystem;

		void Awake()
		{
			inputSystem = GetComponent<InputSystem>();
			physicsSystem = GetComponent<PhysicsSystem>();

			inputSystem.AwakeSystem();
			physicsSystem.AwakeSystem();
		}

		void Update()
		{
			inputSystem.UpdateSystem();
			physicsSystem.UpdateSystem();
		}
	}
}