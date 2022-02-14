using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
	[Space]
	public float Gravity = -9.8f;
	public float MinSpeed = 0.1f;
	public float MaxFallSpeed = -53f;
	public float GroundSpeedSmoothTime = 0.24f;
	public float AirSpeedSmoothTime = 0.05f;
	public float RunSpeed = 8f;
	public Vector2 ClimbSpeed = new Vector2(1.8f, 3.2f);

	[Space]
	public float MinJumpSpeed = 6f;
	public Vector2 JumpVelocity = new Vector2(0, 24f);
	public Vector2 WallJumpVelocity = new Vector2(10f, 16f);

	[Space]
	public float WallSlideHoldTime = 0.3f;
	public float WallSlideDamping = 0.2f;
	public float MaxWallSlideSpeed = 4.2f;

	[Space]
	public float HangTime = 0.2f;
	public Vector2 HangPositionOffset = new Vector2(0.19f, -1.38f);
	
	[Space]
	public Vector2 ClimbLedgeOffset = new Vector2(0.62f, 1.18f);
}
