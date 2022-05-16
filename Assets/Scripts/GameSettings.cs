using UnityEngine;

namespace C0
{
	[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
	public class GameSettings : ScriptableObject
	{
		public Vector2 StartPosition = new Vector2(-41, 2);

		[Space]
		public float DefaultGravityScale = 6.0f;
		public float FallingGravityScale = 8.6f;
		public float WallSlideGravityScale = 0.4f;
		public float TerminalVelocity = -30f;

		[Space]
		public float RunSpeed = 12f;
		public float MinMoveSpeed = 0.01f;
		public float MinRunSpeed = 1.2f;
		public float MinJumpSpeed = 0.01f;
		public float MinFallSpeed = 0.01f;
		public Vector2 ClimbSpeed = new Vector2(1.8f, 3.2f);
		public float GroundSpeedSmoothTime = 0.14f;
		public float AirSpeedSmoothTime = 0.27f;

		[Space]
		public float JumpVelocity = 18f;
		public Vector2 WallJumpVelocity = new Vector2(1200f, 18f);

		[Space]
		public float WallSlideHoldTime = 0.3f;

		[Space]
		public float HangTime = 0.2f;
		public Vector2 HangOffset = new Vector2(-0.185f, -1.37f);

		[Space]
		public Vector2 ClimbLedgeOffset = new Vector2(0.62f, 1.37f);

		[Space]
		public Vector2 CameraTargetSize = new Vector3(0.5f, 1.6f);
		public Vector2 FocusTargetSize = new Vector3(3, 5);
	}
}