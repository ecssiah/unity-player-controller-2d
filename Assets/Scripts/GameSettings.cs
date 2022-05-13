using UnityEngine;

namespace C0
{
	[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
	public class GameSettings : ScriptableObject
	{
		[Space]
		public float MinSpeed = 0.01f;
		public float RunSpeed = 12f;
		public Vector2 ClimbSpeed = new Vector2(1.8f, 3.2f);

		[Space]
		public Vector2 JumpForce = new Vector2(0, 16f);
		public Vector2 WallJumpForceRight = new Vector2(6f, 12f);
		public Vector2 WallJumpForceLeft = new Vector2(-6f, 12f);

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