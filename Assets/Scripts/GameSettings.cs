using UnityEngine;

namespace C0
{
	[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
	public class GameSettings : ScriptableObject
	{
		public Vector2 StartPosition = new Vector2(-32, -11f);
		public Bounds LevelBounds = new Bounds(Vector3.zero, new Vector3(80, 40, 0));

		[Space]
		public float DefaultGravityScale = 6.0f;
		public float FallingGravityScale = 8.5f;
		public float WallSlideGravityScale = 0.4f;

		[Space]
		public float RunSpeed = 10f;
		public float MinMoveSpeed = 0.01f;
		public float MinJumpSpeed = 0.01f;
		public float MinFallSpeed = 0.01f;
		public float MaxFallSpeed = -30f;
		public float MinRunSpeed = 1.2f;
		public Vector2 ClimbSpeed = new Vector2(1.8f, 3.2f);

		[Space]
		public float GroundSpeedSmoothTime = 0.08f;
		public float AirSpeedSmoothTime = 0.27f;
		public float WallSlideHoldTime = 0.3f;
		public float HangTimeBeforeClimb = 0.2f;

		[Space]
		public float JumpVelocity = 22f;
		public Vector2 WallJumpVelocity = new Vector2(1200f, 18f);

		[Space]
		public Vector2 HangOffset = new Vector2(-0.185f, -1.37f);
		public Vector2 ClimbLedgeOffset = new Vector2(0.62f, 1.37f);

		[Space]
		public Vector2 CameraTargetSize = new Vector3(0.5f, 1.6f);
		public Vector2 FocusTargetSize = new Vector3(3, 5);

		[Space]
		public Vector2 WallTriggerSize = new Vector2(0.1f, 0.2f);
	}
}