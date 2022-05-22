using UnityEngine;

namespace C0
{
	public class GameManager : MonoBehaviour
	{
		private InputSystem inputSystem;
		private CameraSystem cameraSystem;
		
		private Player player;

		void Awake()
		{
			inputSystem = GetComponent<InputSystem>();
			inputSystem.AwakeManaged();

			cameraSystem = GetComponent<CameraSystem>();
			cameraSystem.AwakeManaged();

			player = GameObject.Find("Player").GetComponent<Player>();
			player.AwakeManaged();
		}

		void Start()
		{
			player.StartManaged();
		}

		void Update()
		{
			inputSystem.UpdateManaged();
		}

		void FixedUpdate()
		{
			player.FixedUpdateManaged();
		}

		void LateUpdate()
		{
			cameraSystem.LateUpdateManaged();
		}
	}
}