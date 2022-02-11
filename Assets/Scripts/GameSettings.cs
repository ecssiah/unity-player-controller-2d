using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
	public float Gravity = -9.8f;

	public float MinimumVelocity = 0.2f;
	public float TerminalVelocity = -53f;

	public float WallSlideDamping = 0.1f;

	public float HangTime = 0.1f;

	public Vector2 HangPositionOffset = new Vector2(0.19f, -1.38f);
	public Vector2 ClimbLedgeOffset = new Vector2(0.62f, 1.18f);
}
