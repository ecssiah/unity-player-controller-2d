using UnityEngine;

namespace C0
{
	[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
	public class GameSettings : ScriptableObject
	{
		public Vector2 StartPosition = new Vector2(-32, -12.5f);
		public Bounds LevelBounds = new Bounds(Vector3.zero, new Vector3(80, 40, 0));

		[Space]
		public float DefaultGravityScale = 6.0f;
		public float FallingGravityScale = 8.5f;
		public float WallSlideGravityScale = 0.4f;

		[Space]
		public float MinMoveSpeed = 0.01f;
		public float RunSpeed = 10f;
		public float MinRunSpeed = 1.2f;
		public float JumpSpeed = 22f;
		public float MinJumpSpeed = 0.01f;
		public float MinFallSpeed = 0.01f;
		public float MaxFallSpeed = -30f;
		public float DashSpeed = 10f;
		public Vector2 ClimbSpeed = new Vector2(1.8f, 3.2f);
		public Vector2 WallJumpSpeed = new Vector2(1200f, 18f);

		[Space]
		public float GroundSpeedSmoothTime = 0.08f;
		public float AirSpeedSmoothTime = 0.27f;
		public float WallSlideHoldTime = 0.3f;
		public float HangBeforeClimbTime = 0.2f;

		[Space]
		public Vector2 HangOffset = new Vector2(-0.25f, -1.38f);
		public Vector2 ClimbLedgeOffset = new Vector2(0.63f, 1.38f);

		[Space]
		public Vector2 CameraTargetSize = new Vector3(0.5f, 1.6f);
		public Vector2 FocusTargetSize = new Vector3(3, 5);

		[Space]
		public Vector2 WallTriggerSize = new Vector2(0.1f, 0.2f);
	}
}