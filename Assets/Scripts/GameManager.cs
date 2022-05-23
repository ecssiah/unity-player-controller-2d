using UnityEngine;
using UnityEngine.Tilemaps;

namespace C0
{
	public class GameManager : MonoBehaviour
	{
		private InputSystem inputSystem;
		private CameraSystem cameraSystem;
		private MapSystem mapSystem;
		
		private Player player;

		void Awake()
		{
			inputSystem = GetComponent<InputSystem>();
			inputSystem.AwakeManaged();

			cameraSystem = GetComponent<CameraSystem>();
			cameraSystem.AwakeManaged();

			mapSystem = GetComponent<MapSystem>();
			mapSystem.AwakeManaged();
			
			player = GameObject.Find("Player").GetComponent<Player>();
			player.AwakeManaged();
		}

		void Start()
		{
			mapSystem.StartManaged();

			player.StartManaged();
		}

		void Update()
		{
			inputSystem.UpdateManaged();

			player.UpdateManaged();
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