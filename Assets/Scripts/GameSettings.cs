using UnityEngine;

namespace C0
{
	[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
	public class GameSettings : ScriptableObject
	{
		public float DefaultGravityScale = 4.0f;
		public float FallingGravityScale = 7.0f;
		public float WallSlideGravityScale = 0.4f;
		public float TerminalVelocity = -30f;

		[Space]
		public float MinHorizontalMovementSpeed = 0.11f;
		public float MinRunSpeed = 0.01f;
		public float MinJumpSpeed = 0.01f;
		public float MinFallSpeed = 0.01f;
		public float RunSpeed = 12f;
		public float HorizontalDamping = 0.27f;
		public Vector2 ClimbSpeed = new Vector2(1.8f, 3.2f);

		[Space]
		public float JumpForce = 18f;
		public Vector2 WallJumpForceRight = new Vector2(960f, 16f);
		public Vector2 WallJumpForceLeft = new Vector2(-960f, 16f);

		[Space]
		public float WallSlideHoldTime = 0.3f;

		[Space]
		public float HangTime = 0.2f;
		public Vector2 HangPositionOffset = new Vector2(-0.185f, -1.37f);

		[Space]
		public Vector2 ClimbLedgeOffsetRight = new Vector2(0.62f, 1.37f);
		public Vector2 ClimbLedgeOffsetLeft = new Vector2(-0.62f, 1.37f);
	}
}