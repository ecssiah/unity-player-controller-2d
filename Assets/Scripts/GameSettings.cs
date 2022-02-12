using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
	[Space]
	public float Gravity = -9.8f;
	public float MinSpeed = 0.2f;
	public float MaxAirSpeed = -53f;

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
