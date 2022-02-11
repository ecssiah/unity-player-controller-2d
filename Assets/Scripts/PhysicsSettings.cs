using UnityEngine;

[CreateAssetMenu(fileName = "PhysicsSettings", menuName = "Physics Settings")]
public class PhysicsSettings : ScriptableObject
{
	public float Gravity = -9.8f;

	public float MinimumVelocity = 0.2f;
	public float TerminalVelocity = -53f;

	public float WallSlideDamping = 0.1f;

	public Vector2 HangPositionOffset = new Vector2(0, -1.3f);
	public Vector2 ClimbLedgePosition = new Vector2(0.55f, 1.20f);
}
